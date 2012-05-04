using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoKit.UI.PagedViews;
using System.ComponentModel;
using MonoKit.UI;
using MonoKit.UI.Elements;
using MonoKit.DataBinding;
using MonoKitSample;

namespace iPhoneTest
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        private UIWindow window;
        private Samples monokitSamples;
        
        public class MyNavigationController : UINavigationController
        {
            public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
            {
                return true;
            }
        }
        
        private UINavigationController controller;
        private TableViewController tableController;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            this.controller = new MyNavigationController();
            this.monokitSamples = new Samples(this.controller);
            
            // create a new window instance based on the screen size
            this.window = new UIWindow(UIScreen.MainScreen.Bounds);

            this.window.AddSubview(this.controller.View);
   
            // setup main index
            this.tableController = new TableViewController(UITableViewStyle.Grouped);
            this.tableController.Title = "MonoKit";
            this.controller.PushViewController(this.tableController, false);
            this.monokitSamples.SetupMainIndexSection(this.tableController.Source);
            
            this.window.MakeKeyAndVisible();
            
            return true;
        }
    }
}

