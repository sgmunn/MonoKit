//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file=".cs" company="sgmunn">
//    (c) sgmunn 2012  
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//    documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//    to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//    the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//    THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//    CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//    IN THE SOFTWARE.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
//
using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoKit.Metro;
using System.Drawing;
using MonoKit.UI;
using MonoKit.UI.Elements;

namespace PanoramaSample
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        UIWindow window;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // create a new window instance based on the screen size
            this.window = new UIWindow(UIScreen.MainScreen.Bounds);

            var p = new TestPanorama();
            this.window.RootViewController = p;

//            p.Title = "my minions";
//            p.AddContent(new ContentItem(150) {Controller = new Item1Controller() });
//            p.AddContent(new ContentItem(400) {Controller = new Item2Controller() });
//            p.AddContent(new ContentItem(0) {Controller = new Item3Controller() });


            this.window.MakeKeyAndVisible();
            
            return true;
        }
    }

    public class TestPanorama : UIPanoramaViewController
    {
        public TestPanorama()
        {

            this.Title = "my minions";
            this.AddController(new Item2Controller(), 0);
            this.AddController(new Item1Controller(), 0);
            this.AddController(new Item3Controller(), 0);
        }

        public override void LoadView()
        {
            base.LoadView();

            this.BackgroundView.BackgroundColor = UIColor.Brown;

            var tiledBackground = true;

            if (tiledBackground)
            {
                var img = UIImage.FromBundle("Images/brushTexture1.png");

                this.BackgroundView.AddSubview(
                    new UIView(new RectangleF(0, 0, 1200, 1000)) { BackgroundColor = UIColor.FromPatternImage(img)}
                );

            }
            else
            {
                var img = UIImage.FromBundle("Images/whereswalle.jpg");

//                this.Panorama.BackgroundView.BackgroundColor = UIColor.FromPatternImage(img);
//                this.Panorama.BackgroundView.AddSubview(
//                    new UIImageView(img) { AutoresizingMask = UIViewAutoresizing.FlexibleDimensions, }
//                );
            }
        }
    }

    public class Item1Controller : TableViewController
    {
        private bool doneInit;
        public Item1Controller() : base(UITableViewStyle.Plain)
        {
            this.Title = "item 1";
        }

        public override void LoadView()
        {
            base.LoadView();
            if (!doneInit)
            {
            this.InitSample();
            }

            doneInit = true;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Console.WriteLine("will apear 1");
        }

        
        public void InitSample()
        {
            var section1 = new TableViewSection(this.Source);
            section1.Add(new StringElement("test") { Command = this.DoTest });
            section1.Add(new StringElement("add") { Command = this.DoAdd });
        }

        private void DoTest(Element element)
        {
            var p = this.ParentViewController as UIPanoramaViewController;
            p.Present(new OtherController());
        }

        private void DoAdd(Element element)
        {
            var p = this.ParentViewController as UIPanoramaViewController;
            p.AddController(new Item3Controller());
        }

    }

    public class Item2Controller : UIViewController
    {
        public Item2Controller() : base()
        {
            this.Title = "item 2";
        }

        public override void LoadView()
        {
            base.LoadView();
            this.View = new UIView() { BackgroundColor = UIColor.Cyan };
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Console.WriteLine("will apear 2");
        }
    }

    public class Item3Controller : TableViewController
    {
        public Item3Controller() : base(UITableViewStyle.Plain)
        {
            this.Title = "item 3";
        }

        public override void LoadView()
        {
            base.LoadView();

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Console.WriteLine("will apear 3");
        }
    }

    
    public class OtherController : TableViewController
    {
        public OtherController() : base(UITableViewStyle.Grouped)
        {
        }

        public override void LoadView()
        {
            base.LoadView();
            this.InitController();
            this.View.BackgroundColor = UIColor.Clear;
        }

        public void InitController()
        {
            var section1 = new TableViewSection(this.Source);
            section1.Header = " ";
            section1.Add(new StringElement("Close") { Command = this.Close });
        }
        
        public void Close(Element element)
        {
            var p = this.ParentViewController as UIPanoramaViewController;
            p.Dismiss();



            //this.DismissViewController(true, null);
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }


}

