using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Essentials;

// Added for Plotting Section
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
        //*********************************************************************************
        public IAdapter adapter;
        public IBluetoothLE bluetoothBLE;
        public ObservableCollection<IDevice> list;
        public IDevice device1, device2, device3, device4, device5;
        Popup popup;
        Popup graph;
        int SELECTED;
        public DateTime START;
        UInt64 epoch_time;
        public Dictionary<char, float> S = new Dictionary<char, float>() { { '0', 1.0f }, { '1', -1.0f } };

        // For LiveChartsPlotting
        public Axis[] x_axis { get; set; } = { new Axis { Name="Time Elapsed (s)", TextSize=14 } };
        public Axis[] y_axis { get; set; } = { new Axis { Name="Temperature (°C)", TextSize=14 } };

        public ObservableCollection<ObservablePoint> d1_points = new ObservableCollection<ObservablePoint>();
        public ObservableCollection<ObservablePoint> d2_points = new ObservableCollection<ObservablePoint>();
        public ObservableCollection<ObservablePoint> d3_points = new ObservableCollection<ObservablePoint>();
        public ObservableCollection<ObservablePoint> d4_points = new ObservableCollection<ObservablePoint>();
        public ObservableCollection<ObservablePoint> d5_points = new ObservableCollection<ObservablePoint>();
        public ObservableCollection<ISeries> Series { get; set; }
        //*********************************************************************************

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


        public MainPage()
        {
            InitializeComponent();
            adapter = CrossBluetoothLE.Current.Adapter;
            bluetoothBLE = CrossBluetoothLE.Current;
            SELECTED = 0;

            // Set up list of devices
            list = new ObservableCollection<IDevice>();

            // Set up event handlers
            adapter.DeviceDiscovered += OnDeviceDiscovered;
            adapter.DeviceConnectionLost += OnDeviceDisconnected;
            
            Device.BeginInvokeOnMainThread(() =>
            {
                Shirt1.Source = new FileImageSource { File = "Man30.png" };
                Shirt2.Source = new FileImageSource { File = "Man30.png" };
                Shirt3.Source = new FileImageSource { File = "Man30.png" };
                Shirt4.Source = new FileImageSource { File = "Man30.png" };
                Shirt5.Source = new FileImageSource { File = "Man30.png" };
                Fiber1.Text = "Not Connected"; Fiber1.TextColor = Color.FromHex("#000000");
                Fiber2.Text = "Not Connected"; Fiber2.TextColor = Color.FromHex("#000000");
                Fiber3.Text = "Not Connected"; Fiber3.TextColor = Color.FromHex("#000000");
                Fiber4.Text = "Not Connected"; Fiber4.TextColor = Color.FromHex("#000000");
                Fiber5.Text = "Not Connected"; Fiber5.TextColor = Color.FromHex("#000000");
                Temp1_1.ItemsSource = d1_temp1; Temp1_2.ItemsSource = d1_temp2; Temp1_3.ItemsSource = d1_temp3;
                Temp2_1.ItemsSource = d2_temp1; Temp2_2.ItemsSource = d2_temp2; Temp2_3.ItemsSource = d2_temp3;
                Temp3_1.ItemsSource = d3_temp1; Temp3_2.ItemsSource = d3_temp2; Temp3_3.ItemsSource = d3_temp3;
                Temp4_1.ItemsSource = d4_temp1; Temp4_2.ItemsSource = d4_temp2; Temp4_3.ItemsSource = d4_temp3;
                Temp5_1.ItemsSource = d5_temp1; Temp5_2.ItemsSource = d5_temp2; Temp5_3.ItemsSource = d5_temp3;
            });

            d1_temp1.Add("--"); d1_temp2.Add("--"); d1_temp3.Add("--");
            d2_temp1.Add("--"); d2_temp2.Add("--"); d2_temp3.Add("--");
            d3_temp1.Add("--"); d3_temp2.Add("--"); d3_temp3.Add("--");
            d4_temp1.Add("--"); d4_temp2.Add("--"); d4_temp3.Add("--");
            d5_temp1.Add("--"); d5_temp2.Add("--"); d5_temp3.Add("--");
        }


        // HANDLER: Handle the scanning of devices
        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if ((!list.Contains(args.Device)) && (args.Device.Name != null) && (args.Device.Name.Contains("IFM")))
                    list.Add(args.Device);
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
            adapter.ScanMode = ScanMode.Balanced;
            await adapter.StartScanningForDevicesAsync();
        }


        // BUTTON: Connect to a device
        private async void ConnectDevice(object sender, EventArgs e)
        {
            list.Clear();
            var listview = new ListView { ItemsSource = list };
            listview.ItemSelected += OnItemSelected;

            // Get button clicked so we don't need five button functions
            Button clicked = (Button)sender;
            if      (clicked==Button1) SELECTED = 1;
            else if (clicked==Button2) SELECTED = 2;
            else if (clicked==Button3) SELECTED = 3;
            else if (clicked==Button4) SELECTED = 4;
            else if (clicked==Button5) SELECTED = 5;

            popup = new Popup
            {
                Content = new StackLayout
                {
                    Children =
                    {
                        new Label      // Header of List
                        {
                            Text = "Connect to Subject " + SELECTED.ToString(),
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 26,
                            Margin = new Thickness(0, 10)
                        },
                        listview,     // List of Devices Found
                    }
                }
            };

            SearchDevices(null, null);

            // Show the Popup
            var result = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
        }


        // BUTTON: Attempt connection for associated device slot
        async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (SELECTED==1 && device1==null)
            {
                device1 = e.SelectedItem as IDevice;
                await Setup_Device(0, device1);
            }
            else if (SELECTED==2 && device2==null)
            {
                device2 = e.SelectedItem as IDevice;
                await Setup_Device(1, device2);
            }
            else if (SELECTED==3 && device3==null)
            {
                device3 = e.SelectedItem as IDevice;
                await Setup_Device(2, device3);
            }
            else if (SELECTED==4 && device4==null)
            {
                device4 = e.SelectedItem as IDevice;
                await Setup_Device(3, device4);
            }
            else if (SELECTED==5 && device5==null)
            {
                device5 = e.SelectedItem as IDevice;
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
                    Fiber1.Text = d.Name;
                    Fiber1.TextColor = Color.FromHex("#008F20");
                    Shirt1.Source = new FileImageSource { File = "Green30.jpeg" };
                    ServicesAndCharacteristics(0);
                }
                catch (DeviceConnectionException ex)
                {
                    Fiber1.Text = "Device Connection Error! Please retry.";
                    Fiber1.TextColor = Color.FromHex("#002AD1");
                    Shirt1.Source = new FileImageSource { File = "Blue.jpg" };
                    return -1;
                }
                catch (Exception ex)
                {
                    Fiber1.Text = "Backend error! Please retry.";
                    Fiber1.TextColor = Color.FromHex("#E60008");
                    Shirt1.Source = new FileImageSource { File = "Red.png" };
                    return 1;
                }
            }
            else if (index==1)
            {
                try 
                {
                    await adapter.ConnectToDeviceAsync(d);
                    Fiber2.Text = d.Name;
                    Fiber2.TextColor = Color.FromHex("#008F20");
                    Shirt2.Source = new FileImageSource { File = "Green30.jpeg" };
                    ServicesAndCharacteristics(1);
                }
                catch (DeviceConnectionException ex)
                {
                    Fiber2.Text = "Device Connection Error! Please retry.";
                    Fiber2.TextColor = Color.FromHex("#002AD1");
                    Shirt2.Source = new FileImageSource { File = "Blue.jpg" };
                    return -1;
                }
                catch (Exception ex)
                {
                    Fiber2.Text = "Backend error! Please retry.";
                    Fiber2.TextColor = Color.FromHex("#E60008");
                    Shirt2.Source = new FileImageSource { File = "Red.png" };
                    return 1;
                }
            }
            else if (index==2)
            {
                try 
                {
                    await adapter.ConnectToDeviceAsync(d);
                    Fiber3.Text = d.Name;
                    Fiber3.TextColor = Color.FromHex("#008F20");
                    Shirt3.Source = new FileImageSource { File = "Green30.jpeg" };
                    ServicesAndCharacteristics(2);
                }
                catch (DeviceConnectionException ex)
                {
                    Fiber3.Text = "Device Connection Error! Please retry.";
                    Fiber3.TextColor = Color.FromHex("#002AD1");
                    Shirt3.Source = new FileImageSource { File = "Blue.jpg" };
                    return -1;
                }
                catch (Exception ex)
                {
                    Fiber3.Text = "Backend error! Please retry.";
                    Fiber3.TextColor = Color.FromHex("#E60008");
                    Shirt3.Source = new FileImageSource { File = "Red.png" };
                    return 1;
                }
            }
            else if (index==3)
            {
                try 
                {
                    await adapter.ConnectToDeviceAsync(d);
                    Fiber4.Text = d.Name;
                    Fiber4.TextColor = Color.FromHex("#008F20");
                    Shirt4.Source = new FileImageSource { File = "Green30.jpeg" };
                    ServicesAndCharacteristics(3);
                }
                catch (DeviceConnectionException ex)
                {
                    Fiber4.Text = "Device Connection Error! Please retry.";
                    Fiber4.TextColor = Color.FromHex("#002AD1");
                    Shirt4.Source = new FileImageSource { File = "Blue.jpg" };
                    return -1;
                }
                catch (Exception ex)
                {
                    Fiber4.Text = "Backend error! Please retry.";
                    Fiber4.TextColor = Color.FromHex("#E60008");
                    Shirt4.Source = new FileImageSource { File = "Red.png" };
                    return 1;
                }
            }
            else if (index==4)
            {
                try 
                {
                    await adapter.ConnectToDeviceAsync(d);
                    Fiber5.Text = d.Name;
                    Fiber5.TextColor = Color.FromHex("#008F20");
                    Shirt5.Source = new FileImageSource { File = "Green30.jpeg" };
                    ServicesAndCharacteristics(4);
                }
                catch (DeviceConnectionException ex)
                {
                    Fiber5.Text = "Device Connection Error! Please retry.";
                    Fiber5.TextColor = Color.FromHex("#002AD1");
                    Shirt5.Source = new FileImageSource { File = "Blue.jpg" };
                    return -1;
                }
                catch (Exception ex)
                {
                    Fiber5.Text = "Backend error! Please retry.";
                    Fiber5.TextColor = Color.FromHex("#E60008");
                    Shirt5.Source = new FileImageSource { File = "Red.png" };
                    return 1;
                }
            }

            return 0;
        }


        /***********************************************************
         * Set up the services and characteristics specific device.
        ***********************************************************/
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
            epoch_time = (UInt64)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            byte[] epoch_bytes = BitConverter.GetBytes(epoch_time);
            byte[] epoch_array = new byte[9];
            epoch_bytes.CopyTo(epoch_array, 1);
            epoch_array[0] = 0x1D;

            // Write the epoch time to all connected devices
            if (device1 != null)
            {
                int error = await d1c2[0].WriteAsync(epoch_array);
                Task.Delay(1000).Wait();
                error = await d1c2[0].WriteAsync(new byte[] { 0x0C });
            }
            if (device2 != null)
            {
                int error = await d2c2[0].WriteAsync(epoch_array);
                Task.Delay(1000).Wait();
                error = await d2c2[0].WriteAsync(new byte[] { 0x0C });
            }
            if (device3 != null)
            {
                int error = await d3c2[0].WriteAsync(epoch_array);
                Task.Delay(1000).Wait();
                error = await d3c2[0].WriteAsync(new byte[] { 0x0C });
            }
            if (device4 != null)
            {
                int error = await d4c2[0].WriteAsync(epoch_array);
                Task.Delay(1000).Wait();
                error = await d4c2[0].WriteAsync(new byte[] { 0x0C });
            }
            if (device5 != null)
            {
                int error = await d5c2[0].WriteAsync(epoch_array);
                Task.Delay(1000).Wait();
                error = await d5c2[0].WriteAsync(new byte[] { 0x0C });
            }

            while (true)
            {
                try
                {
                    int error_ = await CollectionCommand();
                    if (error_ != 0)
                        continue;
                }
                catch (DeviceConnectionException ex)
                {
                    // await DisplayAlert("Error disconnected!", ex.Message, "Ok.");
                    break;
                }
                await Task.Delay(800);
            }
        }


        /***********************************************
         * Holds the loop for collecting data over BLE.
        ***********************************************/
        public async Task<int> CollectionCommand()
        {
            try
            {
                if (device1!=null) await ProcessDeviceData(device1, d1c2, d1_temp1, d1_temp2, d1_temp3);
                if (device2!=null) await ProcessDeviceData(device2, d2c2, d2_temp1, d2_temp2, d2_temp3);
                if (device3!=null) await ProcessDeviceData(device3, d3c2, d3_temp1, d3_temp2, d3_temp3);
                if (device4!=null) await ProcessDeviceData(device4, d4c2, d4_temp1, d4_temp2, d4_temp3);
                if (device5!=null) await ProcessDeviceData(device5, d5c2, d5_temp1, d5_temp2, d5_temp3);
            }
            catch (DeviceConnectionException ex)
            {
                // Device.BeginInvokeOnMainThread
                // (
                //     async() => await DisplayAlert("Error!", ex.Message, "Ok.")
                // );
                return -1;
            }
            return 0;
        }


        /**********************************************************************
         * Remove a device from the list of connected devices if disconnected
        **********************************************************************/
        private void ClearDevice(IDevice device)
        {
            if (device == device1)
            {
                Fiber1.Text = "Not Connected";
                Fiber1.TextColor = Color.FromHex("#000000");
                Shirt1.Source = new FileImageSource { File = "Man30.png" };
                device1 = null;
                d1s1.Clear(); d1s2.Clear(); d1c1.Clear(); d1c2.Clear();
                d1_temp1.Clear(); d1_temp2.Clear(); d1_temp3.Clear();
                d1_temp1.Add("--"); d1_temp2.Add("--"); d1_temp3.Add("--");
            }
            else if (device == device2)
            {
                Fiber2.Text = "Not Connected";
                Fiber2.TextColor = Color.FromHex("#000000");
                Shirt2.Source = new FileImageSource { File = "Man30.png" };
                device2 = null;
                d2s1.Clear(); d2s2.Clear(); d2c1.Clear(); d2c2.Clear();
                d2_temp1.Clear(); d2_temp2.Clear(); d2_temp3.Clear();
                d2_temp1.Add("--"); d2_temp2.Add("--"); d2_temp3.Add("--");
            }
            else if (device == device3)
            {
                Fiber3.Text = "Not Connected";
                Fiber3.TextColor = Color.FromHex("#000000");
                Shirt3.Source = new FileImageSource { File = "Man30.png" };
                device3 = null;
                d3s1.Clear(); d3s2.Clear(); d3c1.Clear(); d3c2.Clear();
                d3_temp1.Clear(); d3_temp2.Clear(); d3_temp3.Clear();
                d3_temp1.Add("--"); d3_temp2.Add("--"); d3_temp3.Add("--");
            }
            else if (device == device4)
            {
                Fiber4.Text = "Not Connected";
                Fiber4.TextColor = Color.FromHex("#000000");
                Shirt4.Source = new FileImageSource { File = "Man30.png" };
                device4 = null;
                d4s1.Clear(); d4s2.Clear(); d4c1.Clear(); d4c2.Clear();
                d4_temp1.Clear(); d4_temp2.Clear(); d4_temp3.Clear();
                d4_temp1.Add("--"); d4_temp2.Add("--"); d4_temp3.Add("--");
            }
            else if (device == device5)
            {
                Fiber5.Text = "Not Connected";
                Fiber5.TextColor = Color.FromHex("#000000");
                Shirt5.Source = new FileImageSource { File = "Man30.png" };
                device5 = null;
                d5s1.Clear(); d5s2.Clear(); d5c1.Clear(); d5c2.Clear();
                d5_temp1.Clear(); d5_temp2.Clear(); d5_temp3.Clear();
                d5_temp1.Add("--"); d5_temp2.Add("--"); d5_temp3.Add("--");
            }
        }


        private async Task ProcessDeviceData(
            IDevice device, 
            ObservableCollection<ICharacteristic> connection, 
            ObservableCollection<string> temp1,
            ObservableCollection<string> temp2,
            ObservableCollection<string> temp3)
        {
            try
            {
                if (device==null)  // Check that device exists
                    return;
                    
                if (connection.Count < 3)   // Check that index 2 exists
                    return;

                (byte[], int) bytes;
                try
                {
                    bytes = await connection[2]?.ReadAsync();
                }
                // catch (Exception ex)
                catch (DeviceConnectionException ex)
                {
                    ClearDevice(device);
                    return;
                }

                // Exit if bytes or bytes.Item1 is null or insufficiently initialized
                if (bytes.Item1 == null || bytes.Item1.Length < 6)
                    return;

                string t1 = CombineBytesToBinaryString( bytes.Item1[0], bytes.Item1[1] );
                string t2 = CombineBytesToBinaryString( bytes.Item1[2], bytes.Item1[3] );
                string t3 = CombineBytesToBinaryString( bytes.Item1[4], bytes.Item1[5] );
                string temp1_string = Math.Round( ParseFloat16(t1), 1).ToString() + "°";
                string temp2_string = Math.Round( ParseFloat16(t2), 1).ToString() + "°";
                string temp3_string = Math.Round( ParseFloat16(t3), 1).ToString() + "°";

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
                if (!(temp3.ElementAt(0)==temp3_string))
                {
                    temp3.RemoveAt(0);
                    temp3.Add(temp3_string);
                }
            }
            catch (DeviceConnectionException ex)
            {
                ClearDevice(device);
                return;
            }
            catch (Exception ex)
            {
                return;
            }
        }


        /********************************************************
         * Resume collection of data after command already sent.
        ********************************************************/
        async void OnCollectClicked(object sender, EventArgs args)
        {
            // Popup for LiveCharts
            // Series = new ObservableCollection<ISeries>{};

            // if (device1 != null)
            // {
            //     Series.Add(new LineSeries<ObservablePoint>
            //     {
            //         Values = d1_points,
            //         Fill = null,
            //         Name = device1.Name
            //     });
            // }
            // if (device2 != null)
            // {
            //     Series.Add(new LineSeries<ObservablePoint>
            //     {
            //         Values = d2_points,
            //         Fill = null,
            //         Name = device2.Name
            //     });
            // }
            // if (device3 != null)
            // {
            //     Series.Add(new LineSeries<ObservablePoint>
            //     {
            //         Values = d3_points,
            //         Fill = null,
            //         Name = device3.Name
            //     });
            // }
            // if (device4 != null)
            // {
            //     Series.Add(new LineSeries<ObservablePoint>
            //     {
            //         Values = d4_points,
            //         Fill = null,
            //         Name = device4.Name
            //     });
            // }
            // if (device5 != null)
            // {
            //     Series.Add(new LineSeries<ObservablePoint>
            //     {
            //         Values = d5_points,
            //         Fill = null,
            //         Name = device5.Name
            //     });
            // }

            // Popup a CartesianChart
            // graph = new Popup
            // {
                // Content = new StackLayout
                // {
                //     CartesianChart
                //     {
                //         Series = Series,
                //         XAxes = x_axis,
                //         YAxes = y_axis,
                //         LegendBackground = new SolidColorPaint { Color = SKColors.White },
                //         LegendBorder = new SolidColorPaint { Color = SKColors.Black },
                //         LegendPadding = 10,
                //         LegendMargin = 10,
                //         LegendFont = new SKPaint { TextSize = 14, IsAntialias = true, Color = SKColors.Black },
                //         LegendTextPaint = new SKPaint { TextSize = 14, IsAntialias = true, Color = SKColors.Black }
                //     }
                // }
            // };
        }

        // HELPER: Clears device to Blue (-1) Disconnection State
        private void ClearDeviceConnectionException(IDevice device)
        {
            if (device == device1)
            {
                Fiber1.Text = "Device Connection Error! Tap to retry connecting.";
                Fiber1.TextColor = Color.FromHex("#002AD1");
                Shirt1.Source = new FileImageSource { File = "Blue.jpg" };
                device1 = null;
                d1s1.Clear(); d1s2.Clear(); d1c1.Clear(); d1c2.Clear();
                d1_temp1.Clear(); d1_temp2.Clear(); d1_temp3.Clear();
                d1_temp1.Add("--"); d1_temp2.Add("--"); d1_temp3.Add("--");
            }
            else if (device == device2)
            {
                Fiber2.Text = "Device Connection Error! Tap to retry connecting.";
                Fiber2.TextColor = Color.FromHex("#002AD1");
                Shirt2.Source = new FileImageSource { File = "Blue.jpg" };
                device2 = null;
                d2s1.Clear(); d2s2.Clear(); d2c1.Clear(); d2c2.Clear();
                d2_temp1.Clear(); d2_temp2.Clear(); d2_temp3.Clear();
                d2_temp1.Add("--"); d2_temp2.Add("--"); d2_temp3.Add("--");
            }
            else if (device == device3)
            {
                Fiber3.Text = "Device Connection Error! Tap to retry connecting.";
                Fiber3.TextColor = Color.FromHex("#002AD1");
                Shirt3.Source = new FileImageSource { File = "Blue.jpg" };
                device3 = null;
                d3s1.Clear(); d3s2.Clear(); d3c1.Clear(); d3c2.Clear();
                d3_temp1.Clear(); d3_temp2.Clear(); d3_temp3.Clear();
                d3_temp1.Add("--"); d3_temp2.Add("--"); d3_temp3.Add("--");
            }
            else if (device == device4)
            {
                Fiber4.Text = "Device Connection Error! Tap to retry connecting.";
                Fiber4.TextColor = Color.FromHex("#002AD1");
                Shirt4.Source = new FileImageSource { File = "Blue.jpg" };
                device4 = null;
                d4s1.Clear(); d4s2.Clear(); d4c1.Clear(); d4c2.Clear();
                d4_temp1.Clear(); d4_temp2.Clear(); d4_temp3.Clear();
                d4_temp1.Add("--"); d4_temp2.Add("--"); d4_temp3.Add("--");
            }
            else if (device == device5)
            {
                Fiber5.Text = "Device Connection Error! Tap to retry connecting.";
                Fiber5.TextColor = Color.FromHex("#002AD1");
                Shirt5.Source = new FileImageSource { File = "Blue.jpg" };
                device5 = null;
                d5s1.Clear(); d5s2.Clear(); d5c1.Clear(); d5c2.Clear();
                d5_temp1.Clear(); d5_temp2.Clear(); d5_temp3.Clear();
                d5_temp1.Add("--"); d5_temp2.Add("--"); d5_temp3.Add("--");
            }
        }


        /***************************************************
         * Perform Chip Erase (0xCE) with device selector.
        ***************************************************/
        async void OnChipErase(object sender, EventArgs args)
        {
            string action = await DisplayActionSheet(
                "Which device would you like to erase?",
                "Cancel",
                null,
                "1", "2", "3", "4", "5"
            );

            switch (action)
            {
                case "Cancel":
                    break;
                case "1":
                    if (device1 == null)
                        await DisplayAlert("Error", "No device connected.", "OK");
                    else
                        await d1c2[0].WriteAsync(new byte[] { 0xCE });
                    break;
                case "2":
                    if (device2 == null)
                        await DisplayAlert("Error", "No device connected.", "OK");
                    else
                        await d2c2[0].WriteAsync(new byte[] { 0xCE });
                    break;
                case "3":
                    if (device3 == null)
                        await DisplayAlert("Error", "No device connected.", "OK");
                    else
                        await d3c2[0].WriteAsync(new byte[] { 0xCE });
                    break;
                case "4":
                    if (device4 == null)
                        await DisplayAlert("Error", "No device connected.", "OK");
                    else
                        await d4c2[0].WriteAsync(new byte[] { 0xCE });
                    break;
                case "5":
                    if (device5 == null)
                        await DisplayAlert("Error", "No device connected.", "OK");
                    else
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

            // This line is unreachable, but C# requires a return statement in all code paths
            return 0.0f;
        }

    }
}
