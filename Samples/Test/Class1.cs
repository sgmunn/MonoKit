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
using MonoTouch.UIKit;

namespace Test
{

    // todo: wrapped controllers need to return navigation controller and item from the wrapping controller



    public interface IWrappableController
    {
        void SetWrappingController(WrapController controller);
    }





    public class WrapController : UIViewController
    {
        private  UIViewController wrappedController;

        public WrapController(UIViewController wrappedController)
        {
            this.wrappedController = wrappedController;

            ((IWrappableController)wrappedController).SetWrappingController(this);
        }

        public float StatusBarHeight
        {
            get
            {
                switch (UIApplication.SharedApplication.StatusBarOrientation)
                {
                    case UIInterfaceOrientation.LandscapeLeft:
                    case UIInterfaceOrientation.LandscapeRight:
                        return UIApplication.SharedApplication.StatusBarFrame.Width;
                    default:
                        return UIApplication.SharedApplication.StatusBarFrame.Height;
                }
            }
        }



        public override void LoadView()
        {
            base.LoadView();
            this.AddChildViewController(this.wrappedController);
            var viewFrame = this.wrappedController.View.Frame; // todo: less status bar height
            this.View = new UIView(viewFrame);
            this.View.AutoresizingMask = this.wrappedController.View.AutoresizingMask;
            this.wrappedController.View.Frame = this.View.Bounds;
            this.wrappedController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            this.View.AddSubview(this.wrappedController.View);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
//            this.OnViewDidLoad(this);
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
            this.wrappedController.View.RemoveFromSuperview();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.wrappedController.RemoveFromParentViewController();
                ((IWrappableController)this.wrappedController).SetWrappingController(null);
                this.wrappedController = null;
            }
        }

        public override bool AutomaticallyForwardAppearanceAndRotationMethodsToChildViewControllers
        {
            get
            {
                return false;
            }
        }

        public override UITabBarItem TabBarItem
        {
            get
            {
                return this.wrappedController.TabBarItem;
            }

            set
            {
                this.wrappedController.TabBarItem = value;
            }
        }

        public override bool HidesBottomBarWhenPushed
        {
            get
            {
                return this.wrappedController.HidesBottomBarWhenPushed;
            }

            set
            {
                this.wrappedController.HidesBottomBarWhenPushed = value;
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
//            this.OnViewWillAppear(this, animated);

            this.wrappedController.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
//            this.OnViewDidAppear(this, animated);

            this.wrappedController.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
//            this.OnViewWillDisappear(this, animated);

            this.wrappedController.ViewWillDisappear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
//            this.OnViewDidDisappear(this, animated);

            this.wrappedController.ViewDidDisappear(animated);
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return this.wrappedController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation);
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);
            this.wrappedController.WillAnimateRotation(toInterfaceOrientation, duration);
        }

        public override void WillAnimateFirstHalfOfRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateFirstHalfOfRotation(toInterfaceOrientation, duration);
            this.wrappedController.WillAnimateFirstHalfOfRotation(toInterfaceOrientation, duration);
        }

        public override void WillAnimateSecondHalfOfRotation(UIInterfaceOrientation fromInterfaceOrientation, double duration)
        {
            base.WillAnimateSecondHalfOfRotation(fromInterfaceOrientation, duration);
            this.wrappedController.WillAnimateSecondHalfOfRotation(fromInterfaceOrientation, duration);
        }

        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);
            this.wrappedController.WillRotate(toInterfaceOrientation, duration);
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            this.wrappedController.DidRotate(fromInterfaceOrientation);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            this.wrappedController.DidReceiveMemoryWarning();
        }
    }
}

