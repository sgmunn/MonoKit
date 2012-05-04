using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoKit.UI;
using MonoKit.UI.Elements;
using System.ComponentModel;
using System.Collections.Generic;
using MonoKit.DataBinding;
using MonoKitSample;

namespace iPadTest
{
    public class Application
    {
        static void Main(string[] args)
        {
            UIApplication.Main(args);
        }
    }
    
    public partial class AppDelegate : UIApplicationDelegate
    {
        private Samples monokitSamples;
        private UINavigationController controller;
        private TableViewController tableController;
        
        // This method is invoked when the application has loaded its UI and its ready to run
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            this.controller = new UINavigationController();
            this.monokitSamples = new Samples(this.controller);

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

