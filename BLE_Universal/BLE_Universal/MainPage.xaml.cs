using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Essentials;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;


namespace BLE_Universal
{
    public partial class MainPage : ContentPage
    {
        // Plugin.BLE Documentation for Xamarin Forms package
        // https://github.com/dotnet-bluetooth-le/dotnet-bluetooth-le
        //---------------------------------------------------------------------------------------
        public IAdapter adapter;
        public IBluetoothLE bluetoothBLE;
        public ObservableCollection<IDevice> list;
        public ObservableCollection<string> namelist;
        public IDevice device1, device2, device3, device4, device5;
        public IDevice temp_device;
        Popup popup;
        int SELECTED;
        public Dictionary<char, float> S = new Dictionary<char, float>() { { '0', 1.0f }, { '1', -1.0f } };

        bool IS_COLLECTION_RUNNING;

        public Dictionary<string, string> BLE_MAP = new Dictionary<string, string>
        {
            {"IFM_FIBER_51", "1A. Subject 1 Shirt 1"},
            {"IFM_FIBER_52", "1B. Subject 2 Shirt 2"},
            {"IFM_FIBER_57", "1B. Subject 16 Shirt 3"},

            {"IFM_FIBER_54", "2B. Subject 5 Shirt 5"},
            {"IFM_FIBER_55", "1A. Subject 7 Shirt 6"},
            {"IFM_FIBER_61", "2B. Subject 6 Shirt 7"},

            {"IFM_FIBER_56", "1B. Subject 22 Shirt 8"},
            {"IFM_FIBER_71", "2A. Subject 24 Shirt 9"},
            {"IFM_FIBER_67", "1B. Subject 10 Shirt 10"},

            {"IFM_FIBER_76", "1A. Subject 19 Shirt 13"},
            {"IFM_FIBER_63", "2B. Subject 14 Shirt 14"},
            {"IFM_FIBER_64", "1A. Subject 15 Shirt 15"},

            {"IFM_FIBER_72", "2A. Subject 17 Shirt 17"},
            {"IFM_FIBER_99", "2B. Subject 18 Shirt 18"},
            {"IFM_FIBER_98", "2B. Subject 25 Shirt 19"},

            {"IFM_FIBER_75",  "1A. Subject 20 Shirt 20"},
            {"IFM_FIBER_102", "1B. Subject 21 Shirt 21"},
            {"IFM_FIBER_103", "2A. Subject 11 Shirt 22"},

            {"IFM_FIBER_104", "2A. Subject 23 Shirt 23"},

            {"IFM_FIBER_53", "2A. Subject 4 Shirt 4"},
        };
        
        //---------------------------------------------------------------------------------------

        CancellationTokenSource cancelsource1; CancellationToken canceltoken1;
        CancellationTokenSource cancelsource2; CancellationToken canceltoken2;
        CancellationTokenSource cancelsource3; CancellationToken canceltoken3;
        CancellationTokenSource cancelsource4; CancellationToken canceltoken4;
        CancellationTokenSource cancelsource5; CancellationToken canceltoken5;
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

        // ***
        private Dictionary<string, ObservableCollection<ObservablePoint>> _tags1;
        public ObservableCollection<ISeries> Series1 { get; set; }

        // public event PropertyChangedEventHandler PropertyChanged;
        // protected virtual void OnPropertyChanged(string propertyName = null)
        // {
        //     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        // }

        // ObservableCollection<ISeries> _Series1;
        // public ObservableCollection<ISeries> Series1
        // {
        //     get { return this._Series1;}
        //     set
        //     {
        //         this._Series1 = value;
        //         // OnPropertyChanged("Series1");
        //         // RaisePropertyChanged("Series1");
        //     }
        // }

        public Axis[] X_axis { get; set; } = {
            new Axis
            {
                Name = "Seconds Elapsed",
                TextSize=14,
            }
        };

        public Axis[] Y_axis { get; set; } = {
            new Axis
            {
                Name = "Temperature (°C)",
                TextSize=14,
                MinLimit=-5,
                MaxLimit=30,
            }
        };

        public DateTime START; // To calculate seconds elapsed in AddOrUpdateTagData()

        //***********************************************************************************************************


