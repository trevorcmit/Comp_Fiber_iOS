using Plugin.BLE;
using Plugin.BLE.Abstractions;
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
        public Color Subject1Color, Subject2Color, Subject3Color, Subject4Color, Subject5Color;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Bound variables for XAML of five devices
        public ImageSource im1, im2, im3, im4, im5;
        public string f1, f2, f3, f4, f5;


        public MainPage()
        {
            InitializeComponent();
            adapter = CrossBluetoothLE.Current.Adapter;
            bluetoothBLE = CrossBluetoothLE.Current;

            // Set up list of devices
            list = new ObservableCollection<IDevice>();
            DevicesList.ItemsSource = list;

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

            // Check for location permission
            GetLocationPermission();
        }


        private async void GetLocationPermission()
        {
            if (Device.RuntimePlatform==Device.Android)
            {
                var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
        }


        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // Filter out devices by IFM prefix.
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


        private async void ToConnectedPage(object sender, EventArgs e)
        {
            if (device1 == null)
                await DisplayAlert("Error!", "Not connected to a device.", "OK");
            else
            {
                // We cannot use null devices as inputs to the constructor.
                // Thus we must check how many devices are connected/passable.

                if (device2 == null)
                    await Navigation.PushAsync( new ConnectedPage(device1) );
                else if (device3 == null)
                    await Navigation.PushAsync( new ConnectedPage(device1, device2) );
                else if (device4 == null)
                    await Navigation.PushAsync( new ConnectedPage(device1, device2, device3) );
                else if (device5 == null)
                    await Navigation.PushAsync( new ConnectedPage(device1, device2, device3, device4) );
                else
                    await Navigation.PushAsync( new ConnectedPage(device1, device2, device3, device4, device5) );
            }
        }


        async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (device1==null)
            {
                device1 = DevicesList.SelectedItem as IDevice;
                await Setup_Device(0, device1);
            }
            else if (device2==null)
            {
                device2 = DevicesList.SelectedItem as IDevice;
                await Setup_Device(1, device2);
            }
            else if (device3==null)
            {
                device3 = DevicesList.SelectedItem as IDevice;
                await Setup_Device(2, device3);
            }
            else if (device4==null)
            {
                device4 = DevicesList.SelectedItem as IDevice;
                await Setup_Device(3, device4);
            }
            else if (device5==null)
            {
                device5 = DevicesList.SelectedItem as IDevice;
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
            var result = await DisplayAlert
            (
                "Connect fiber to Subject " + (index + 1).ToString() + ".",
                "Do you want to connect to " + d.Name + "?",
                "Connect", "Cancel"
            );

            if (!result) return -1;   // For debugging if necessary

            await adapter.StopScanningForDevicesAsync(); // Stop Scanner

            try
            {
                await adapter.ConnectToDeviceAsync(d);
                await DisplayAlert("Connected", "Status: " + d.State, "OK");

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

                return 0;
            }
            catch (DeviceConnectionException ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
                return 1;
            }
        }

    }
}
