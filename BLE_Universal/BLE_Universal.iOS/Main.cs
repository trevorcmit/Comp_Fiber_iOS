﻿using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;


namespace BLE_Universal.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate" you can specify it here.
            UIApplication.Main(args, null, typeof(AppDelegate));
            // set orientation to landscape
            UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.LandscapeLeft), new NSString("orientation"));
        }

    }
}
