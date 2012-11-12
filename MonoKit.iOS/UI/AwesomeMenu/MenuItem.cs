//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="MenuItem.cs" company="sgmunn">
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
    using MonoTouch.UIKit;
    using System.Drawing;

    public sealed class MenuItem : UIImageView
    {
        private UIImageView contentImageView;

        public MenuItem() : base()
        {
        }

        public MenuItem(UIImage image, UIImage highlightImage, UIImage contentImage, UIImage highlightContentImage)
            : base()
        {
            this.Image = image;
            this.HighlightedImage = highlightImage;
            this.UserInteractionEnabled = true;

            // figure this out
            if (contentImage == null)
            {
                contentImage = image;
            }

            if (contentImage != null)
            {
                this.contentImageView = new UIImageView(contentImage);

                if (highlightContentImage != null)
                {
                    this.contentImageView.HighlightedImage = highlightContentImage;
                }

                this.AddSubview(this.contentImageView);
            }
        }

        public MenuItem(UIImage image, UIImage highlightImage, UIImage contentImage)
            : base()
        {
            this.Image = image;
            this.HighlightedImage = highlightImage;
            this.UserInteractionEnabled = true;
            this.contentImageView = new UIImageView(contentImage);
            this.AddSubview(this.contentImageView);
        }

        public MenuItem(UIImage image, UIImage highlightImage)
            : base()
        {
            this.Image = image;
            this.HighlightedImage = highlightImage;
            this.UserInteractionEnabled = true;
        }
        
        public event EventHandler<EventArgs> Touched;

        public event EventHandler<EventArgs> Selected;

        public override bool Highlighted
        {
            get
            {
                return base.Highlighted;
            }

            set
            {
                base.Highlighted = value;
                this.contentImageView.Highlighted = value;
            }
        }

        public UIImageView ContentImageView
        {
            get
            {
                return this.contentImageView;
            }
        }

        public bool ContentHighlighted
        {
            get
            {
                return this.contentImageView.Highlighted;
            }

            set
            {
                this.contentImageView.Highlighted = value;
            }
        }

        internal PointF StartPoint { get; set; }

        internal PointF EndPoint { get; set; }

        internal PointF NearPoint { get; set; }

        internal PointF FarPoint { get; set; }

        internal int MenuTag { get; set; }

        public static RectangleF ScaleRect(RectangleF rect, float n)
        {
            return new RectangleF((rect.Size.Width - rect.Size.Width * n) / 2, (rect.Size.Height - rect.Size.Height * n) / 2, rect.Size.Width * n, rect.Size.Height * n);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            
            this.Bounds = new RectangleF(0, 0, this.Image.Size.Width, this.Image.Size.Height);

            float width = this.contentImageView.Image.Size.Width;
            float height = this.contentImageView.Image.Size.Height;
            this.contentImageView.Frame = new RectangleF(this.Bounds.Size.Width / 2 - width / 2, this.Bounds.Size.Height / 2 - height / 2, width, height);
        }

        public override void TouchesBegan(MonoTouch.Foundation.NSSet touches, UIEvent evt)
        {
            this.Highlighted = true;

            if (this.Touched != null)
            {
                this.Touched(this, new EventArgs());
            }
        }

        public override void TouchesMoved(MonoTouch.Foundation.NSSet touches, UIEvent evt)
        {
            // if move out of 2x rect, cancel highlighted.
            var location = new PointF(0,0);//todo: touches.AnyObject.LocationInView(this);
            if (!ScaleRect(this.Bounds, 2.0f).Contains(location))
            {
                this.Highlighted = false;
            }
        }

        public override void TouchesEnded(MonoTouch.Foundation.NSSet touches, UIEvent evt)
        {
            this.Highlighted = false;

            // if stop in the area of 2x rect, response to the touches event.
            var location = new PointF(0,0);// todo: touches.AnyObject.LocationInView(this);
            if (ScaleRect(this.Bounds, 2.0f).Contains(location))
            {
                if (this.Selected != null)
                {
                    this.Selected(this, new EventArgs());
                }
            }
        }

        public override void TouchesCancelled(MonoTouch.Foundation.NSSet touches, UIEvent evt)
        {
            this.Highlighted = false;
        }
    }
}