        // CONSTRUCTOR: Initialize the MainPage
        public MainPage()
        {
            InitializeComponent();
            adapter = CrossBluetoothLE.Current.Adapter;
            bluetoothBLE = CrossBluetoothLE.Current;
            SELECTED = 0;

            list = new ObservableCollection<IDevice>();            // Set up list of devices
            namelist = new ObservableCollection<string>();         // Set up list of device names

            adapter.DeviceDiscovered += OnDeviceDiscovered;        // Set up event handlers
            adapter.DeviceConnectionLost += OnDeviceDisconnected;
            
            Device.BeginInvokeOnMainThread(() =>
            {
                startimage.Source = new FileImageSource { File = "start.png" };
                eraseimage.Source = new FileImageSource { File = "erase.png" };
                Shirt1.Source = new FileImageSource { File = "Man30.png" };
                Shirt2.Source = new FileImageSource { File = "Man30.png" };
                Shirt3.Source = new FileImageSource { File = "Man30.png" };
                // Shirt4.Source = new FileImageSource { File = "Man30.png" };
                // Shirt5.Source = new FileImageSource { File = "Man30.png" };
                Fiber1.Text = "Not Connected"; Fiber1.TextColor = Color.FromHex("#000000");
                Fiber2.Text = "Not Connected"; Fiber2.TextColor = Color.FromHex("#000000");
                Fiber3.Text = "Not Connected"; Fiber3.TextColor = Color.FromHex("#000000");
                // Fiber4.Text = "Not Connected"; Fiber4.TextColor = Color.FromHex("#000000");
                // Fiber5.Text = "Not Connected"; Fiber5.TextColor = Color.FromHex("#000000");
                Temp1_1.ItemsSource = d1_temp1; Temp1_2.ItemsSource = d1_temp2; Temp1_3.ItemsSource = d1_temp3;
                Temp2_1.ItemsSource = d2_temp1; Temp2_2.ItemsSource = d2_temp2; Temp2_3.ItemsSource = d2_temp3;
                Temp3_1.ItemsSource = d3_temp1; Temp3_2.ItemsSource = d3_temp2; Temp3_3.ItemsSource = d3_temp3;
                // Temp4_1.ItemsSource = d4_temp1; Temp4_2.ItemsSource = d4_temp2; Temp4_3.ItemsSource = d4_temp3;
                // Temp5_1.ItemsSource = d5_temp1; Temp5_2.ItemsSource = d5_temp2; Temp5_3.ItemsSource = d5_temp3;
                StartColor.Color = Color.FromHex("#AEAEAE");
            });

            IS_COLLECTION_RUNNING = false;

            d1_temp1.Add("--"); d1_temp2.Add("--"); d1_temp3.Add("--");
            d2_temp1.Add("--"); d2_temp2.Add("--"); d2_temp3.Add("--");
            d3_temp1.Add("--"); d3_temp2.Add("--"); d3_temp3.Add("--");
            // d4_temp1.Add("--"); d4_temp2.Add("--"); d4_temp3.Add("--");
            // d5_temp1.Add("--"); d5_temp2.Add("--"); d5_temp3.Add("--");

            // ****************
            // Initialize LiveCharts Series and Dictionaries
            _tags1 = new Dictionary<string, ObservableCollection<ObservablePoint>>
            {
                { "Forearm",  new ObservableCollection<ObservablePoint>{ } },
                { "Underarm", new ObservableCollection<ObservablePoint>{ } },
                { "Waist",    new ObservableCollection<ObservablePoint>{ } },
            };

            Series1 = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservablePoint> { Name="Forearm",  Values=_tags1["Forearm"],  Fill=null, GeometrySize=3, },
                new LineSeries<ObservablePoint> { Name="Underarm", Values=_tags1["Underarm"], Fill=null, GeometrySize=3, },
                new LineSeries<ObservablePoint> { Name="Waist",    Values=_tags1["Waist"],    Fill=null, GeometrySize=3, },
            };

            _tags1["Forearm"].Add(new ObservablePoint { X = 0.0, Y = 15.0 });

            START = DateTime.Now;
            // ****************
        }


        // HANDLER: Handle the scanning of devices
        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _tags1["Underarm"].Add(new ObservablePoint { X = 0.0, Y = 15.0 });

