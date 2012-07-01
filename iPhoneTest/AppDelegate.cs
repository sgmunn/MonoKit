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
using MonoKit.Metro;

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

        private TableViewController content1Controller;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {

            /// Just a note, the panorama works best with transparent views, but I've only just thrown these
            /// ones in to handle both the old samples and the new panorama view
            /// make yours views styled better ! :)



            this.InitSamples();
            //this.InitSamplesOld();

            return true;
        }

        private void InitSamples()
        {
            this.window = new UIWindow(UIScreen.MainScreen.Bounds);

            this.controller = new UINavigationController();
            this.controller.NavigationBarHidden = true;
            this.monokitSamples = new Samples2(this.controller);

            this.window.RootViewController = this.controller;

            this.controller.PushViewController(this.CreateMainPanorama(), false);

            this.window.MakeKeyAndVisible();
        }

        private UIPanoramaViewController CreateMainPanorama()
        {
            var panorama = new UIPanoramaViewController();

            panorama.Title = "monokit samples";

            panorama.Add(new ContentItem("samples", this.CreateContentItem1View, 0));
            panorama.Add(new ContentItem("tests", this.CreateContentItem2View, 600));
            panorama.Add(new ContentItem("utils", this.CreateContentItem3View, 0));

            var img = UIImage.FromBundle("Images/whereswalle.jpg");

            panorama.BackgroundView.AddSubview(
                new UIImageView(img) {
                    AutoresizingMask = UIViewAutoresizing.FlexibleDimensions,

                });

            return panorama;
        }

        private UIView CreateContentItem1View()
        {
            this.content1Controller = new TableViewController(UITableViewStyle.Plain);
            ((Samples2)this.monokitSamples).SetupContent1(this.content1Controller.Source);

            //this.content1Controller.View.BackgroundColor = UIColor.Clear;
            return this.content1Controller.View;
        }

        private UIView CreateContentItem2View()
        {
            this.content1Controller = new TableViewController(UITableViewStyle.Plain);
            ((Samples2)this.monokitSamples).SetupContent2(this.content1Controller.Source);

            //this.content1Controller.View.BackgroundColor = UIColor.Clear;
            return this.content1Controller.View;
        }

        private UIView CreateContentItem3View()
        {
            this.content1Controller = new TableViewController(UITableViewStyle.Plain);
            ((Samples2)this.monokitSamples).SetupContent3(this.content1Controller.Source);

            //this.content1Controller.View.BackgroundColor = UIColor.Clear;
            return this.content1Controller.View;
        }

        private void InitSamplesOld()
        {
            this.controller = new MyNavigationController();
            this.monokitSamples = new Samples(this.controller);
            
            // create a new window instance based on the screen size
            this.window = new UIWindow(UIScreen.MainScreen.Bounds);
   
            // setup main index
            this.tableController = new TableViewController(UITableViewStyle.Grouped);
            this.tableController.Title = "MonoKit";
            this.controller.PushViewController(this.tableController, false);
            this.monokitSamples.SetupMainIndexSection(this.tableController.Source);

            this.window.RootViewController = this.controller;
            this.window.MakeKeyAndVisible();
        }
    }
}

