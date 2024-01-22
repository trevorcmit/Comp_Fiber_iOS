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
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;


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

        public FileResult pick_result;


        // LiveCharts Section ----------------------------------
        public DateTime START; // To calculate seconds elapsed in AddOrUpdateTagData()

        private ObservableCollection<ISeries> _Series_;
        public ObservableCollection<ISeries> Series_
        {
            get { return this._Series_; }
            set
            {
                this._Series_ = value;
                // OnPropertyChanged("Series_");
                OnPropertyChanged(nameof(Series_));
            }
        }

        // private ObservableCollection<ObservablePoint> _temp1;
        // public ObservableCollection<ObservablePoint> Temp1
        // {
        //     get => _temp1;
        //     set
        //     {
        //         _temp1 = value;
        //         OnPropertyChanged(nameof(Temp1));
        //     }
        // }

        // private ObservableCollection<ObservablePoint> _temp2;
        // public ObservableCollection<ObservablePoint> Temp2
        // {
        //     get => _temp2;
        //     set
        //     {
        //         _temp2 = value;
        //         OnPropertyChanged(nameof(Temp2));
        //     }
        // }

        private string _temp_1;
        public string Temp_1
        {
            get => _temp_1;
            set
            {
                _temp_1 = value;
                OnPropertyChanged(nameof(Temp_1));
            }
        }

        private string _temp_2;
        public string Temp_2
        {
            get => _temp_2;
            set
            {
                _temp_2 = value;
                OnPropertyChanged(nameof(Temp_2));
            }
        }

        public Axis[] X_axis { get; set; } = 
        {
            new Axis
            {
                Name = "Seconds Elapsed",
                TextSize=19,
            }
        };

        public Axis[] Y_axis { get; set; } = 
        {
            new Axis
            {
                Name = "Temperature (°C)",
                TextSize=19,
                MinLimit=0,
                MaxLimit=30,
            }
        };
        //------------------------------------------------------


        public ConnectedPage(IDevice d)
        {
            InitializeComponent();
            device = d;

            Service1.ItemsSource = Serv1;
            Service2.ItemsSource = Serv2;
            Service3.ItemsSource = Serv3;
            Service4.ItemsSource = Serv4;

            //*********************************
            // Setup chart (LiveCharts CartesianChart) in constructor
            // Device.BeginInvokeOnMainThread(() =>
            // {
            //     chart.Series = Series_;
            //     chart.XAxes = X_axis;
            //     chart.YAxes = Y_axis;
            // });

            // _temp1 = new ObservableCollection<ObservablePoint>();
            // _temp2 = new ObservableCollection<ObservablePoint>();

            // Series_ = new ObservableCollection<ISeries>
            // {
            //     new LineSeries<ObservablePoint> { Name = "Temp_1", Values = Temp1, Fill = null, GeometrySize=3, },
            //     new LineSeries<ObservablePoint> { Name = "Temp_2", Values = Temp2, Fill = null, GeometrySize=3, },
            // };
            //**********************************

            Device.BeginInvokeOnMainThread(() =>
            {
                // set binding content
                // Label1.BindingContext = this;
                // Label2.BindingContext = this;
                // Label1.Text = Temp_1;
                // Label2.Text = Temp_2;

                Accel_Data1.ItemsSource = ACCEL_DATA1;
                Accel_Data2.ItemsSource = ACCEL_DATA2;
            });

            ACCEL_DATA1.Add("N/A");
            ACCEL_DATA2.Add("N/A");

            // Temp_1 = "N/A";
            // Temp_2 = "N/A";

            // Accel_Data1.ItemsSource = ACCEL_DATA1;
            // Accel_Data2.ItemsSource = ACCEL_DATA2;
            // ACCEL_DATA1.Add("N/A");
            // ACCEL_DATA2.Add("N/A");

            GetPermissions();
            SetupDevice();
        }


        public async void GetPermissions()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                PermissionStatus reqStatus   = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();  
                PermissionStatus checkStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                PermissionStatus readStatus  = await Permissions.RequestAsync<Permissions.StorageRead>();
                PermissionStatus WriteStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();
                connectedDevice.Text = device.Name;
            });  
            // if (DeviceInfo.Platform == DevicePlatform.Android) // If Android, select file to save data to
            // {
            //     pick_result = await FilePicker.PickAsync();
            // }
        }


        public async void SetupDevice()
        {
            // Discover Services of Connected Device
            IReadOnlyList<IService> s_ = await device.GetServicesAsync();

            for (int i = 0; i < s_.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        Serv1.Add(s_[i]);
                        IReadOnlyList<ICharacteristic> c1 = await s_[i].GetCharacteristicsAsync();
                        foreach (var c_ in c1) { Device.BeginInvokeOnMainThread(() => { Char1.Add(c_); }); }
                        break;
                    case 1:
                        Serv2.Add(s_[i]);
                        IReadOnlyList<ICharacteristic> c2 = await s_[i].GetCharacteristicsAsync();
                        foreach (var c_ in c2) { Device.BeginInvokeOnMainThread(() => { Char2.Add(c_); }); }
                        break;
                    case 2:
                        Serv3.Add(s_[i]);
                        IReadOnlyList<ICharacteristic> c3 = await s_[i].GetCharacteristicsAsync();
                        foreach (var c_ in c3) { Device.BeginInvokeOnMainThread(() => { Char3.Add(c_); }); }
                        break;
                    case 3:
                        Serv4.Add(s_[i]);
                        IReadOnlyList<ICharacteristic> c4 = await s_[i].GetCharacteristicsAsync();
                        foreach (var c_ in c4) { Device.BeginInvokeOnMainThread(() => { Char4.Add(c_); }); }
                        break;
                    default:
                        break;
                }
            }
        }


        // Write Command to BLE Device to trigger MCU collection of accel. data
        async void OnStartClicked(object sender, EventArgs args)
        {
            int error = await Char4[0].WriteAsync(new byte[] { 0x0C });

            START = DateTime.Now; // Setup Start DateTime for X-Axis of LiveChart

            Task.Delay(2000).Wait();

            // Loop to collect temp characteristic updates from BLE device
            while (true)
            {
                int error_ = await CollectionCommand();
                if (error_ != 0)
                    continue;
            }
        }


        async void OnCollectClicked(object sender, EventArgs args)
        {
            // int error = await Char4[0].WriteAsync(new byte[] { 0xCC });
            // Task.Delay(600).Wait();
            // for (int J = 0; J < 11000; J++)
            // {
            //     int error_ = await CollectionCommand();
            //     Counter.Text= J.ToString();
            // }
        }


        private async Task<int> CollectionCommand()
        {
            Task.Delay(1000).Wait();
            (byte[], int) bytes = await Char4[2].ReadAsync();

            // Convert bytes to binary string
            string temp1 = CombineBytesToBinaryString(bytes.Item1[0], bytes.Item1[1]);
            string temp2 = CombineBytesToBinaryString(bytes.Item1[2], bytes.Item1[3]);

            // Convert binary string to float
            double temp1_float = Math.Round( ParseFloat16(temp1), 1);
            double temp2_float = Math.Round( ParseFloat16(temp2), 1);

            string temp1_string = temp1_float.ToString() + "°";
            string temp2_string = temp2_float.ToString() + "°";

            double TimeDiff = (DateTime.Now - START).TotalSeconds;

            if (!(ACCEL_DATA1.ElementAt(0)==temp1_string))
            {
                // Device.BeginInvokeOnMainThread(() =>
                // {
                    ACCEL_DATA1.RemoveAt(0);
                    ACCEL_DATA1.Add(temp1_string);

                // });
            }

            if (!(ACCEL_DATA2.ElementAt(0)==temp2_string))
            {
                // Device.BeginInvokeOnMainThread(() =>
                // {
                    ACCEL_DATA2.RemoveAt(0);
                    ACCEL_DATA2.Add(temp2_string);
                // });
            }

            // ACCEL_DATA1.Add(temp1_string);
            // ACCEL_DATA2.Add(temp2_string);

            // Device.BeginInvokeOnMainThread(() =>
            // {
                // Temp1.Add(new ObservablePoint(TimeDiff, temp1_float));
                // Temp2.Add(new ObservablePoint(TimeDiff, temp2_float));

                // Temp_1 = temp1_string;
                // Temp_2 = temp2_string;
            // });

            return bytes.Item2;
        }


        async void OnChipErase(object sender, EventArgs args)
        {
            int error = await Char4[0].WriteAsync(new byte[] { 0xCE });
        }


        /**************************************************
         * Combine two bytes into a single binary string.
        **************************************************/
        static string CombineBytesToBinaryString(byte byte1, byte byte2)
        {
            string binaryByte1 = Convert.ToString(byte1, 2).PadLeft(8, '0');
            string binaryByte2 = Convert.ToString(byte2, 2).PadLeft(8, '0');

            // Combine the two binary strings
            string combinedBinaryString = binaryByte1 + binaryByte2;

            return combinedBinaryString;
        }


        /****************************************************************************
         * Convert a binary string to a float.
         * Function written by ChatGPT 3.5, converted from my original Python code.
        ****************************************************************************/
        static float ParseFloat16(string binfloat)
        {
            // Map sign bit string to multiplicand
            float[] S = { 1.0f, -1.0f };

            char sign = binfloat[0];
            string exp = binfloat.Substring(1, 5);
            string mantissa = binfloat.Substring(6);

            if (exp == "00000")
            {
                if (mantissa == "0000000000") // Underflow
                {
                    return 0.0f;
                }
                else
                {
                    return S[sign - '0'] * (float)Math.Pow(2, -14) * (Convert.ToInt32(mantissa, 2) * (float)Math.Pow(2, -10));
                }
            }
            else if (exp == "11111") // Overflow case
            {
                if (sign == '0')
                {
                    return float.NaN; // Currently set to NaN, this could be float.PositiveInfinity
                }
                else if (sign == '1')
                {
                    return float.NaN; // Currently set to NaN, this could be float.NegativeInfinity
                }
            }
            else
            {
                float fraction = 1.0f + (Convert.ToInt32(mantissa, 2) * (float)Math.Pow(2, -10));
                return S[sign - '0'] * fraction * (float)Math.Pow(2, Convert.ToInt32(exp, 2) - 15); // 15 is half-precision bias
            }

            // This line is unreachable, but C# requires a return statement in all code paths
            return 0.0f;
        }

    }
}