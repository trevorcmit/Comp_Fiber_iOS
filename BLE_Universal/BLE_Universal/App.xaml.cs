﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace BLE_Universal
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Switched to Nagivation Page setup for UWP Compatibility
            MainPage = new NavigationPage (new MainPage());

            // Set orientation to landscape
            // MainPage = new MainPage();
            // MainPage.SetValue(NavigationPage.HasNavigationBarProperty, false);           
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

    }
}
