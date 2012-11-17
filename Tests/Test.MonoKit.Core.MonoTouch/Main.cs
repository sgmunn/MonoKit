using System;
using MonoTouch.UIKit;

namespace MonoKit.MonoTouch.Tests
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => { Console.WriteLine(e); };

            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
