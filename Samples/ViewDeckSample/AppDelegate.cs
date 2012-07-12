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
using MonoKit.UI.ViewDeck;

namespace ViewDeckSample
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
            window = new UIWindow(UIScreen.MainScreen.Bounds);
            
            window.RootViewController = sample2();

            // If you have defined a view, add it here:
            // window.AddSubview (navigationController.View);
            
            // make the window visible
            window.MakeKeyAndVisible();
            
            return true;
        }

        
        private UIViewController sample0()
        {
            var centerController = new Center();

            var deckController = new ViewDeckController(centerController);
            deckController.RightLedge = 100;

            return deckController;
        }

        private UIViewController sample1()
        {

            var leftController = new Left(); // << subclass and create a view
            var rightController = new Right();

            var centerController = new Center();

            var deckController = new ViewDeckController(centerController, leftController, rightController);
            deckController.RightLedge = 100;

            return deckController;
        }

        private UIViewController sample2()
        {
            return new NavStart();
        }

        private UIViewController sample3()
        {
            var leftController = new Left(); 
            var bottomController = new Right();

            var centerController = new Center();
            centerController.Title = "Center 1";

            var secondDeckController = new ViewDeckController(leftController, bottomController);
            secondDeckController.LeftLedge = 100;

            var deckController = new ViewDeckController(centerController, secondDeckController);
            deckController.LeftLedge = 30;

            return deckController;
        }

    }

    
    public class NavStart : UIViewController
    {
        public override void LoadView()
        {
            base.LoadView();
            this.View = new UIView();
            this.View.BackgroundColor = UIColor.Gray;
            var btn = new UIButton(UIButtonType.RoundedRect);
            btn.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            btn.SetTitle("nav start", UIControlState.Normal);
            this.View.AddSubview(btn);

            this.NavigationItem.Title = "nav start";

            btn.TouchUpInside += delegate(object sender, EventArgs e) {
                var center = new NavCenter();
                // behaviour = integration
                this.PresentViewController(new UINavigationController(center), true, null);
            };
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }

    public class NavCenter : UIViewController
    {
        public override void LoadView()
        {
            base.LoadView();
            this.View = new UIView();
            this.View.BackgroundColor = UIColor.Gray;
            var btn = new UIButton(UIButtonType.RoundedRect);
            btn.Frame = new System.Drawing.RectangleF(0,0,100, 50);
            //btn.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            btn.SetTitle("nav center", UIControlState.Normal);
            this.View.AddSubview(btn);

            this.NavigationItem.Title = "nav center";

            btn.TouchUpInside += delegate(object sender, EventArgs e) {
                this.Test();
                if (this.NavigationController != null)
                {
//                    this.NavigationController.PushViewController(new SubView(), true);
                }
            };
        }

        private void Test()
        {
            var leftController = new Left(); 
            var rightController = new Right();

            var centerController = new Center();
            centerController.Title = "Center 1";

            var deckController = new ViewDeckController(centerController, leftController, rightController);
            deckController.RightLedge = 100;

            deckController.NavigationControllerBehavior = ViewDeckNavigationControllerBehavior.Contained;

            this.NavigationController.PushViewController(deckController, true);
           // return deckController;

        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }

    }




    public class Center : UIViewController
    {
        public override void LoadView()
        {
            base.LoadView();
            this.View = new UIView();
            this.View.BackgroundColor = UIColor.Gray;
            var btn = new UIButton(UIButtonType.RoundedRect);
            btn.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            btn.SetTitle("hello", UIControlState.Normal);
            this.View.AddSubview(btn);

            this.NavigationItem.Title = "Center";

            btn.TouchUpInside += delegate(object sender, EventArgs e) {
                if (this.NavigationController != null)
                {
                    this.NavigationController.PushViewController(new SubView(), true);
                }
            };
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }

    public class Left : UIViewController
    {
        public override void LoadView()
        {
            base.LoadView();
            this.View = new UIView();
            this.View.BackgroundColor = UIColor.White;
            var btn = new UIButton(UIButtonType.RoundedRect);
            btn.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            btn.SetTitle("hello", UIControlState.Normal);
            this.View.AddSubview(btn);
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }

    public class Right : UIViewController
    {
        public override void LoadView()
        {
            base.LoadView();
            this.View = new UIView();
            this.View.BackgroundColor = UIColor.Blue;
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }

    public class SubView : UIViewController
    {
        public override void LoadView()
        {
            base.LoadView();
            this.View = new UIView();
            this.View.BackgroundColor = UIColor.Green;
            this.NavigationItem.Title = "Sub";

            var btn = new UIButton(UIButtonType.RoundedRect);
            btn.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            btn.SetTitle("hello", UIControlState.Normal);
            this.View.AddSubview(btn);

            btn.TouchUpInside += delegate(object sender, EventArgs e) {
                this.NavigationController.PopViewControllerAnimated(true);
            };

        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }

}

