using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Essentials;



namespace BLE_Universal
{
    public partial class MainPage : ContentPage
    {
        // Plugin.BLE Documentation for Xamarin Forms package
        // https://github.com/dotnet-bluetooth-le/dotnet-bluetooth-le
        public IAdapter adapter;
        public IBluetoothLE bluetoothBLE;
        public ObservableCollection<IDevice> list;
        public IDevice device1, device2, device3, device4, device5;
        Popup popup;
        int SELECTED;
        public Dictionary<char, float> S = new Dictionary<char, float>() { { '0', 1.0f }, { '1', -1.0f } };

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
            
            Device.BeginInvokeOnMainThread(() =>
            {
                Shirt1.Source = new FileImageSource { File = "Man30.png" };
                Shirt2.Source = new FileImageSource { File = "Man30.png" };
                Shirt3.Source = new FileImageSource { File = "Man30.png" };
                Shirt4.Source = new FileImageSource { File = "Man30.png" };
                Shirt5.Source = new FileImageSource { File = "Man30.png" };
                Fiber1.Text = "Not Connected";
                Fiber2.Text = "Not Connected";
                Fiber3.Text = "Not Connected";
                Fiber4.Text = "Not Connected";
                Fiber5.Text = "Not Connected";
                Fiber1.TextColor = Color.FromHex("#000000");
                Fiber2.TextColor = Color.FromHex("#000000");
                Fiber3.TextColor = Color.FromHex("#000000");
                Fiber4.TextColor = Color.FromHex("#000000");
                Fiber5.TextColor = Color.FromHex("#000000");
            });
        }


        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if ((!list.Contains(args.Device)) && (args.Device.Name != null) && (args.Device.Name.Contains("IFM")))
                    list.Add(args.Device);
            });
        }


        private async void SearchDevices(object sender, EventArgs e)
        {
            list.Clear();
            adapter.ScanTimeout = 10000;
            adapter.ScanMode = ScanMode.Balanced;
            await adapter.StartScanningForDevicesAsync();
        }


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
                            FontSize = 24,
                            Margin = new Thickness(0, 10)
                        },
                        listview,     // List of Devices Found
                        new Button    // Close Button
                        {
                            Text = "Search",
                            FontSize=20,
                            Command = new Command(() => SearchDevices(null, null)),
                            BackgroundColor = Color.FromHex("#0C0C0C"),
                        }
                    }
                }
            };

            // Show the Popup
            var result = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
        }


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


        async Task<int> Setup_Device(int index, IDevice d)
        {
            // var result = await DisplayAlert
            // (
            //     "Connect fiber to Subject " + (index + 1).ToString() + ".",
            //     "Do you want to connect to " + d.Name + "?",
            //     "Connect", "Cancel"
            // );
            // if (!result) return -1;   // For debugging if necessary

            await adapter.StopScanningForDevicesAsync(); // Stop Scanner

            try
            {
                await adapter.ConnectToDeviceAsync(d);
                // await DisplayAlert("Connected", "Status: " + d.State, "OK");

                if (index==0)
                {
                    Fiber1.Text = d.Name;
                    Fiber1.TextColor = Color.FromHex("#008F30");
                    Shirt1.Source = new FileImageSource { File = "Green30.jpeg" };
                }
                else if (index==1)
                {
                    Fiber2.Text = d.Name;
                    Fiber2.TextColor = Color.FromHex("#008F30");
                    Shirt2.Source = new FileImageSource { File = "Green30.jpeg" };
                }
                else if (index==2)
                {
                    Fiber3.Text = d.Name;
                    Fiber3.TextColor = Color.FromHex("#008F30");
                    Shirt3.Source = new FileImageSource { File = "Green30.jpeg" };

                }
                else if (index==3)
                {
                    Fiber4.Text = d.Name;
                    Fiber4.TextColor = Color.FromHex("#008F30");
                    Shirt4.Source = new FileImageSource { File = "Green30.jpeg" };
                }
                else if (index==4)
                {
                    Fiber5.Text = d.Name;
                    Fiber5.TextColor = Color.FromHex("#008F30");
                    Shirt5.Source = new FileImageSource { File = "Green30.jpeg" };
                }

                popup.Dismiss(null); // Dismiss the popup automatically

                return 0;
            }
            catch (DeviceConnectionException ex)
            {
                await DisplayAlert("Error!", ex.Message, "Ok");
                return 1;
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
            // Map sign bit string to multiplicand
            // Dictionary<char, float> S = new Dictionary<char, float>()
            // {
            //     { '0', 1.0f },
            //     { '1', -1.0f },
            // };

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