                if ((!list.Contains(args.Device)) && (args.Device.Name != null) && args.Device.Name.Contains("IFM"))
                {
                    list.Add(args.Device);
                    
                    string dev_name = BLE_MAP.ContainsKey(args.Device.Name) ? BLE_MAP[args.Device.Name] : args.Device.Name;

                    if (!namelist.Contains(dev_name))
                        namelist.Add(dev_name);
                }
            });
        }


        // HANDLER: Handle the disconnection of devices
        private void OnDeviceDisconnected(object sender, DeviceEventArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ClearDevice(args.Device);
            });
        }


        // HELPER FUNCTION: Adjust scanning mode and scans for specific timeout
        private async void SearchDevices(object sender, EventArgs e)
        {
            list.Clear();
            adapter.ScanTimeout = 10000;
            adapter.ScanMode = ScanMode.LowLatency; // Previously ScanMode.Balanced before 1.8.22
            await adapter.StartScanningForDevicesAsync();
        }


        // BUTTON: Plot Data
        private async void PlotData(object sender, EventArgs e)
        {
            // Get button clicked so we don't need five button functions
            // Button clicked = (Button)sender;
            // if      (clicked==Graph1) SELECTED = 1;
            // else if (clicked==Button2) SELECTED = 2;
            // else if (clicked==Button3) SELECTED = 3;
            // else if (clicked==Button4) SELECTED = 4;
            // else if (clicked==Button5) SELECTED = 5;

            // LiveCharts Cartesian Chart in the Popup

            // popup = new Popup
            // {
            //     Content = new StackLayout
            //     {
            //         Children =
            //         {
            //             new Label   // Header of List
            //             {
            //                 Text = "Plot of Subject 1",
            //                 FontAttributes = FontAttributes.Bold,
            //                 FontSize = 26,
            //                 Margin = new Thickness(0, 10)
            //             },
            //             new CartesianChart<Page, ISeries>
            //             {
            //                 Series = Series1,
            //                 XAxes = x_axis,
            //                 YAxes = y_axis,
            //             },
            //             new CartesianChart<MainPage, ISeries>
            //             {
            //                 Series = Series1,
            //                 XAxes = x_axis,
            //                 YAxes = y_axis,
            //             },
            //         }
            //     },
            //     Size = new Size(600, 600),
            // };
            // var result = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
        }


        // BUTTON: Connect to a device
        private async void ConnectDevice(object sender, EventArgs e)
        {
            list.Clear();
            namelist.Clear();
            var listview = new ListView { ItemsSource = namelist };
            listview.ItemSelected += OnItemSelected;

            // Get button clicked so we don't need five button functions
            Button clicked = (Button)sender;
            if      (clicked==Button1) SELECTED = 1;
            else if (clicked==Button2) SELECTED = 2;
            else if (clicked==Button3) SELECTED = 3;
            // else if (clicked==Button4) SELECTED = 4;
            // else if (clicked==Button5) SELECTED = 5;

            popup = new Popup
            {
                Content = new StackLayout
                {
                    Children =
                    {
                        new Label   // Header of List
                        {
                            Text = "Connect to Subject " + SELECTED.ToString(),
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 26,
                            Margin = new Thickness(0, 10)
                        },
                        listview,  // List of Devices Found
                    }
                },
            };

            SearchDevices(null, null);

            // Show the Popup
            var result = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
        }


        // BUTTON: Attempt connection for associated device slot
        async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Find index of selected name in namelist
            int index = namelist.IndexOf(e.SelectedItem as string);

            // Get the device from the list of devices
            temp_device = list.ElementAt(index);

            if (SELECTED==1 && device1==null)
            {
                device1 = temp_device;
                await Setup_Device(0, device1);
            }
            else if (SELECTED==2 && device2==null)
            {
                device2 = temp_device;
                await Setup_Device(1, device2);
            }
            else if (SELECTED==3 && device3==null)
            {
                device3 = temp_device;
                await Setup_Device(2, device3);
            }
            else if (SELECTED==4 && device4==null)
            {
                device4 = temp_device;
                await Setup_Device(3, device4);
            }
            else if (SELECTED==5 && device5==null)
            {
                device5 = temp_device;
                await Setup_Device(4, device5);
            }
            else
            {
                await DisplayAlert("Error", "All devices are connected.", "OK");
                return;
            }
        }


        // HELPER FUNCTION: Set up the device for connection
        async Task<int> Setup_Device(int index, IDevice d)
        {
            await adapter.StopScanningForDevicesAsync(); // Stop Scanner

            if (index==0)
            {
                try
                {
                    await adapter.ConnectToDeviceAsync(d);
                    Fiber1.Text = BLE_MAP.ContainsKey(d.Name) ? BLE_MAP[d.Name] : d.Name;
                    Fiber1.TextColor = Color.FromHex("#004A10");
                    Shirt1.Source = new FileImageSource { File = "Green30.jpeg" };
                    cancelsource1 = new CancellationTokenSource(); canceltoken1 = cancelsource1.Token;
                    ServicesAndCharacteristics(0);
                }
                catch (Exception)
                {
                    Fiber1.Text = "Error! Please retry.";
                    Fiber1.TextColor = Color.FromHex("#E60008");
                    Shirt1.Source = new FileImageSource { File = "Red.png" };
                    popup.Dismiss(null);
                    return 1;
                }
            }
            else if (index==1)
            {
                try
                {
                    await adapter.ConnectToDeviceAsync(d);
                    Fiber2.Text = BLE_MAP.ContainsKey(d.Name) ? BLE_MAP[d.Name] : d.Name;
                    Fiber2.TextColor = Color.FromHex("#004A10");
                    Shirt2.Source = new FileImageSource { File = "Green30.jpeg" };
                    cancelsource2 = new CancellationTokenSource(); canceltoken2 = cancelsource2.Token;
                    ServicesAndCharacteristics(1);
                }
                catch (Exception)
                {
                    Fiber2.Text = "Error! Please retry.";
                    Fiber2.TextColor = Color.FromHex("#E60008");
                    Shirt2.Source = new FileImageSource { File = "Red.png" };
                    popup.Dismiss(null);
                    return 1;
                }
            }
            else if (index==2)
            {
                try
                {
                    await adapter.ConnectToDeviceAsync(d);
                    Fiber3.Text = BLE_MAP.ContainsKey(d.Name) ? BLE_MAP[d.Name] : d.Name;
                    Fiber3.TextColor = Color.FromHex("#004A10");
                    Shirt3.Source = new FileImageSource { File = "Green30.jpeg" };
                    cancelsource3 = new CancellationTokenSource(); canceltoken3 = cancelsource3.Token;
                    ServicesAndCharacteristics(2);

                }
                catch (Exception)
                {
                    Fiber3.Text = "Error! Please retry.";
                    Fiber3.TextColor = Color.FromHex("#E60008");
                    Shirt3.Source = new FileImageSource { File = "Red.png" };
                    popup.Dismiss(null);
                    return 1;
                }
            }
            popup.Dismiss(null);
            return 0;
        }


        // HELPER FUNCTION: Set up the services and characteristics specific device.
        public async void ServicesAndCharacteristics(int index)
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
            }
        }


        // BUTTON: Write Command to BLE Device to trigger MCU collection of accel. data
        async void OnStartClicked(object sender, EventArgs args)
        {
            // Write Epoch time in exactly 8 bytes of space, with 0x1D command on the head
            // example: { 0x1D, 0x00, 0x00, 0x00, 0x00, 0x5F, 0x5E, 0x5D, 0x5C }
            // epoch_time = (UInt64)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            // byte[] epoch_bytes = BitConverter.GetBytes(epoch_time);

            byte[] epoch_array = new byte[ 8 ];
            epoch_array[0] = 0x1D;  // Command
            epoch_array[1] = 0x07;  // 1st byte of 2024
            epoch_array[2] = 0xE8;  // 2nd byte of 2024
            epoch_array[3] = (byte)DateTime.Now.Month;
            epoch_array[4] = (byte)DateTime.Now.Day;
            epoch_array[5] = (byte)DateTime.Now.Hour;
            epoch_array[6] = (byte)DateTime.Now.Minute;
            epoch_array[7] = (byte)DateTime.Now.Second;
            // epoch_array[0] = 0x1D;   // Command
            // epoch_array[1] = 0x12;   // Test bytes currently
            // epoch_array[2] = 0x34;
            // epoch_array[3] = 0x56;
            // epoch_array[4] = 0x78;
            // epoch_array[5] = 0x9A;

            List<string> validOptions = new List<string>
            {
                "START",
                "RESUME",
            };

            string str1 = device1?.Name;
            string str2 = device2?.Name;
            string str3 = device3?.Name;
            // string str4 = device4?.Name;
            // string str5 = device5?.Name;

            if (validOptions.Count == 0)
            {
                await DisplayAlert("Error!", "No devices connected.", "OK");
                return;
            }

            // Setup a selection of devices, returns the selected device name
            string action = await DisplayActionSheet(
                "Select a device to start data collection", "Cancel", null,
                validOptions.ToArray()
            );

            
            if (action==str1)
            {
                int error = await d1c2[0].WriteAsync(epoch_array);
                Task.Delay(1500).Wait();
                error = await d1c2[0].WriteAsync(new byte[] { 0x0C });
            }
            else if (action==str2)
            {
                int error = await d2c2[0].WriteAsync(epoch_array);
                Task.Delay(1500).Wait();
                error = await d2c2[0].WriteAsync(new byte[] { 0x0C });
            }
            else if (action==str3)
            {
                int error = await d3c2[0].WriteAsync(epoch_array);
                Task.Delay(1500).Wait();
                error = await d3c2[0].WriteAsync(new byte[] { 0x0C });
            }

            else if (action=="RESUME")
            {
                if (!IS_COLLECTION_RUNNING)
                {
                    await CollectLoop();
                }
            }

            else return;

            Task.Delay(600).Wait();
            if (!IS_COLLECTION_RUNNING)
            {
                int error = await CollectLoop();
                if (error != 0)
                {
                    StartColor.Color = Color.FromHex("#FF2432");
                    IS_COLLECTION_RUNNING = false;
                }
            }

            return;
        }


        public async Task<int> CollectLoop()
        {
            IS_COLLECTION_RUNNING = true;
                StartColor.Color = Color.FromHex("#1CFF59");
                await Task.Run(async () =>
                {
                    while (IS_COLLECTION_RUNNING)
                    {
                        await Task.Delay(1000);
                        try
                        {
                            int error_ = await CollectionCommand();
                            if (error_ != 0)
                            {
                                StartColor.Color = Color.FromHex("#FF2432");
                                IS_COLLECTION_RUNNING = false;
                            }
                        }
                        catch (Exception)
                        {
                            StartColor.Color = Color.FromHex("#FF2432");
                            IS_COLLECTION_RUNNING = false;
                            break;
                        }
                    }
                });
            return -1;
        }


        // HELPER FUNCTION: Holds the loop for collecting data over BLE.
        public async Task<int> CollectionCommand()
        {
            try
            {
                if (device1!=null) await ProcessDeviceData(device1, d1c2, d1_temp1, d1_temp2, d1_temp3);
                if (device2!=null) await ProcessDeviceData(device2, d2c2, d2_temp1, d2_temp2, d2_temp3);
                if (device3!=null) await ProcessDeviceData(device3, d3c2, d3_temp1, d3_temp2, d3_temp3);
            }
            catch (Exception)
            {
                IS_COLLECTION_RUNNING = false;
                return 1;
            }
            return 0;
        }


        // HELPER FUNCTION: Read and update the characteristics of the associated device
        async Task ProcessDeviceData(
            IDevice device, 
            ObservableCollection<ICharacteristic> connection, 
            ObservableCollection<string> temp1,
            ObservableCollection<string> temp2,
            ObservableCollection<string> temp3)
        {
            if (device==null)         return;  // Check that device exists
            if (connection.Count < 3) return;  // Check that index 2 exists

            (byte[], int) bytes;
            try
            {
                bytes = await connection[2]?.ReadAsync();
            }
            catch (Exception)
            {
                ClearDevice(device);
                return;
            }

            // Exit if bytes or bytes.Item1 is null or insufficiently initialized
            if (bytes.Item1 == null || bytes.Item1.Length < 4)
                return;

            // Convert the hex values into strings
            string t1 = CombineBytesToBinaryString( bytes.Item1[0], bytes.Item1[1] );
            string t2 = CombineBytesToBinaryString( bytes.Item1[2], bytes.Item1[3] );

            // Convert the strings into float16 values
            float value1 = ParseFloat16(t1);
            float value2 = ParseFloat16(t2);

            // Convert the float16 values into rounded strings
            string temp1_string = Math.Round(value1, 1).ToString() + "°";
            string temp2_string = Math.Round(value2, 1).ToString() + "°";

            var TimeDiff = (DateTime.Now - START).TotalSeconds;

            if (bytes.Item1.Length==6)
            {
                string t3 = CombineBytesToBinaryString( bytes.Item1[4], bytes.Item1[5] );
                float value3 = ParseFloat16(t3);
                string temp3_string = Math.Round(value3, 1).ToString() + "°";
                if (!(temp3.ElementAt(0)==temp3_string))
                {
                    temp3.RemoveAt(0);
                    temp3.Add(temp3_string);
                }

                // TEMPORARY
                _tags1["Waist"].Add(new ObservablePoint { X = TimeDiff, Y = value3 });
            }

            if (!(temp1.ElementAt(0)==temp1_string))
            {
                temp1.RemoveAt(0);
                temp1.Add(temp1_string);
            }
            if (!(temp2.ElementAt(0)==temp2_string))
            {
                temp2.RemoveAt(0);
                temp2.Add(temp2_string);
            }

            // Add to LiveCharts data for device1
            if (device == device1)
            {
                _tags1["Forearm"].Add(new ObservablePoint { X = TimeDiff, Y = value1 });
                _tags1["Underarm"].Add(new ObservablePoint { X = TimeDiff, Y = value2 });
            }
            
            Task.Delay(1200).Wait();
        }


        // HELPER: Remove a device from the list of connected devices if misc. exception
        private void ClearDevice(IDevice device)
        {
            if (device == device1)
            {
                cancelsource1.Cancel();
                Fiber1.TextColor = Color.FromHex("#707070");
                Shirt1.Source = new FileImageSource { File = "Man30.png" };
                device1 = null;
                d1s1.Clear(); d1s2.Clear(); d1c1.Clear(); d1c2.Clear();
            }
            else if (device == device2)
            {
                cancelsource2.Cancel();
                Fiber2.TextColor = Color.FromHex("#707070");
                Shirt2.Source = new FileImageSource { File = "Man30.png" };
                device2 = null;
                d2s1.Clear(); d2s2.Clear(); d2c1.Clear(); d2c2.Clear();
            }
            else if (device == device3)
            {
                cancelsource3.Cancel();
                Fiber3.TextColor = Color.FromHex("#707070");
                Shirt3.Source = new FileImageSource { File = "Man30.png" };
                device3 = null;
                d3s1.Clear(); d3s2.Clear(); d3c1.Clear(); d3c2.Clear();
            }
        }


        // BUTTON: Perform Chip Erase (0xCE) with device selector.
        async void OnChipErase(object sender, EventArgs args)
        {            
            List<string> validOptions = new List<string>();

            string str1 = device1?.Name;
            string str2 = device2?.Name;
            string str3 = device3?.Name;

            if (str1 != null) validOptions.Add(str1);
            if (str2 != null) validOptions.Add(str2);
            if (str3 != null) validOptions.Add(str3);

            if (validOptions.Count == 0)
            {
                await DisplayAlert("Error!", "No devices connected.", "OK");
                return;
            }

            string action = await DisplayActionSheet(
                "Select a device to chip erase", "Cancel", null,
                validOptions.ToArray()
            );

            if (action == str1)
            {
                if (device1 == null) await DisplayAlert("Error!", "No device connected.", "OK");
                else await d1c2[0].WriteAsync(new byte[] { 0xCE });
            }
            else if (action == str2)
            {
                if (device2 == null) await DisplayAlert("Error!", "No device connected.", "OK");
                else await d2c2[0].WriteAsync(new byte[] { 0xCE });
            }
            else if (action == str3)
            {
                if (device3 == null) await DisplayAlert("Error!", "No device connected.", "OK");
                else await d3c2[0].WriteAsync(new byte[] { 0xCE });
            }
            else
                return; 
        }


        // HELPER FUNCTION: Combine two bytes into a single binary string.
        static string CombineBytesToBinaryString(byte byte1, byte byte2)
        {
            return Convert.ToString(byte1, 2).PadLeft(8, '0') + Convert.ToString(byte2, 2).PadLeft(8, '0');
        }


        // HELPER FUNCTION: Convert a binary string to a float, written by ChatGPT 3.5, converted from my original Python code.
        float ParseFloat16(string binfloat)
        {
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

            return 0.0f;  // This line is unreachable, but C# requires a return statement in all code paths
        }

    }

    
    // internal class CartesianChart<T1, T2> : View
    // {
    //     public ObservableCollection<ISeries> Series { get; set; }
    //     public Axis[] XAxes { get; set; }
    //     public Axis[] YAxes { get; set; }
    // }
}
