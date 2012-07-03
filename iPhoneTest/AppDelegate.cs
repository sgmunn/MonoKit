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
using System.Drawing;

namespace iPhoneTest
{
    public class TestPanorama : UIPanoramaViewController
    {
        public TestPanorama()
        {
        }

        public override void LoadView()
        {
            base.LoadView();

            Func<UIView> createView = () => new UIView() { BackgroundColor = UIColor.Green };

            this.Panorama.TitleText = "my minions";
            this.Panorama.TextColor = UIColor.DarkGray;

            this.Panorama.ContentItems.Add(new ContentItem("item 1", createView, 0));
            this.Panorama.ContentItems.Add(new ContentItem("item 2", createView, 600));
            this.Panorama.ContentItems.Add(new ContentItem("item 3", createView, 0));

            var tiledBackground = true;

            if (tiledBackground)
            {
                var img = UIImage.FromBundle("Images/brushTexture1.png");

                this.Panorama.BackgroundView.AddSubview(
                    new UIView(new RectangleF(0, 0, 1200, 1000)) { BackgroundColor = UIColor.FromPatternImage(img)}
                );

            }
            else
            {
                var img = UIImage.FromBundle("Images/whereswalle.jpg");

                this.Panorama.BackgroundView.BackgroundColor = UIColor.FromPatternImage(img);
                this.Panorama.BackgroundView.AddSubview(
                    new UIImageView(img) { AutoresizingMask = UIViewAutoresizing.FlexibleDimensions, }
                );
            }
        }
    }

    public class SamplePanorama : UIPanoramaViewController
    {
        private TableViewController content1Controller;
        private TableViewController content2Controller;
        private TableViewController content3Controller;

        private Samples2 monokitSamples;

        public SamplePanorama(UINavigationController controller)
        {
            this.monokitSamples = new Samples2(controller);
        }

        public override void LoadView()
        {
            base.LoadView();

            this.Panorama.TitleText = "monokit samples";

            this.Panorama.ContentItems.Add(new ContentItem("samples", this.CreateContentItem1View, 0));
            this.Panorama.ContentItems.Add(new ContentItem("tests", this.CreateContentItem2View, 600));
            this.Panorama.ContentItems.Add(new ContentItem("utils", this.CreateContentItem3View, 0));

            var img = UIImage.FromBundle("Images/whereswalle.jpg");

            this.Panorama.BackgroundView.AddSubview(
                new UIImageView(img) {
                    AutoresizingMask = UIViewAutoresizing.FlexibleDimensions,

                });

        }

        private UIView CreateContentItem1View()
        {
            this.content1Controller = new TableViewController(UITableViewStyle.Plain);
            this.monokitSamples.SetupContent1(this.content1Controller.Source);

            //this.content1Controller.View.BackgroundColor = UIColor.Clear;
            return this.content1Controller.View;
        }

        private UIView CreateContentItem2View()
        {
            this.content2Controller = new TableViewController(UITableViewStyle.Plain);
            this.monokitSamples.SetupContent2(this.content2Controller.Source);

            //this.content2Controller.View.BackgroundColor = UIColor.Clear;
            return this.content2Controller.View;
        }

        private UIView CreateContentItem3View()
        {
            this.content3Controller = new TableViewController(UITableViewStyle.Plain);
            this.monokitSamples.SetupContent3(this.content3Controller.Source);

            //this.content3Controller.View.BackgroundColor = UIColor.Clear;
            return this.content3Controller.View;
        }
    }




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
            /// Just a note, the panorama works best with transparent views, but I've only just thrown these
            /// ones in to handle both the old samples and the new panorama view
            /// make yours views styled better ! :)


            this.InitPanoramaSample();
            //this.InitSamplesWithPanoramaView();
            //this.InitSamples();

            return true;
        }

        private void InitPanoramaSample()
        {
            this.window = new UIWindow(UIScreen.MainScreen.Bounds);

            this.window.RootViewController = new TestPanorama();
            this.window.MakeKeyAndVisible();
        }

        private void InitSamplesWithPanoramaView()
        {
            this.window = new UIWindow(UIScreen.MainScreen.Bounds);

            this.controller = new UINavigationController();
            //this.controller.NavigationBarHidden = true;

            this.window.RootViewController = this.controller;

            this.controller.PushViewController(new SamplePanorama(this.controller), false);

            this.window.MakeKeyAndVisible();
        }

        private void InitSamples()
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

