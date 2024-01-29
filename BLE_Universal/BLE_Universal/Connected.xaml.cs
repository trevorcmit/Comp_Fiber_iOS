using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
// using Plugin.BLE.Abstractions.EventArgs;
// using Plugin.BLE.Abstractions.Exceptions;
// using Plugin.BLE.Abstractions.Utils;
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



namespace BLE_Universal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectedPage : ContentPage
    {
        public IDevice device1, device2, device3, device4, device5;
        public DateTime START;  // To calculate seconds elapsed in AddOrUpdateTagData()

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<ICharacteristic> Char1 = new ObservableCollection<ICharacteristic>();
        public ObservableCollection<ICharacteristic> Char2 = new ObservableCollection<ICharacteristic>();

        public ObservableCollection<IService> d1s1 = new ObservableCollection<IService>();
        public ObservableCollection<IService> d1s2 = new ObservableCollection<IService>();
        public ObservableCollection<IService> d2s1 = new ObservableCollection<IService>();
        public ObservableCollection<IService> d2s2 = new ObservableCollection<IService>();
        public ObservableCollection<IService> d3s1 = new ObservableCollection<IService>();
        public ObservableCollection<IService> d3s2 = new ObservableCollection<IService>();
        public ObservableCollection<IService> d4s1 = new ObservableCollection<IService>();
        public ObservableCollection<IService> d4s2 = new ObservableCollection<IService>();
        public ObservableCollection<IService> d5s1 = new ObservableCollection<IService>();
        public ObservableCollection<IService> d5s2 = new ObservableCollection<IService>();        
        public ObservableCollection<ICharacteristic> d1c1 = new ObservableCollection<ICharacteristic>();
        public ObservableCollection<ICharacteristic> d1c2 = new ObservableCollection<ICharacteristic>();
        public ObservableCollection<ICharacteristic> d2c1 = new ObservableCollection<ICharacteristic>();
        public ObservableCollection<ICharacteristic> d2c2 = new ObservableCollection<ICharacteristic>();
        public ObservableCollection<ICharacteristic> d3c1 = new ObservableCollection<ICharacteristic>();
        public ObservableCollection<ICharacteristic> d3c2 = new ObservableCollection<ICharacteristic>();
        public ObservableCollection<ICharacteristic> d4c1 = new ObservableCollection<ICharacteristic>();
        public ObservableCollection<ICharacteristic> d4c2 = new ObservableCollection<ICharacteristic>();
        public ObservableCollection<ICharacteristic> d5c1 = new ObservableCollection<ICharacteristic>();
        public ObservableCollection<ICharacteristic> d5c2 = new ObservableCollection<ICharacteristic>();
        public ObservableCollection<string> d1_temp1 = new ObservableCollection<string>();
        public ObservableCollection<string> d1_temp2 = new ObservableCollection<string>();
        public ObservableCollection<string> d1_temp3 = new ObservableCollection<string>();
        public ObservableCollection<string> d2_temp1 = new ObservableCollection<string>();
        public ObservableCollection<string> d2_temp2 = new ObservableCollection<string>();
        public ObservableCollection<string> d2_temp3 = new ObservableCollection<string>();
        public ObservableCollection<string> d3_temp1 = new ObservableCollection<string>();
        public ObservableCollection<string> d3_temp2 = new ObservableCollection<string>();
        public ObservableCollection<string> d3_temp3 = new ObservableCollection<string>();
        public ObservableCollection<string> d4_temp1 = new ObservableCollection<string>();
        public ObservableCollection<string> d4_temp2 = new ObservableCollection<string>();
        public ObservableCollection<string> d4_temp3 = new ObservableCollection<string>();
        public ObservableCollection<string> d5_temp1 = new ObservableCollection<string>();
        public ObservableCollection<string> d5_temp2 = new ObservableCollection<string>();
        public ObservableCollection<string> d5_temp3 = new ObservableCollection<string>();

        // Tester OC for updating Temp Data Live
        public ObservableCollection<string> ACCEL_DATA1 = new ObservableCollection<string>();
        public ObservableCollection<string> ACCEL_DATA2 = new ObservableCollection<string>();
        public ObservableCollection<string> ACCEL_DATA3 = new ObservableCollection<string>();



        public ConnectedPage(IDevice d1=null, IDevice d2=null, IDevice d3=null, IDevice d4=null, IDevice d5=null)
        {
            InitializeComponent();

            Device.BeginInvokeOnMainThread(() =>
            {
                Temp1_1.ItemsSource = d1_temp1; Temp1_2.ItemsSource = d1_temp2; Temp1_3.ItemsSource = d1_temp3;
                Temp2_1.ItemsSource = d2_temp1; Temp2_2.ItemsSource = d2_temp2; Temp2_3.ItemsSource = d2_temp3;
                Temp3_1.ItemsSource = d3_temp1; Temp3_2.ItemsSource = d3_temp2; Temp3_3.ItemsSource = d3_temp3;
                Temp4_1.ItemsSource = d4_temp1; Temp4_2.ItemsSource = d4_temp2; Temp4_3.ItemsSource = d4_temp3;
                Temp5_1.ItemsSource = d5_temp1; Temp5_2.ItemsSource = d5_temp2; Temp5_3.ItemsSource = d5_temp3;
            });

            for (int i = 0; i < 5; i++)
            {
                IDevice currentDevice = null;
                Label currentFiberLabel = null;
                BoxView currentBoxView = null;
                ObservableCollection<string> currentTemp1 = null;
                ObservableCollection<string> currentTemp2 = null;
                ObservableCollection<string> currentTemp3 = null;

                switch (i)
                {
                    case 0:
                        currentDevice = d1;
                        currentFiberLabel = Fiber1;
                        currentBoxView = Box1;
                        currentTemp1 = d1_temp1;
                        currentTemp2 = d1_temp2;
                        currentTemp3 = d1_temp3;
                        break;
                    case 1:
                        currentDevice = d2;
                        currentFiberLabel = Fiber2;
                        currentBoxView = Box2;
                        currentTemp1 = d2_temp1;
                        currentTemp2 = d2_temp2;
                        currentTemp3 = d2_temp3;
                        break;
                    case 2:
                        currentDevice = d3;
                        currentFiberLabel = Fiber3;
                        currentBoxView = Box3;
                        currentTemp1 = d3_temp1;
                        currentTemp2 = d3_temp2;
                        currentTemp3 = d3_temp3;
                        break;
                    case 3:
                        currentDevice = d4;
                        currentFiberLabel = Fiber4;
                        currentBoxView = Box4;
                        currentTemp1 = d4_temp1;
                        currentTemp2 = d4_temp2;
                        currentTemp3 = d4_temp3;
                        break;
                    case 4:
                        currentDevice = d5;
                        currentFiberLabel = Fiber5;
                        currentBoxView = Box5;
                        currentTemp1 = d5_temp1;
                        currentTemp2 = d5_temp2;
                        currentTemp3 = d5_temp3;
                        break;
                    default:
                        break;
                }

                if (currentDevice != null)
                {
                    currentFiberLabel.Text = currentDevice.Name;
                    currentBoxView.Color = Color.FromHex("#FFFFFF");
                    SetupDevices(i);
                    currentTemp1.Add("--");
                    currentTemp2.Add("--");
                    currentTemp3.Add("--");
                }
                else
                {
                    currentFiberLabel.Text = "Not connected";
                    currentBoxView.Color = Color.FromHex("#808080");
                    currentTemp1.Add("N/A");
                    currentTemp2.Add("N/A");
                    currentTemp3.Add("N/A");
                }
            }
        }
        

        public async void SetupDevices(int index)
        {
            IReadOnlyList<IService> s_;

            switch (index)
            {
                case 0:
                    if (device1 == null) break;
                    s_ = await device1.GetServicesAsync();
                    for (int i = 0; i < s_.Count; i++)
                        switch (i)
                        {
                            case 0:
                                d1s1.Add(s_[i]);
                                IReadOnlyList<ICharacteristic> c1 = await s_[i].GetCharacteristicsAsync();
                                foreach (var c_ in c1) { d1c1.Add(c_); }
                                break;
                            case 1:
                                d1s2.Add(s_[i]);
                                IReadOnlyList<ICharacteristic> c2 = await s_[i].GetCharacteristicsAsync();
                                foreach (var c_ in c2) { d1c2.Add(c_); }
                                break;
                            default:
                                break;
                        }
                    break;
                case 1:
                    if (device2 == null) break;
                    s_ = await device2.GetServicesAsync();
                    for (int i = 0; i < s_.Count; i++)
                        switch (i)
                        {
                            case 0:
                                d2s1.Add(s_[i]);
                                IReadOnlyList<ICharacteristic> c1 = await s_[i].GetCharacteristicsAsync();
                                foreach (var c_ in c1) { d2c1.Add(c_); }
                                break;
                            case 1:
                                d2s2.Add(s_[i]);
                                IReadOnlyList<ICharacteristic> c2 = await s_[i].GetCharacteristicsAsync();
                                foreach (var c_ in c2) { d2c2.Add(c_); }
                                break;
                            default:
                                break;
                        }
                    break;
                case 2:
                    if (device3 == null) break;
                    s_ = await device3.GetServicesAsync();
                    for (int i = 0; i < s_.Count; i++)
                        switch (i)
                        {
                            case 0:
                                d3s1.Add(s_[i]);
                                IReadOnlyList<ICharacteristic> c1 = await s_[i].GetCharacteristicsAsync();
                                foreach (var c_ in c1) { d3c1.Add(c_); }
                                break;
                            case 1:
                                d3s2.Add(s_[i]);
                                IReadOnlyList<ICharacteristic> c2 = await s_[i].GetCharacteristicsAsync();
                                foreach (var c_ in c2) { d3c2.Add(c_); }
                                break;
                            default:
                                break;
                        }
                    break;
                case 3:
                    if (device4 == null) break;
                    s_ = await device4.GetServicesAsync();
                    for (int i = 0; i < s_.Count; i++)
                        switch (i)
                        {
                            case 0:
                                d4s1.Add(s_[i]);
                                IReadOnlyList<ICharacteristic> c1 = await s_[i].GetCharacteristicsAsync();
                                foreach (var c_ in c1) { d4c1.Add(c_); }
                                break;
                            case 1:
                                d4s2.Add(s_[i]);
                                IReadOnlyList<ICharacteristic> c2 = await s_[i].GetCharacteristicsAsync();
                                foreach (var c_ in c2) { d4c2.Add(c_); }
                                break;
                            default:
                                break;
                        }
                    break;
                case 4:
                    if (device5 == null) break;
                    s_ = await device5.GetServicesAsync();
                    for (int i = 0; i < s_.Count; i++)
                        switch (i)
                        {
                            case 0:
                                d5s1.Add(s_[i]);
                                IReadOnlyList<ICharacteristic> c1 = await s_[i].GetCharacteristicsAsync();
                                foreach (var c_ in c1) { d5c1.Add(c_); }
                                break;
                            case 1:
                                d5s2.Add(s_[i]);
                                IReadOnlyList<ICharacteristic> c2 = await s_[i].GetCharacteristicsAsync();
                                foreach (var c_ in c2) { d5c2.Add(c_); }
                                break;
                            default:
                                break;
                        }
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

            // Write the epoch time to all connected devices
            if (device1 != null)
            {
                int error = await d1c2[0].WriteAsync(epoch_array);
                error = await d1c2[0].WriteAsync(new byte[] { 0x0C });
            }
            if (device2 != null)
            {
                int error = await d2c2[0].WriteAsync(epoch_array);
                error = await d2c2[0].WriteAsync(new byte[] { 0x0C });
            }
            if (device3 != null)
            {
                int error = await d3c2[0].WriteAsync(epoch_array);
                error = await d3c2[0].WriteAsync(new byte[] { 0x0C });
            }
            if (device4 != null)
            {
                int error = await d4c2[0].WriteAsync(epoch_array);
                error = await d4c2[0].WriteAsync(new byte[] { 0x0C });
            }
            if (device5 != null)
            {
                int error = await d5c2[0].WriteAsync(epoch_array);
                error = await d5c2[0].WriteAsync(new byte[] { 0x0C });
            }

            // Begin the collection process for all connected devices
            // error = await d1c2[0].WriteAsync(new byte[] { 0x0C });
            // error = await d2c2[0].WriteAsync(new byte[] { 0x0C });
            // error = await d3c2[0].WriteAsync(new byte[] { 0x0C });
            // error = await d4c2[0].WriteAsync(new byte[] { 0x0C });
            // error = await d5c2[0].WriteAsync(new byte[] { 0x0C });

            // int error = await Char2[0].WriteAsync( epoch_array );
            // Task.Delay(1500).Wait();

            // error = await Char2[0].WriteAsync(new byte[] { 0x0C });
            // Task.Delay(1500).Wait();

            // while (true)
            // {
            //     int error_ = await CollectionCommand();
            //     if (error_ != 0)
            //         continue;
            // }
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
            // bool answer = await DisplayAlert(
            //     "Be careful!", 
            //     "Are you sure you want to erase the SPI flash?\n\nThe chip erase takes ~30 seconds.\nWait for the buttons to change color.", 
            //     "Yes", "No"
            // );

            string action = await DisplayActionSheet(
                "Which device would you like to erase?",
                "Cancel",
                null,
                Fiber1.Text, Fiber2.Text, Fiber3.Text, Fiber4.Text, Fiber5.Text
            );

            switch (action)
            {
                case "Cancel":
                    break;
                case "Not connected":
                    await DisplayAlert("Error", "No device connected.", "OK");
                    break;
                case "Fiber 1":
                    await d1c2[0].WriteAsync(new byte[] { 0xCE });
                    break;
                case "Fiber 2":
                    await d2c2[0].WriteAsync(new byte[] { 0xCE });
                    break;
                case "Fiber 3":
                    await d3c2[0].WriteAsync(new byte[] { 0xCE });
                    break;
                case "Fiber 4":
                    await d4c2[0].WriteAsync(new byte[] { 0xCE });
                    break;
                case "Fiber 5":
                    await d5c2[0].WriteAsync(new byte[] { 0xCE });
                    break;
                default:
                    break;
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
