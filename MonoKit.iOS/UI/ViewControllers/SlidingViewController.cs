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
using System.Drawing;

namespace MonoKit.ViewControllers
{
    public class SlidingViewController : UIViewController
    {
        public SlidingViewController()
        {
        }

        public bool ShowingMasterViewController
        {
            get;
            private set;
        }

        public UIViewController MasterViewController
        {
            get;
            set;
        }

        public UIViewController DetailViewController
        {
            get;
            set;
        }

        public void SetShowingMasterViewController(bool flag)
        {
            this.SetShowingMasterViewController(flag, false, null);
        }

        public void SetShowingMasterViewController(bool flag, bool animate, Action<bool> onCompletion)
        {
            if (this.ShowingMasterViewController == flag)
            {
                if (onCompletion != null)
                {
                    onCompletion(false);
                }
            }

            this.ShowingMasterViewController = flag;

            double duration = animate ? 0.3 : 0;


            // oncompletion doesn't have a parameter
            UIView.Animate(
                duration, 
                0,
                UIViewAnimationOptions.BeginFromCurrentState | UIViewAnimationOptions.AllowUserInteraction, 
                () => this.LayoutViews(),
                () => {
                if (onCompletion != null)
                {
                    onCompletion(true);
                }});
        }

        private void SetMasterViewController(UIViewController master)
        {
            this.SetMasterViewController(master, false, null);
        }

        public void SetMasterViewController(UIViewController master, bool animate, Action<bool> onCompletion)
        {
            if (this.MasterViewController == master)
            {
                if (onCompletion != null)
                {
                    onCompletion(false);
                }
            }

            if (this.MasterViewController != null)
            {
                this.MasterViewController.WillMoveToParentViewController(null);
                this.MasterViewController.RemoveFromParentViewController();
                this.MasterViewController.View.RemoveFromSuperview();
            }

            this.MasterViewController = master;

            if (this.MasterViewController != null)
            {
                this.AddChildViewController(this.MasterViewController);
                this.ConfigureMasterView(this.MasterViewController.View);
                this.View.AddSubview(this.MasterViewController.View);
                this.MasterViewController.DidMoveToParentViewController(this);

            }

            this.LayoutViews();

            if (onCompletion != null)
            {
                onCompletion(true);
            }
        }

        public void SetDetailViewController(UIViewController detail, bool animate, Action<bool> onCompletion)
        {
            if (this.DetailViewController == detail)
            {
                if (onCompletion != null)
                {
                    onCompletion(false);
                }
            }

            if (this.DetailViewController != null)
            {
                this.DetailViewController.WillMoveToParentViewController(null);
                this.DetailViewController.RemoveFromParentViewController();
                this.DetailViewController.View.RemoveFromSuperview();
            }

            this.DetailViewController = detail;

            if (this.DetailViewController != null)
            {
                this.AddChildViewController(this.DetailViewController);
                this.ConfigureMasterView(this.DetailViewController.View);
                this.View.AddSubview(this.DetailViewController.View);
                this.DetailViewController.DidMoveToParentViewController(this);

            }

            this.LayoutViews();

            if (onCompletion != null)
            {
                onCompletion(true);
            }
        }

        public RectangleF RectForMasterView
        {
            get
            {
                return this.View.Bounds;
            }
        }

        public RectangleF RectForDetailView
        {
            get
            {
                if (this.ShowingMasterViewController)
                {
                    return new RectangleF(this.View.Bounds.X + 200, this.View.Bounds.Y, this.View.Bounds.Width, this.View.Bounds.Height);
                }

                return this.View.Bounds;
            }
        }


        public PointF DetailViewTranslationForGestureTranslation(PointF translation)
        {
            return new PointF(translation.X, 0);
        }

        public bool ShouldShowMasterViewControllerWithGestureTranslation(PointF translation)
        {
            if (this.ShowingMasterViewController && translation.X > 0)
            {
                return true;
            }

            if (this.ShowingMasterViewController && translation.X < 0)
            {
                return false;
            }

            return this.ShowingMasterViewController;
        }

        private UIPanGestureRecognizer panGestureRecognizer;

        public UIPanGestureRecognizer PanGestureRecognizer
        {
            get
            {
                if (this.panGestureRecognizer == null)
                {
                    this.panGestureRecognizer = new UIPanGestureRecognizer(this.HandlePan);
                    //this.panGestureRecognizer.AddTarget(this, this.HandlePan);
                    this.panGestureRecognizer.WeakDelegate = this;
                    this.panGestureRecognizer.CancelsTouchesInView = true;
                }

                return this.panGestureRecognizer;
            }
        }

        private UITapGestureRecognizer tapGestureRecognizer;

        public UITapGestureRecognizer TapGestureRecognizer
        {
            get
            {
                if (this.tapGestureRecognizer == null)
                {
                    this.tapGestureRecognizer = new UITapGestureRecognizer(this.HandleTap);
                    //this.tapGestureRecognizer.AddTarget(this, () => this.HandleTap());
                    this.tapGestureRecognizer.WeakDelegate = this;
                    this.tapGestureRecognizer.CancelsTouchesInView = true;
                }

                return this.tapGestureRecognizer;
            }
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            if (this.MasterViewController == null || this.DetailViewController == null)
            {
                return true;
            }

            return this.MasterViewController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation)
                && this.DetailViewController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation);
        }

        private void HandlePan()
        {

        }

        private void HandleTap()
        {
            UIView.Animate(0.25, 0, UIViewAnimationOptions.CurveEaseInOut, 
                           () =>
                           {
                this.SetShowingMasterViewController(false);
                this.View.LayoutSubviews();
                this.LayoutViews();
            }, null

            );
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.View.AddGestureRecognizer(this.PanGestureRecognizer);
            this.View.AddGestureRecognizer(this.TapGestureRecognizer);
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
            this.panGestureRecognizer = null;
            this.tapGestureRecognizer = null;
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            this.LayoutViews();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            this.LayoutViews();
        }

        
        private void LayoutViews()
        {
            var masterView = this.MasterViewController.View;
            var detailView = this.DetailViewController.View;
            masterView.Frame = this.RectForMasterView;
            detailView.Frame = this.RectForDetailView;

            detailView.Superview.BringSubviewToFront(detailView);
            detailView.UserInteractionEnabled = !this.ShowingMasterViewController;
        }
        
        protected virtual void ConfigureMasterView(UIView view)
        {

        }

        protected virtual void ConfigureDetailView(UIView view)
        {

        }

    }
}

