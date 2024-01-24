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
        public IDevice device;

        public Color Subject1Color { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


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
            
            // Device.BeginInvokeOnMainThread(() =>
            // {
                // Subject1Color = Color.FromHex("#9C9C9C");
            // });

            // Subject1.Image = UIImage.FromBundle("body");
            // Subject1.Source = ImageSource.FromFile("Man10.png");
            // Subject1.Source = ImageSource.FromResource("Man10.png");
            // Subject1.Source = ImageSource.FromFile
            // Subject1.Source = ImageSource.FromFile("Icon20.png");

            // Check for location permission
            GetLocationPermission();
        }


        private async void GetLocationPermission()
        {
            if (Device.RuntimePlatform == Device.Android)
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
                {
                    list.Add(args.Device);
                }
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
            if (device == null)
            {
                await DisplayAlert("Error!", "Not connected to a device.", "OK");
                // return;
            }
            else
                await Navigation.PushAsync(new ConnectedPage(device));
        }


        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            device = DevicesList.SelectedItem as IDevice;

            var result = await DisplayAlert
            (
                "Connect to Device.",
                "Do you want to connect to " + device.Name + "?",
                "Connect", "Cancel"
            );

            if (!result)
                return;

            await adapter.StopScanningForDevicesAsync(); // Stop Scanner

            try
            {
                await adapter.ConnectToDeviceAsync(device);
                await DisplayAlert("Connected", "Status: " + device.State, "OK");
            }
            catch (DeviceConnectionException ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

    }
}
