//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="Menu.cs" company="sgmunn">
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
//  Created by Levey on 11/30/11.
//  Copyright (c) 2011 Levey & Other Contributors. All rights reserved.
//  https://github.com/levey/AwesomeMenu

namespace MonoKit.UI.AwesomeMenu
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using MonoTouch.Foundation;
    using MonoTouch.CoreAnimation;
    using MonoTouch.CoreGraphics;
    using MonoTouch.ObjCRuntime;
    using MonoTouch.UIKit;

    public enum LayoutMode
    {
        Radial,
        Vertical,
        Horizontal
    }

    public sealed class Menu : UIView
    {
        private readonly List<MenuItem> menuItems;
        private int flag;
        private NSTimer timer;
        private MenuItem addButton;
        private bool isAnimating;
        private bool expanded;
        private PointF startPoint;

        public Menu(RectangleF frame, UIImage image, UIImage highlightImage, UIImage contentImage, UIImage highlightContentImage) : base(frame)
        {
            this.BackgroundColor = UIColor.Clear;
            this.Mode = LayoutMode.Radial;
            this.NearRadius = MenuDefaults.NearRadius;
            this.EndRadius = MenuDefaults.EndRadius;
            this.FarRadius = MenuDefaults.FarRadius;
            this.TimeOffset = MenuDefaults.TimeOffset;
            this.RotateAngle = MenuDefaults.RotateAngle;
            this.MenuWholeAngle = (float)MenuDefaults.MenuWholeAngle;
            this.StartPoint = new PointF(MenuDefaults.StartPointX, MenuDefaults.StartPointY);
            this.ExpandRotation = (float)MenuDefaults.ExpandRotation;
            this.CloseRotation = (float)MenuDefaults.CloseRotation;

            this.menuItems = new List<MenuItem>();

            this.addButton = new MenuItem(image, highlightImage, contentImage, highlightContentImage);

            this.addButton.Center = this.StartPoint;
            this.addButton.Touched += (sender, e) => this.Expanded = !this.Expanded;

            this.AddSubview(this.addButton);
        }


        public Menu(RectangleF frame, UIImage image, UIImage highlightImage, UIImage contentImage, UIImage highlightContentImage, IEnumerable<MenuItem> menus) : base(frame)
        {
            this.BackgroundColor = UIColor.Clear;
            this.Mode = LayoutMode.Radial;
            this.NearRadius = MenuDefaults.NearRadius;
            this.EndRadius = MenuDefaults.EndRadius;
            this.FarRadius = MenuDefaults.FarRadius;
            this.TimeOffset = MenuDefaults.TimeOffset;
            this.RotateAngle = MenuDefaults.RotateAngle;
            this.MenuWholeAngle = (float)MenuDefaults.MenuWholeAngle;
            this.StartPoint = new PointF(MenuDefaults.StartPointX, MenuDefaults.StartPointY);
            this.ExpandRotation = (float)MenuDefaults.ExpandRotation;
            this.CloseRotation = (float)MenuDefaults.CloseRotation;

            this.menuItems = new List<MenuItem>(menus);

            this.addButton = new MenuItem(image, highlightImage, contentImage, highlightContentImage);

            this.addButton.Touched += (sender, e) => this.Expanded = !this.Expanded;
            this.addButton.Center = this.StartPoint;

            this.AddSubview(this.addButton);
        }

        public event EventHandler<MenuItemSelectedEventArgs> MenuItemSelected;
        
        public List<MenuItem> MenuItems
        {
            get
            {
                return this.menuItems;
            }
        }

        public PointF StartPoint
        {
            get
            {
                return this.startPoint;
            }

            set
            {
                this.startPoint = value;
                if (this.addButton != null)
                {
                    this.addButton.Center = value;
                }
            }
        }

        public LayoutMode Mode { get; set; }

        public double TimeOffset { get; set; }
        
        public float NearRadius { get; set; }

        public float EndRadius  { get; set; }

        public float FarRadius  { get; set; }

        public float RotateAngle { get; set; }

        public float MenuWholeAngle { get; set; }

        public float ExpandRotation { get; set; }

        public float CloseRotation { get; set; }

        public float RadiusStep { get; set; }

        public UIImage Image
        {
            get
            {
                return this.addButton.Image;
            }

            set
            {
                this.addButton.Image = value;
            }
        }

        public UIImage HighlightedImage
        {
            get
            {
                return this.addButton.HighlightedImage;
            }

            set
            {
                this.addButton.HighlightedImage = value;
            }
        }

        public UIImage ContentImage
        {
            get
            {
                return this.addButton.ContentImageView.Image;
            }

            set
            {
                this.addButton.ContentImageView.Image = value;
            }
        }

        public UIImage HighlightedContentImage
        {
            get
            {
                return this.addButton.ContentImageView.HighlightedImage;
            }
            set
            {
                this.addButton.ContentImageView.HighlightedImage = value;
            }
        }


        public bool Expanded
        {
            get
            {
                return this.expanded;
            }

            set
            {
                if (value) 
                {
                    this.SetMenu();
                }

                this.expanded = value;
                float angle = this.Expanded ? - (float)(Math.PI / 4) : 0.0f;

                UIView.Animate(0.2f, () => 
                {
                    this.addButton.Transform = CGAffineTransform.MakeRotation(angle);
                });

                if (this.timer == null) 
                {
                    this.flag = this.Expanded ? 0 : (this.MenuItems.Count - 1);

                    Selector action = this.Expanded ? new Selector("expand:") : new Selector("close:");

                    this.timer = new NSTimer(NSDate.Now, this.TimeOffset, this, action, null, true);

                    NSRunLoop.Current.AddTimer(this.timer, NSRunLoopMode.Common);
                    this.isAnimating = true;
                }
            }
        }

        public void SetMenuItems(IEnumerable<MenuItem> items)
        {
            this.MenuItems.Clear();
            this.MenuItems.AddRange(items);

            foreach (MenuItem view in this.Subviews.OfType<MenuItem>().ToList()) 
            {
                if (view.MenuTag >= 1000) 
                {
                    view.RemoveFromSuperview();
                }
            }
        }

        public override bool PointInside(PointF point, UIEvent @event)
        {
            if (this.isAnimating) 
            {
                return false;
            }

            if (this.Expanded) 
            {
                return true;
            }
            else 
            {
                var result = this.addButton.Frame.Contains(point);
                return result;
            }
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            this.Expanded = !this.Expanded;
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
        }

        private static PointF RotateCGPointAroundCenter(PointF point, PointF center, float angle)
        {
            CGAffineTransform translation = CGAffineTransform.MakeTranslation(center.X, center.Y);
            CGAffineTransform rotation = CGAffineTransform.MakeRotation(angle);

            var inverted = CGAffineTransform.CGAffineTransformInvert(translation);

            CGAffineTransform transformGroup = inverted * rotation * translation;

            return transformGroup.TransformPoint(point);
        }

        private void AwesomeMenuItemTouchesEnd(MenuItem item)
        {
            if (item == this.addButton) 
            {
                return;
            }

            CAAnimationGroup blowup = this.BlowupAnimationAtPoint(item.Center);

            item.Layer.AddAnimation(blowup, "blowup");
            item.Center = item.StartPoint;

            for (int i = 0; i < this.MenuItems.Count; i++) 
            {
                MenuItem otherItem = this.MenuItems[i];
                CAAnimationGroup shrink = this.ShrinkAnimationAtPoint(otherItem.Center);
                if (otherItem.MenuTag == item.MenuTag) 
                {
                    continue;
                }

                otherItem.Layer.AddAnimation(shrink, "shrink");
                otherItem.Center = otherItem.StartPoint;
            }

            this.expanded = false;
            float angle = this.Expanded ? - (float)Math.PI / 4 : 0.0f;

            UIView.Animate(0.2f, () => 
            {
                this.addButton.Transform = CGAffineTransform.MakeRotation(angle);
            });

            var handler = this.MenuItemSelected;
            if (handler != null)
            {
                handler(this, new MenuItemSelectedEventArgs(item.MenuTag - 1000));
            }
        }

        private MenuItem GetByTag(int menuTag)
        {
            return this.menuItems.Where(x => x.MenuTag == menuTag).First();
        }

        [Export("expand:")]
        private void Expand()
        {
            if (flag == this.MenuItems.Count) 
            {
                this.isAnimating = false;
                this.timer.Invalidate();
                this.timer = null;
                return;
            }

            int tag = 1000 + this.flag;

            MenuItem item = this.GetByTag(tag);

            item.Selected -= this.ItemSelected;
            item.Selected += this.ItemSelected;

            var rotateAnimation = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath("transform.rotation.z");
            rotateAnimation.Values = new NSNumber[] {this.ExpandRotation, 0.0f};
            rotateAnimation.Duration = 0.5f;
            rotateAnimation.KeyTimes = new NSNumber[] {0.3f, 0.4f}; 

            var positionAnimation = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath("position");
            positionAnimation.Duration = 0.5f;

            var path = new CGPath();
            path.MoveToPoint(item.StartPoint.X, item.StartPoint.Y);
            path.AddLineToPoint(item.FarPoint.X, item.FarPoint.Y);
            path.AddLineToPoint(item.NearPoint.X, item.NearPoint.Y);
            path.AddLineToPoint(item.EndPoint.X, item.EndPoint.Y);
            positionAnimation.Path = path;

            var animationGroup = new CAAnimationGroup();
            animationGroup.Animations = new[] {positionAnimation, rotateAnimation};
            animationGroup.Duration = 0.5f;
            animationGroup.FillMode = CAFillMode.Forwards;
            animationGroup.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseIn);

            item.Layer.AddAnimation(animationGroup, "Expand");

            item.Alpha = 1f;
            item.Center = item.EndPoint;
            this.flag++;
        }

        private void SetMenu()
        {
            int count = this.MenuItems.Count;

            float step = 0f;

            for (int i = 0; i < count; i++) 
            {
                MenuItem item = this.MenuItems[i];

                item.MenuTag = 1000 + i;
                item.StartPoint = this.StartPoint;

                switch (this.Mode)
                {
                    case LayoutMode.Radial:
                        this.LayoutMenuItemRadial(item, i, count, step);
                        break;

                    case LayoutMode.Horizontal:
                        this.LayoutMenuItemHorizontal(item, i, count, step);
                        break;

                    case LayoutMode.Vertical:
                        this.LayoutMenuItemVertical(item, i, count, step);
                        break;
                }


                item.Center = item.StartPoint;

                this.InsertSubviewBelow(item, this.addButton);

                step += this.RadiusStep;
            }
        }

        private void LayoutMenuItemRadial(MenuItem item, int index, int count, float step)
        {
            var endPoint = new PointF(this.StartPoint.X + (this.EndRadius - step) * (float)Math.Sin(index * this.MenuWholeAngle / count), 
                                      this.StartPoint.Y - (this.EndRadius - step) * (float)Math.Cos(index * this.MenuWholeAngle / count));

            item.EndPoint = RotateCGPointAroundCenter(endPoint, this.StartPoint, this.RotateAngle);

            var nearPoint = new PointF(this.StartPoint.X + (this.NearRadius - step) * (float)Math.Sin(index * this.MenuWholeAngle / count), 
                                       this.StartPoint.Y - (this.NearRadius - step) * (float)Math.Cos(index * this.MenuWholeAngle / count));

            item.NearPoint = RotateCGPointAroundCenter(nearPoint, this.StartPoint, this.RotateAngle);

            var farPoint = new PointF(this.StartPoint.X + (this.FarRadius - step) * (float)Math.Sin(index * this.MenuWholeAngle / count), 
                                      this.StartPoint.Y - (this.FarRadius - step) * (float)Math.Cos(index * this.MenuWholeAngle / count));

            item.FarPoint = RotateCGPointAroundCenter(farPoint, this.StartPoint, this.RotateAngle);
        }

        private void LayoutMenuItemHorizontal(MenuItem item, int index, int count, float step)
        {
            var endPoint = new PointF(this.StartPoint.X + (this.EndRadius - step), this.StartPoint.Y);

            item.EndPoint = RotateCGPointAroundCenter(endPoint, this.StartPoint, this.RotateAngle);

            var nearPoint = new PointF(this.StartPoint.X + (this.NearRadius - step), this.StartPoint.Y);

            item.NearPoint = RotateCGPointAroundCenter(nearPoint, this.StartPoint, this.RotateAngle);

            var farPoint = new PointF(this.StartPoint.X + (this.FarRadius - step), this.StartPoint.Y);

            item.FarPoint = RotateCGPointAroundCenter(farPoint, this.StartPoint, this.RotateAngle);
        }

        private void LayoutMenuItemVertical(MenuItem item, int index, int count, float step)
        {
            var endPoint = new PointF(this.StartPoint.X, this.StartPoint.Y + (this.EndRadius - step));

            item.EndPoint = RotateCGPointAroundCenter(endPoint, this.StartPoint, this.RotateAngle);

            var nearPoint = new PointF(this.StartPoint.X, this.StartPoint.Y + (this.NearRadius - step));

            item.NearPoint = RotateCGPointAroundCenter(nearPoint, this.StartPoint, this.RotateAngle);

            var farPoint = new PointF(this.StartPoint.X, this.StartPoint.Y + (this.FarRadius - step));

            item.FarPoint = RotateCGPointAroundCenter(farPoint, this.StartPoint, this.RotateAngle);
        }

        [Export("close:")]
        private void Close()
        {
            if (flag == -1) 
            {
                this.isAnimating = false;
                this.timer.Invalidate();
                this.timer = null;
                return;
            }

            int tag = 1000 + flag;
            MenuItem item = this.GetByTag(tag);

            item.Selected -= this.ItemSelected;

            var rotateAnimation = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath("transform.rotation.z");
            rotateAnimation.Values = new NSNumber[] {0f, this.CloseRotation, 0f};
            rotateAnimation.Duration = 0.5f;
            rotateAnimation.KeyTimes = new NSNumber[] {0f, 0.4f, 0.5f}; 

            var positionAnimation = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath("position");
            positionAnimation.Duration = 0.5f;

            var path = new CGPath();
            path.MoveToPoint(item.EndPoint.X, item.EndPoint.Y);
            path.AddLineToPoint(item.FarPoint.X, item.FarPoint.Y);
            path.AddLineToPoint(item.StartPoint.X, item.StartPoint.Y);
            positionAnimation.Path = path;

            var alphaAnimation = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath("opacity");
            alphaAnimation.Values = new NSNumber[] {1f, 1f, 0f};
            alphaAnimation.Duration = 0.5f;
            alphaAnimation.KeyTimes = new NSNumber[] {0f, 0.8f, 1f};

            var animationGroup = new CAAnimationGroup();
            animationGroup.Animations = new[] { positionAnimation, rotateAnimation, alphaAnimation };
            animationGroup.Duration = 0.5f;
            animationGroup.FillMode = CAFillMode.Forwards;
            animationGroup.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseIn);

            item.Layer.AddAnimation(animationGroup, "Close");

            item.Alpha = 0f;
            item.Center = item.StartPoint;
            flag--;
        }

        private void ItemSelected(object sender, EventArgs args)
        {
            this.AwesomeMenuItemTouchesEnd(sender as MenuItem);
        }

        // todo: animate opacity to match expand / close 
        private CAAnimationGroup BlowupAnimationAtPoint(PointF p)
        {
            var positionAnimation = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath("position");
            positionAnimation.Values = new NSObject[] { NSValue.FromPointF(p)};
            positionAnimation.KeyTimes = new NSNumber[] {0.3f}; 

            var scaleAnimation = CABasicAnimation.FromKeyPath("transform");
            scaleAnimation.To = NSValue.FromCATransform3D(CATransform3D.MakeScale(3, 3, 1));

            var opacityAnimation = CABasicAnimation.FromKeyPath("opacity");
            opacityAnimation.To = NSNumber.FromFloat(0);

            var animationGroup = new CAAnimationGroup();
            animationGroup.Animations = new CAAnimation[] { positionAnimation, scaleAnimation, opacityAnimation };
            animationGroup.Duration = 0.3f;
            animationGroup.FillMode = CAFillMode.Forwards;

            return animationGroup;
        }

        private CAAnimationGroup ShrinkAnimationAtPoint(PointF p)
        {
            var positionAnimation = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath("position");
            positionAnimation.Values = new NSObject[] { NSValue.FromPointF(p)};
            positionAnimation.KeyTimes = new NSNumber[] {0.3f}; 

            var scaleAnimation = CABasicAnimation.FromKeyPath("transform");
            scaleAnimation.To = NSValue.FromCATransform3D(CATransform3D.MakeScale(0.01f, 0.01f, 1f));

            var opacityAnimation = CABasicAnimation.FromKeyPath("opacity");
            opacityAnimation.To = NSNumber.FromFloat(0);

            var animationGroup = new CAAnimationGroup();
            animationGroup.Animations = new CAAnimation[] { positionAnimation, scaleAnimation, opacityAnimation };
            animationGroup.Duration = 0.3f;
            animationGroup.FillMode = CAFillMode.Forwards;

            return animationGroup;
        }
    }
}




