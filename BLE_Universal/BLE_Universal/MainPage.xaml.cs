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

            list = new ObservableCollection<IDevice>();
            DevicesList.ItemsSource = list;

            adapter.DeviceDiscovered += OnDeviceDiscovered;

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
                if ((!list.Contains(args.Device)) && (args.Device.Name != null))
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

            // 3D2E945C-F2A5-8B5E-F91C-43E902C36A46 epc for the DA14585 RCU code
            await adapter.StartScanningForDevicesAsync();
        }

        private async void ToConnectedPage(object sender, EventArgs e)
        {
            if (device == null)
            {
                await DisplayAlert("Error!", "Not connected to a device.", "OK");
                return;
            }
            else
                await Navigation.PushAsync(new ConnectedPage(device));
        }

        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            device = DevicesList.SelectedItem as IDevice;

            var result = await DisplayAlert(
                "Connect to Device.",
                "Do you want to connect to " + device.Name + "?",
                "Connect", "Cancel"
            );

            if (!result)
                return;

            await adapter.StopScanningForDevicesAsync();  // Stop Scanner

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
