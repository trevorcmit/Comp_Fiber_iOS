using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.BLE.Abstractions.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

// Imports for Plotting
// using LiveChartsCore;
// using LiveChartsCore.Defaults;
// using LiveChartsCore.SkiaSharpView;
// using LiveChartsCore.SkiaSharpView.Painting;
// using SkiaSharp;


namespace BLE_Universal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectedPage : ContentPage
    {
        public IDevice device;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<IService> Serv1 = new ObservableCollection<IService>();
        public ObservableCollection<IService> Serv2 = new ObservableCollection<IService>();
        public ObservableCollection<IService> Serv3 = new ObservableCollection<IService>();
        public ObservableCollection<IService> Serv4 = new ObservableCollection<IService>();
        public ObservableCollection<ICharacteristic> Char1 = new ObservableCollection<ICharacteristic>();
        public ObservableCollection<ICharacteristic> Char2 = new ObservableCollection<ICharacteristic>();
        public ObservableCollection<ICharacteristic> Char3 = new ObservableCollection<ICharacteristic>();
        public ObservableCollection<ICharacteristic> Char4 = new ObservableCollection<ICharacteristic>();

        // Tester OC for updating Temp Data Live
        public ObservableCollection<string> ACCEL_DATA1 = new ObservableCollection<string>();
        public ObservableCollection<string> ACCEL_DATA2 = new ObservableCollection<string>();
        public ObservableCollection<string> ACCEL_DATA3 = new ObservableCollection<string>();

        public DateTime START; // To calculate seconds elapsed in AddOrUpdateTagData()


        public ConnectedPage(IDevice d)
        {
            InitializeComponent();
            device = d;

            Device.BeginInvokeOnMainThread(() =>
            {
                Accel_Data1.ItemsSource = ACCEL_DATA1;
                Accel_Data2.ItemsSource = ACCEL_DATA2;
                Accel_Data3.ItemsSource = ACCEL_DATA3;
            });

            ACCEL_DATA1.Add("N/A");
            ACCEL_DATA2.Add("N/A");
            ACCEL_DATA3.Add("N/A");

            GetPermissions();
            SetupDevice();
        }


        public async void GetPermissions()
        {
            // Formerly held devices permissions. Not necessary on iOS.
            Device.BeginInvokeOnMainThread(async () =>
            {
                connectedDevice.Text = device.Name;  // Set device name at top of UI
            });  
        }


        public async void SetupDevice()
        {
            // Discover Services of Connected Device
            IReadOnlyList<IService> s_ = await device.GetServicesAsync();

            for (int i = 0; i < s_.Count; i++)
                switch (i)
                {
                    case 0:
                        Serv1.Add(s_[i]);
                        IReadOnlyList<ICharacteristic> c1 = await s_[i].GetCharacteristicsAsync();
                        foreach (var c_ in c1) { Char1.Add(c_); }
                        break;
                    case 1:
                        Serv2.Add(s_[i]);
                        IReadOnlyList<ICharacteristic> c2 = await s_[i].GetCharacteristicsAsync();
                        foreach (var c_ in c2) { Char2.Add(c_); }
                        break;
                    default:
                        break;
                }
        }


        // Write Command to BLE Device to trigger MCU collection of accel. data
        async void OnStartClicked(object sender, EventArgs args)
        {
            START = DateTime.Now; // Setup Start DateTime for X-Axis of LiveChart

            // Write Epoch time in exactly 8 bytes of space, with 0x1D command on the head
            // example: { 0x1D, 0x00, 0x00, 0x00, 0x00, 0x5F, 0x5E, 0x5D, 0x5C }
            UInt64 epoch_time = (UInt64)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            byte[] epoch_bytes = BitConverter.GetBytes(epoch_time);
            byte[] epoch_array = new byte[9];
            epoch_bytes.CopyTo(epoch_array, 1);
            epoch_array[0] = 0x1D;

            int error = await Char2[0].WriteAsync( epoch_array );
            Task.Delay(1500).Wait();

            error = await Char2[0].WriteAsync(new byte[] { 0x0C });
            Task.Delay(1500).Wait();

            while (true)
            {
                int error_ = await CollectionCommand();
                if (error_ != 0)
                    continue;
            }
        }


        async void OnCollectClicked(object sender, EventArgs args)
        {
            while (true)
            {
                int error_ = await CollectionCommand();
                if (error_ != 0)
                    continue;
            }
        }


        public async Task<int> CollectionCommand()
        {
            Task.Delay(900).Wait();

            (byte[], int) bytes = await Char2[2].ReadAsync();

            if (bytes.Item1.Count()==6)
            {
                string temp1 = CombineBytesToBinaryString(bytes.Item1[0], bytes.Item1[1]);
                string temp2 = CombineBytesToBinaryString(bytes.Item1[2], bytes.Item1[3]);
                string temp3 = CombineBytesToBinaryString(bytes.Item1[4], bytes.Item1[5]);
                string temp1_string = Math.Round( ParseFloat16(temp1), 1).ToString() + "°";
                string temp2_string = Math.Round( ParseFloat16(temp2), 1).ToString() + "°";
                string temp3_string = Math.Round( ParseFloat16(temp3), 1).ToString() + "°";

                if (!(ACCEL_DATA1.ElementAt(0)==temp1_string))
                {
                    ACCEL_DATA1.RemoveAt(0);
                    ACCEL_DATA1.Add(temp1_string);
                }

                if (!(ACCEL_DATA2.ElementAt(0)==temp2_string))
                {
                    ACCEL_DATA2.RemoveAt(0);
                    ACCEL_DATA2.Add(temp2_string);
                }

                if (!(ACCEL_DATA3.ElementAt(0)==temp3_string))
                {
                    ACCEL_DATA3.RemoveAt(0);
                    ACCEL_DATA3.Add(temp3_string);
                }
            }
            
            return bytes.Item2;
        }


        async void OnChipErase(object sender, EventArgs args)
        {
            bool answer = await DisplayAlert(
                "Be careful!", 
                "Are you sure you want to erase the SPI flash?\n\nThe chip erase takes ~30 seconds.\nWait for the buttons to change color.", 
                "Yes", "No"
            );

            if (answer)
            {
                await Char2[0].WriteAsync(new byte[] { 0xCE });
                await Task.Delay(30000); // wait 30 seconds
            }
        }


        /**************************************************
         * Combine two bytes into a single binary string.
        **************************************************/
        static string CombineBytesToBinaryString(byte byte1, byte byte2)
        {
            return Convert.ToString(byte1, 2).PadLeft(8, '0') + Convert.ToString(byte2, 2).PadLeft(8, '0');
        }


        /****************************************************************************
         * Convert a binary string to a float.
         * Function written by ChatGPT 3.5, converted from my original Python code.
        ****************************************************************************/
        static float ParseFloat16(string binfloat)
        {
            // Map sign bit string to multiplicand
            Dictionary<char, float> S = new Dictionary<char, float>()
            {
                { '0', 1.0f },
                { '1', -1.0f },
            };

            char sign = binfloat[0];
            string exp = binfloat.Substring(1, 5);
            string mantissa = binfloat.Substring(6);

            if (exp == "00000")
            {
                if (mantissa == "0000000000")  // Underflow
                    return 0.0f;
                else
                    return S[sign] * (float)Math.Pow(2, -14) * (Convert.ToInt32(mantissa, 2) * (float)Math.Pow(2, -10));
            }
            else if (exp == "11111")    // Overflow case
            {
                if (sign == '0')
                    return float.NaN;   // Currently set to NaN, this could be float.PositiveInfinity
                else if (sign == '1')
                    return float.NaN;   // Currently set to NaN, this could be float.NegativeInfinity
            }
            else
            {
                float fraction = 1.0f + (Convert.ToInt32(mantissa, 2) * (float)Math.Pow(2, -10));
                return S[sign] * fraction * (float)Math.Pow(2, Convert.ToInt32(exp, 2) - 15); // 15 is half-precision bias
            }

            // This line is unreachable, but C# requires a return statement in all code paths
            return 0.0f;
        }

    }
}
