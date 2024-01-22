using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Xamarin.Essentials;


namespace BLE_Universal.Droid
{
    [Activity(Label = "BLE_Universal", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            GetBLEPermission();
        }

        private async void GetBLEPermission()
        {
            /////////////////////////////////////////////////////////////////
            // Added to get Bluetooth Permissions ///////////////////////////
            await Permissions.CheckStatusAsync<BluetoothConnectPermission>();
            await Permissions.RequestAsync<BluetoothConnectPermission>();
            /////////////////////////////////////////////////////////////////
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }

    public class BluetoothConnectPermission : Xamarin.Essentials.Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
            => new (string androidPermission, bool isRuntime)[]
        {
            (Android.Manifest.Permission.BluetoothScan,    true),
            (Android.Manifest.Permission.BluetoothConnect, true),
        };

    }
}