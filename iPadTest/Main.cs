using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoKit.UI;
using MonoKit.UI.Elements;
using System.ComponentModel;
using System.Collections.Generic;
using MonoKit.DataBinding;
using MonoKitSample;
using MonoKit.Metro;
using System.Drawing;

namespace iPadTest
{
    public class Application
    {
        static void Main(string[] args)
        {
            UIApplication.Main(args);
        }
    }

    // todo: fix up height when we rotate panorama 
    // todo: fix up width when we rotate panorama

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

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
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
            this.InitPanoramaSample();
//            this.InitSamples();

            return true;
        }
        
        private void InitPanoramaSample()
        {
            //this.window = new UIWindow(UIScreen.MainScreen.Bounds);

            this.window.RootViewController = new TestPanorama();
            this.window.MakeKeyAndVisible();
        }

        private void InitSamples()
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
        }
    }
}

