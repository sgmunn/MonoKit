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

        /// <summary>
        /// The space between the title and content items, and the left edge of the view  
        /// </summary>
        private float leftMargin = PanoramaConstants.LeftMargin;

        private float currentScrolledOffset;
        private bool hasAppeared;
        private SizeF titleSize;
        private float headerHeight;
        private float contentWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.Metro.UIPanoramaViewController"/> class.
        /// </summary>
        public UIPanoramaViewController()
        {
            this.items = new List<PanoramaItem>();
            this.panners = new List<UIPanGestureRecognizer>();
            this.Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.Metro.UIPanoramaViewController"/> class.
        /// </summary>
        /// <param name='handle'>Handle to the underlying UIKit object</param>
        public UIPanoramaViewController(IntPtr handle) : base(handle)
        {
            this.items = new List<PanoramaItem>();
            this.panners = new List<UIPanGestureRecognizer>();
            this.Init();
        }
                        
        public UIFont TitleFont { get; set; }

        public UIFont HeaderFont { get; set; }

        public UIColor TextColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to animate the background when the user pans the panorama
        /// </summary>
        /// <remarks>Should only be set prior to the view being loaded</remarks>
        //public bool AnimateBackground { get; set; }


        public float BackgroundMotionRate { get; set; }

                
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
                this.CalculateItemMetrics(this.titleSize.Height + this.headerHeight + 2);
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

            this.AddPanner(this.ContentView);

            this.titleSize = this.ConfigureTitle();
            this.headerHeight = this.CalculateHeaderHeight();
        }

        
        /// <summary>
        /// Called when the controller’s view is released from memory. 
        /// </summary>
        public override void ViewDidUnload()
        {
            this.hasAppeared = false;
            Console.WriteLine("panorama unload");

            foreach (var item in this.items)
            {
                item.Controller.View.RemoveFromSuperview();
                item.LabelView.RemoveFromSuperview();
            }

            this.panners.Clear();

            base.ViewDidUnload();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            this.hasAppeared = true;

            this.CalculateItemMetrics(this.titleSize.Height + this.headerHeight + 2);

            this.LayoutContent(this.currentScrolledOffset);
            Console.WriteLine("panorama appear");
        }

        private UIViewController presentedController;

        public void Present(UIViewController controller)
        {
            this.AddChildViewController(controller);
            this.presentedController = controller;

            presentedController.View.Alpha = 0;
            presentedController.View.Frame = this.View.Bounds;
            this.View.AddSubview(controller.View);

            var scale1 = MonoTouch.CoreGraphics.CGAffineTransform.MakeScale(0.8f, 0.8f);
            var translate1 = MonoTouch.CoreGraphics.CGAffineTransform.MakeTranslation(100, 0);

            presentedController.View.Transform = MonoTouch.CoreGraphics.CGAffineTransform.Multiply(scale1, translate1);

            UIView.Animate(0.4f, 0, UIViewAnimationOptions.CurveEaseInOut | UIViewAnimationOptions.BeginFromCurrentState, () =>
            {
                var scale = MonoTouch.CoreGraphics.CGAffineTransform.MakeScale(1.2f, 1.2f);
                var translate = MonoTouch.CoreGraphics.CGAffineTransform.MakeTranslation(-100, 0);

                this.ContentView.Transform = MonoTouch.CoreGraphics.CGAffineTransform.Multiply(scale, translate);
                this.ContentView.Alpha = 0;

                this.TitleView.Alpha = 0;

                presentedController.View.Alpha = 1f;
                presentedController.View.Transform = MonoTouch.CoreGraphics.CGAffineTransform.MakeIdentity();

            }, () =>
            {
                presentedController.DidMoveToParentViewController(this);
            });
        }

        public void Dismiss()
        {
            if (this.presentedController != null)
            {
                presentedController.WillMoveToParentViewController(null);
                    


                UIView.Animate(0.5f, 0, UIViewAnimationOptions.CurveEaseInOut | UIViewAnimationOptions.BeginFromCurrentState, () =>
                {
                    this.TitleView.Alpha = 1;

                    this.ContentView.Transform = MonoTouch.CoreGraphics.CGAffineTransform.MakeIdentity();
                    this.ContentView.Alpha = 1;

                    var scale1 = MonoTouch.CoreGraphics.CGAffineTransform.MakeScale(0.8f, 0.8f);
                    var translate1 = MonoTouch.CoreGraphics.CGAffineTransform.MakeTranslation(100, 0);

                    presentedController.View.Transform = MonoTouch.CoreGraphics.CGAffineTransform.Multiply(scale1, translate1);
                    presentedController.View.Alpha = 0;
                }, () =>
                {
                    presentedController.View.RemoveFromSuperview();
                    presentedController.RemoveFromParentViewController();
                    this.presentedController = null;
                });
            }
        }
        
        private void Init()
        {
            this.currentScrolledOffset = 0;

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
            this.TitleView.Frame = new RectangleF(this.leftMargin - (offset * this.titleRate), 0, this.titleSize.Width, this.titleSize.Height);
        }

        private void LayoutBackgroundView(float offset)
        {
            var left = 0 - (offset * this.BackgroundMotionRate);

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
                if (item.LabelView == null)
                {
                    item.LabelView = new UILabel();
                    item.LabelView.Font = this.HeaderFont;
                    item.LabelView.TextColor = this.TextColor;
                    item.LabelView.Text = item.Controller.Title;
                    item.LabelView.BackgroundColor = UIColor.Clear;

                    this.ContentView.AddSubview(item.LabelView);
                }

                item.LabelView.Frame = new RectangleF(PanoramaConstants.LeftMargin + item.Origin.X - offset, this.titleSize.Height, item.Size.Width, this.headerHeight);
            }
        }

        private void LayoutItemsInContent(float offset)
        {
            foreach (var item in this.items)
            {
                if (item.Controller.View.Superview == null)
                {
                    this.ContentView.AddSubview(item.Controller.View);
                }

                item.Controller.View.Frame = new RectangleF(PanoramaConstants.LeftMargin + item.Origin.X - offset, item.Origin.Y, item.Size.Width, item.Size.Height);
            
            
            }
        }

        private SizeF ConfigureTitle()
        {
            this.TitleView.Font = this.TitleFont;
            this.TitleView.TextColor = this.TextColor;
            this.TitleView.Text = this.Title;

            return this.View.StringSize(this.Title, this.TitleFont);
        }

        private float CalculateHeaderHeight()
        {
            return this.View.StringSize(this.Title, this.HeaderFont).Height;
        }

        private void CalculateItemMetrics(float top)
        {
            float left = 0;

            foreach (var item in this.items)
            {
                item.Origin = new PointF(left, top);
                item.Size = new SizeF(item.GetWidth(this.View.Bounds.Width - (2 * PanoramaConstants.LeftMargin)), this.View.Bounds.Height - top - 50);
                left += item.Size.Width + PanoramaConstants.LeftMargin + PanoramaConstants.LeftMargin;
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

