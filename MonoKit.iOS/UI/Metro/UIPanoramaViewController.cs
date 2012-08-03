//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="UIPanoramaViewController.cs" company="sgmunn">
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

namespace MonoKit.Metro
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using MonoTouch.UIKit;

    // todo: optimise the shadows.  layout the items without the content controllers. snapshot the 
    // content view and add as a shadow view.  add to content and then turn off shadows

    /// <summary>
    /// Controller for panorama views
    /// </summary>
    public class UIPanoramaViewController : UIViewController
    {
        private readonly List<PanoramaItem> items;

        private readonly List<UIPanGestureRecognizer> panners;

        /// <summary>
        /// The rate at which the title slides in relation to the content
        /// </summary>
        private float titleRate;

        private float currentScrolledOffset;

        private bool hasAppeared;

        private SizeF titleSize;

        private float headerHeight;

        private float contentWidth;

        private float headerTop;

        private float contentTop;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.Metro.UIPanoramaViewController"/> class.
        /// </summary>
        public UIPanoramaViewController()
        {
            this.items = new List<PanoramaItem>();
            this.panners = new List<UIPanGestureRecognizer>();
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.Metro.UIPanoramaViewController"/> class.
        /// </summary>
        /// <param name='handle'>Handle to the underlying UIKit object</param>
        public UIPanoramaViewController(IntPtr handle) : base(handle)
        {
            this.items = new List<PanoramaItem>();
            this.panners = new List<UIPanGestureRecognizer>();
            this.Initialize();
        }
                        

        public UIColor TextColor { get; set; }

        public bool ShowTitle { get; set; }

        public bool AnimateTitle { get; set; }

        public UIFont TitleFont { get; set; }


        public bool ShowHeaders { get; set; }

        public UIFont HeaderFont { get; set; }


        public bool AnimateBackground { get; set; }

        public float BackgroundMotionRate { get; set; }

        public bool ShadowEnabled { get; set; }

        public float PreviewSize { get; set; }

        public float BottomMargin { get; set; }

        public float Margin { get; set; }

                
        /// <summary>
        /// Gets the title view
        /// </summary>
        public UILabel TitleView { get; private set; } 

        /// <summary>
        /// Gets the content view
        /// </summary>
        public UIView ContentView { get; private set; } 

        /// <summary>
        /// Gets the background view
        /// </summary>
        public UIView BackgroundView { get; private set; } 




        public void AddController(UIViewController controller)
        {
            this.AddController(controller, 0);
        }

        public void AddController(UIViewController controller, float width)
        {
            this.AddChildViewController(controller);

            this.items.Add(new PanoramaItem(controller, width));

            if (this.hasAppeared)
            {
                this.CalculateItemMetrics(this.contentTop);
                this.LayoutContent(this.currentScrolledOffset);
            }

            controller.DidMoveToParentViewController(this);
        }

        /// <summary>
        /// Loads the view.
        /// </summary>
        public override void LoadView()
        {
            base.LoadView();
            Console.WriteLine("panorama load");

            this.hasAppeared = false;

            this.View = new UIView();

            this.BackgroundView = new UIView();
            this.BackgroundView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            this.View.AddSubview(this.BackgroundView);
            this.BackgroundView.BackgroundColor = UIColor.Black;

            this.ContentView = new UIView();
            this.ContentView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            this.View.AddSubview(this.ContentView);

            this.TitleView = new UILabel();
            this.TitleView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            this.TitleView.BackgroundColor = UIColor.Clear;
            this.View.AddSubview(this.TitleView);

            if (this.ShadowEnabled && this.ShouldApplyShadow(this.TitleView))
            {
                this.ApplyShadow(this.TitleView);
            }

            this.AddPanner(this.ContentView);

            this.titleSize = this.ConfigureTitle(this.ShowTitle);
            this.headerTop = this.titleSize.Height + (this.ShowTitle ? 2f : 0); // todo: header margin
            this.headerHeight = this.CalculateHeaderHeight(this.ShowHeaders);
            this.contentTop = this.headerTop + this.headerHeight + (this.ShowHeaders ? 2f : 0);// todo: content margin top
        }

        
        /// <summary>
        /// Called when the controller’s view is released from memory. 
        /// </summary>
        public override void ViewDidUnload()
        {
            this.hasAppeared = false;
            Console.WriteLine("panorama unload");

            // todo: retest with content views
            foreach (var item in this.items)
            {
                item.Controller.View.RemoveFromSuperview();
                item.HeaderView.RemoveFromSuperview();
            }

            this.panners.Clear();

            base.ViewDidUnload();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            this.hasAppeared = true;

            this.CalculateItemMetrics(this.contentTop);

            this.LayoutContent(this.currentScrolledOffset);
            Console.WriteLine("panorama appear");
        }

        private UIViewController presentedController;

        public void Present(UIViewController controller)
        {
            this.AddChildViewController(controller);
            this.presentedController = controller;


            // todo: make content view contain headers
            // make content view sit underneath title.
            // snapshot a copy of the content view and animate that, not the content view
            // then the tables will appear to fade correctly

            presentedController.View.Alpha = 0;
            presentedController.View.Frame = new RectangleF(this.ContentView.Bounds.Width, 0, this.ContentView.Bounds.Width, this.ContentView.Bounds.Height);
            this.View.InsertSubviewBelow(controller.View, this.ContentView);



            UIView.Animate(0.3f, 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.BeginFromCurrentState, () =>
            {
                    presentedController.View.Alpha = 0.4f;
                    presentedController.View.Frame = new RectangleF(this.ContentView.Bounds.Width - 100, 0, this.ContentView.Bounds.Width, this.ContentView.Bounds.Height);

                    this.TitleView.Alpha = 0.3f;
                    this.ContentView.Alpha = 0.7f;

            }, () =>
            {
                UIView.Animate(0.3f, 0, UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.BeginFromCurrentState, () =>
                {
                    this.TitleView.Alpha = 0f;
                    this.ContentView.Alpha = 0f;
                    presentedController.View.Alpha = 1f;
                    presentedController.View.Frame = this.View.Bounds;

                }, () =>
                {
                    presentedController.DidMoveToParentViewController(this);
                });
            });





//            var scale1 = MonoTouch.CoreGraphics.CGAffineTransform.MakeScale(0.9f, 0.9f);
//            var translate1 = MonoTouch.CoreGraphics.CGAffineTransform.MakeTranslation(30, this.headerHeight);
//
//            presentedController.View.Transform = MonoTouch.CoreGraphics.CGAffineTransform.Multiply(scale1, translate1);
//
//            UIView.Animate(0.3f, 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.BeginFromCurrentState, () =>
//            {
//                var scale = MonoTouch.CoreGraphics.CGAffineTransform.MakeScale(1.1f, 1.1f);
//                var translate = MonoTouch.CoreGraphics.CGAffineTransform.MakeTranslation(-100, -this.headerHeight);
//
//                this.ContentView.Transform = MonoTouch.CoreGraphics.CGAffineTransform.Multiply(scale, translate);
//                this.ContentView.Alpha = 0;
//
//                this.TitleView.Alpha = 0.3f;
//
//                //presentedController.View.Alpha = 1f;
//                //presentedController.View.Transform = MonoTouch.CoreGraphics.CGAffineTransform.MakeIdentity();
//
//            }, () =>
//            {
//                UIView.Animate(0.3f, 0, UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.BeginFromCurrentState, () =>
//                {
//                    //var scale = MonoTouch.CoreGraphics.CGAffineTransform.MakeScale(1.1f, 1.1f);
//                    //var translate = MonoTouch.CoreGraphics.CGAffineTransform.MakeTranslation(-100, -70);
//
//                    //this.ContentView.Transform = MonoTouch.CoreGraphics.CGAffineTransform.Multiply(scale, translate);
//                    //this.ContentView.Alpha = 0;
//
//                    this.TitleView.Alpha = 0;
//
//                    presentedController.View.Alpha = 1f;
//                    presentedController.View.Transform = MonoTouch.CoreGraphics.CGAffineTransform.MakeIdentity();
//
//                }, () =>
//                {
//                    presentedController.DidMoveToParentViewController(this);
//                });
//            });
        }

        public void Dismiss()
        {
            if (this.presentedController != null)
            {
                presentedController.WillMoveToParentViewController(null);
                    

                UIView.Animate(0.3f, 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.BeginFromCurrentState, () =>
                {
                    presentedController.View.Alpha = 0.7f;
                    presentedController.View.Frame = new RectangleF(this.ContentView.Bounds.Width - 100, 0, this.ContentView.Bounds.Width, this.ContentView.Bounds.Height);

                    this.TitleView.Alpha = 0.5f;
                    this.ContentView.Alpha = 0.3f;

                }, () =>
                {
                    UIView.Animate(0.3f, 0, UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.BeginFromCurrentState, () =>
                    {
                        this.TitleView.Alpha = 1f;
                        this.ContentView.Alpha = 1f;
                        presentedController.View.Alpha = 0f;
                        presentedController.View.Frame = new RectangleF(this.ContentView.Bounds.Width, 0, this.ContentView.Bounds.Width, this.ContentView.Bounds.Height);

                    }, () =>
                    {
                        presentedController.View.RemoveFromSuperview();
                        presentedController.RemoveFromParentViewController();
                        this.presentedController = null;
                    });
                });


//                UIView.Animate(0.3f, 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.BeginFromCurrentState, () =>
//                {
//                    this.TitleView.Alpha = 0.3f;
//
//                    //this.ContentView.Transform = MonoTouch.CoreGraphics.CGAffineTransform.MakeIdentity();
//                    //this.ContentView.Alpha = 1;
//
//                    var scale1 = MonoTouch.CoreGraphics.CGAffineTransform.MakeScale(0.9f, 0.9f);
//                    var translate1 = MonoTouch.CoreGraphics.CGAffineTransform.MakeTranslation(30, this.headerHeight);
//
//                    presentedController.View.Transform = MonoTouch.CoreGraphics.CGAffineTransform.Multiply(scale1, translate1);
//                    presentedController.View.Alpha = 0;
//                }, () =>
//                {
//                    UIView.Animate(0.3f, 0, UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.BeginFromCurrentState, () =>
//                    {
//                        this.TitleView.Alpha = 1;
//
//                        this.ContentView.Transform = MonoTouch.CoreGraphics.CGAffineTransform.MakeIdentity();
//                        this.ContentView.Alpha = 1;
//
//                        //var scale1 = MonoTouch.CoreGraphics.CGAffineTransform.MakeScale(0.8f, 0.9f);
//                        //var translate1 = MonoTouch.CoreGraphics.CGAffineTransform.MakeTranslation(200, 0);
//                        //
//                        //presentedController.View.Transform = MonoTouch.CoreGraphics.CGAffineTransform.Multiply(scale1, translate1);
//                        //presentedController.View.Alpha = 0;
//                    }, () =>
//                    {
//                        presentedController.View.RemoveFromSuperview();
//                        presentedController.RemoveFromParentViewController();
//                        this.presentedController = null;
//                    });
//                });
            }
        }

        protected virtual void ApplyShadow(UIView view)
        {
            view.Layer.MasksToBounds = false;
            //view.Layer.ShadowRadius = 5;
            //view.Layer.ShadowOffset = new SizeF(5,5);
            view.Layer.ShadowOpacity = 0.5f;
        }

        protected virtual bool ShouldApplyShadow(UIView view)
        {
            if (!this.AnimateTitle)
            {
                return view != this.TitleView;
            }

            return true;
        }
        
        private void Initialize()
        {
            this.ShowTitle = true;
            this.ShowHeaders = true;
            this.ShadowEnabled = true;
            this.AnimateTitle = true;

            this.currentScrolledOffset = 0;

            this.BottomMargin = PanoramaConstants.Margin;
            this.Margin = PanoramaConstants.Margin;
            this.PreviewSize = PanoramaConstants.NextContentItemPreviewSize;
            this.TitleFont = UIFont.FromName(PanoramaConstants.DefaultFontName, PanoramaConstants.DefaultTitleFontSize);
            this.TextColor = PanoramaConstants.DefaultTextColor;
            this.HeaderFont = UIFont.FromName(PanoramaConstants.DefaultFontName, PanoramaConstants.DefaultHeaderFontSize);

            this.BackgroundMotionRate = PanoramaConstants.DefaultBackgroundMotionRate;
        }

        private void AddPanner(UIView view) 
        {
            if (view == null) return;

            UIPanGestureRecognizer panner = new UIPanGestureRecognizer(this.Panned);

            this.View.AddGestureRecognizer(panner);
            this.panners.Add(panner);
        }

        private float LimitOffset(float offset)
        {
            if (offset < 0)
            {
                return offset / 2;
            }
            else
            {
                var pos = this.items[this.items.Count - 1].Origin.X;
                if (offset > pos)
                {
                    var delta = offset - pos;
                    return pos + (delta / 2);
                }
            }

            return offset;
        }

        private void Panned(UIPanGestureRecognizer gestureRecognizer)
        {
            if (this.items.Count < 2 || this.contentWidth <= this.View.Bounds.Width)
            {
                return;
            }

            var panLocation = gestureRecognizer.TranslationInView(this.ContentView);

            var offset = this.currentScrolledOffset - panLocation.X;

            // this is the total amount that we've scrolled away from zero
            this.LayoutContent(this.LimitOffset(offset));

            if (gestureRecognizer.State != UIGestureRecognizerState.Ended)
            {
                return;
            }

            var velocity = gestureRecognizer.VelocityInView(this.View).X;

            if (Math.Abs(velocity) < 500)
            {
                this.currentScrolledOffset = this.CalculatePannedLocation(offset, panLocation.X);
                this.ScrollContent(currentScrolledOffset);
            }
            else
            {
                if (panLocation.X < 0)
                {
                    var proposedOffset = this.GetNextOffset(offset);

                    this.currentScrolledOffset = proposedOffset;
                }
                else
                {
                    var proposedOffset = this.GetPriorOffset(offset);

                    this.currentScrolledOffset = proposedOffset;
                }

                this.ScrollContent(this.currentScrolledOffset);
            }

            gestureRecognizer.SetTranslation(PointF.Empty, this.View);
        }

        private float CalculatePannedLocation(float currentOffset, float movement)
        {
            if (movement < 0)
            {
                var left = this.currentScrolledOffset;
                var right = this.GetNextOffset(this.currentScrolledOffset);

                if ((right - currentOffset) < (currentOffset - left) )
                {
                    return right;
                }

                return left;
            }
            else
            {
                var left = this.GetPriorOffset(this.currentScrolledOffset);
                var right = this.currentScrolledOffset;

                if ((right - currentOffset) < (currentOffset - left) )
                {
                    return right;
                }

                return left;

            }
        }

        private float GetNextOffset(float currentOffset)
        {
            var nextItems = from item in this.items.Where(v => v.Origin.X > currentOffset)
                select item;

            var nextItem = nextItems.FirstOrDefault();
            if (nextItem != null)
            {
                return nextItem.Origin.X;
            }

            return this.items.Last().Origin.X;
        }

        private float GetPriorOffset(float currentOffset)
        {
            var nextItems = from item in this.items.Where(v => v.Origin.X < currentOffset)
                select item;

            var nextItem = nextItems.LastOrDefault();
            if (nextItem != null)
            {
                return nextItem.Origin.X;
            }

            return 0;
        }

        private void ScrollContent(float offset)
        {

            UIView.Animate(0.2f, 0, UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.BeginFromCurrentState, () =>
            {
                this.LayoutContent(offset);
            }, () =>
            {
                //if (completed != null) completed(this);
                //if (callDelegate && this.Delegate != null) 
                //{
                //    this.Delegate.DidOpenLeftView(this, animated);
                //}
            });

        }

        private void LayoutContent(float offset)
        {
            this.LayoutTitleView(offset);
            this.LayoutBackgroundView(offset);
            this.LayoutItemTitlesInContent(offset);
            this.LayoutItemsInContent(offset);
        }


        private void LayoutTitleView(float offset)
        {
            if (this.ShowTitle)
            {
                this.TitleView.Hidden = false;
                this.TitleView.Frame = new RectangleF(this.Margin - (offset * (this.AnimateTitle ? this.titleRate : 0)), 0, this.titleSize.Width, this.titleSize.Height);
            }
            else
            {
                this.TitleView.Hidden = true;
            }
        }

        private void LayoutBackgroundView(float offset)
        {
            float left = 0 - (this.AnimateBackground ? (offset * this.BackgroundMotionRate) : 0);

            if (left > 0)
            {
                left = 0;
            }

            this.BackgroundView.Frame = new RectangleF(left, 0, this.View.Bounds.Width, this.View.Bounds.Height);
        }


        private void LayoutItemTitlesInContent(float offset)
        {
            foreach (var item in this.items)
            {
                if (item.HeaderView == null)
                {
                    item.HeaderView = new UILabel();
                    item.HeaderView.Font = this.HeaderFont;
                    item.HeaderView.TextColor = this.TextColor;
                    item.HeaderView.Text = item.Controller.Title;
                    item.HeaderView.BackgroundColor = UIColor.Clear;

                    this.ContentView.AddSubview(item.HeaderView);

                    if (this.ShadowEnabled && this.ShouldApplyShadow(item.HeaderView))
                    {
                        this.ApplyShadow(item.HeaderView);
                    }
                }

                item.HeaderView.Frame = new RectangleF(this.Margin + item.Origin.X - offset, this.headerTop, item.HeaderSize.Width, item.HeaderSize.Height);
            }
        }

        private void LayoutItemsInContent(float offset)
        {
            foreach (var item in this.items)
            {
                if (item.Controller.View.Superview == null)
                {
                    item.Controller.View.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

                    item.ContentView = new UIView();
                    this.ContentView.AddSubview(item.ContentView);

                    item.ContentView.AddSubview(item.Controller.View);
                    item.Controller.View.Frame = item.ContentView.Bounds;

                    if (this.ShadowEnabled && this.ShouldApplyShadow(item.ContentView))
                    {
                        this.ApplyShadow(item.ContentView);
                    }
                }

                item.ContentView.Frame = new RectangleF(this.Margin + item.Origin.X - offset, item.Origin.Y, item.Size.Width, item.Size.Height);
            
            
            }
        }

        private SizeF ConfigureTitle(bool shouldShow)
        {
            if (!shouldShow)
            {
                return new SizeF(0, 0);
            }

            this.TitleView.Font = this.TitleFont;
            this.TitleView.TextColor = this.TextColor;
            this.TitleView.Text = this.Title;

            return this.View.StringSize(this.Title, this.TitleFont);
        }

        private float CalculateHeaderHeight(bool shouldShow)
        {
            return shouldShow ? this.View.StringSize(this.Title, this.HeaderFont).Height : 0;
        }

        private void CalculateItemMetrics(float top)
        {
            float left = 0;

            foreach (var item in this.items)
            {
                item.Origin = new PointF(left, top);
                item.Size = new SizeF(item.GetWidth(this.View.Bounds.Width - (2 * this.Margin), this.PreviewSize), this.View.Bounds.Height - top - this.BottomMargin);
                left += item.Size.Width + this.Margin;
                item.HeaderSize = new SizeF(item.Size.Width, this.headerHeight);
            }
            
            var totalWidth = left;
            // round up to nearest multiple of frame width
            var x = Math.Truncate(totalWidth / this.View.Frame.Width);
            if (x * this.View.Frame.Width < totalWidth) 
            {
                x++;
            }

            totalWidth = (float)(x * this.View.Frame.Width);
            this.contentWidth = totalWidth;

            this.titleRate = 0;
            if (totalWidth != 0 && this.titleSize.Width != 0 && this.items.Count > 1)
            {
               // this.backgroundRate = titleWidth / w; 

                // title should disappear just before the last page
                // titleWidth * rate = (totalWidth - titleWidth)
                // rate = (totalWidth - titleWidth) / titleWidth

                this.titleRate = 1 / ((totalWidth - this.titleSize.Width) / this.titleSize.Width);
            }
        }
    }
}

