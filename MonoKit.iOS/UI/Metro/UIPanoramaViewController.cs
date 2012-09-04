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

    // todo: smooth out the presentation animations

    public class UIPanoramaViewController : UIViewController
    {
        private readonly List<PanoramaItem> items;

        private readonly List<UIPanGestureRecognizer> panners;

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

        public UILabel TitleView { get; private set; } 

        public UIView ContentView { get; private set; } 

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

            this.hasAppeared = false;

            this.View = new UIView();

            this.BackgroundView = new UIView();
            this.BackgroundView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            this.View.AddSubview(this.BackgroundView);
            this.BackgroundView.BackgroundColor = UIColor.Black;

            this.TitleView = new UILabel();
            this.TitleView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            this.TitleView.BackgroundColor = UIColor.Clear;
            this.View.AddSubview(this.TitleView);

            this.ContentView = new UIView();
            this.ContentView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            this.View.AddSubview(this.ContentView);

            // mmm, this means that more than one needs to be added prior to loading ?? 
            if (this.ChildViewControllers.Count() > 1)
            {
                this.AddPanner(this.ContentView);
            }

            this.titleSize = this.ConfigureTitle(this.ShowTitle);
            this.headerTop = this.titleSize.Height + (this.ShowTitle ? 2f : 0); // todo: header margin
            this.headerHeight = this.CalculateHeaderHeight(this.ShowHeaders);
            this.contentTop = this.headerTop + this.headerHeight + (this.ShowHeaders ? 2f : 0);// todo: content margin top
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }
        
        /// <summary>
        /// Called when the controller’s view is released from memory. 
        /// </summary>
        public override void ViewDidUnload()
        {
            this.hasAppeared = false;

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

            this.InitViews();

            this.TitleView.Hidden = !this.ShowTitle;

            this.LayoutContent(this.currentScrolledOffset);

            if (this.ShadowEnabled)
            {
                foreach (var item in this.items)
                {
                    this.ApplyShadow(item.ContentView);
                }
            }

            if (this.presentedController != null)
            {
                this.Present(this.presentedController, false);
            }
        }
        
        private void InitViews()
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
                }

                if (item.Controller.View.Superview == null)
                {
                    item.Controller.View.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

                    item.ContentView = new UIView();
                    this.ContentView.AddSubview(item.ContentView);

                    item.ContentView.AddSubview(item.Controller.View);
                    item.Controller.View.Frame = item.ContentView.Bounds;
                }
            }
        }


        private UIViewController presentedController;

        public virtual void Present(UIViewController controller, bool animated)
        {
            this.AddChildViewController(controller);
            this.presentedController = controller;

            presentedController.View.Alpha = 0;
            presentedController.View.Frame = new RectangleF(this.ContentView.Bounds.Width, 0, this.ContentView.Bounds.Width, this.ContentView.Bounds.Height);

            if (this.ShadowEnabled)
            {
                // ? maybe this.ApplyShadow(presentedController.View);
            }

            this.View.InsertSubviewAbove(controller.View, this.ContentView);

            if (animated)
            {
                UIView.Animate(0.4f, 0, UIViewAnimationOptions.CurveEaseInOut | UIViewAnimationOptions.BeginFromCurrentState, () =>
                               {
                    this.ContentView.Frame = new RectangleF(0 - this.contentWidth + this.currentScrolledOffset, 0, this.ContentView.Bounds.Width, this.ContentView.Bounds.Height);
                    this.TitleView.Alpha = 0f;
                    presentedController.View.Alpha = 1f;
                    presentedController.View.Frame = this.View.Bounds;
                    
                }, () =>
                {
                    presentedController.DidMoveToParentViewController(this);
                });
            }
            else
            {
                this.ContentView.Frame = new RectangleF(0 - this.contentWidth + this.currentScrolledOffset, 0, this.ContentView.Bounds.Width, this.ContentView.Bounds.Height);
                this.TitleView.Alpha = 0f;
                presentedController.View.Alpha = 1f;
                presentedController.View.Frame = this.View.Bounds;
                presentedController.DidMoveToParentViewController(this);
            }
        }

        public virtual void Present(UIViewController controller)
        {
            this.Present(controller, true);
        }

        public virtual void Dismiss()
        {
            if (this.presentedController != null)
            {
                presentedController.WillMoveToParentViewController(null);

                UIView.Animate(0.4f, 0, UIViewAnimationOptions.CurveEaseInOut | UIViewAnimationOptions.BeginFromCurrentState, () =>
                {
                    this.TitleView.Alpha = 1f;
                    this.ContentView.Alpha = 1f;
                    presentedController.View.Frame = new RectangleF(this.ContentView.Bounds.Width, 0, this.ContentView.Bounds.Width, this.ContentView.Bounds.Height);
                    this.ContentView.Frame = new RectangleF(0, 0, this.ContentView.Bounds.Width, this.ContentView.Bounds.Height);
                    this.ContentView.Alpha = 1f;
                    presentedController.View.Alpha = 0f;
                }, () =>
                {
                    presentedController.View.RemoveFromSuperview();
                    presentedController.RemoveFromParentViewController();
                    this.presentedController = null;
                });
            }
        }

        protected virtual void ApplyShadow(UIView view)
        {
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = UIColor.Black.CGColor;
            view.Layer.ShadowRadius = 5;
            view.Layer.ShadowOffset = SizeF.Empty;
            view.Layer.ShadowOpacity = 0.5f;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
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

            if (Math.Abs(velocity) < 700)
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

        protected virtual float CalculateTitleOffset(float offset)
        {
            return this.Margin - (offset * (this.AnimateTitle ? this.titleRate : 0));
        }

        protected void LayoutContent()
        {
            this.LayoutContent(this.currentScrolledOffset);
        }

        private void LayoutContent(float offset)
        {
            this.LayoutBackgroundView(offset);

            if (this.Title != this.TitleView.Text)
            {
                var sz = this.View.StringSize(this.Title, this.TitleFont);
                this.TitleView.Text = this.Title;
                this.titleSize = new SizeF(sz.Width, this.titleSize.Height);
            }

            this.TitleView.Frame = new RectangleF(this.CalculateTitleOffset(offset), 0, this.titleSize.Width, this.titleSize.Height);

            foreach (var item in this.items)
            {
                item.HeaderView.Frame = new RectangleF(this.Margin + item.Origin.X - offset, this.headerTop, item.HeaderSize.Width, item.HeaderSize.Height);
                item.ContentView.Frame = new RectangleF(this.Margin + item.Origin.X - offset, item.Origin.Y, item.Size.Width, item.Size.Height);
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

