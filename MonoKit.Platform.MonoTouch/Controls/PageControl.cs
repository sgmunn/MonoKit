// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageControl.cs" company="sgmunn">
//   (c) sgmunn 2012  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Controls
{
    using System;
    using System.Drawing;
    using MonoTouch.UIKit;
    
    /// <summary>
    /// Implements a replacement for UIPageControl.
    /// </summary>
    public class PageControl : UIControl
    {
        private int pageCount;
        private int currentPage;
        private bool hidesForSinglePage;
        private UIColor activePageColor;
        private UIColor inactivePageColor;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.UI.PageControl"/> class.
        /// </summary>
        public PageControl(RectangleF frame) : base(frame)
        {
            this.HidesForSinglePage = false;
            this.BackgroundColor = UIColor.Clear;
            this.Enabled = false;
            this.ActivePageColor = UIColor.DarkGray;
            this.InactivePageColor = UIColor.LightGray;
        }
        
        /// <summary>
        /// Gets or sets the number of indicators that the page control should display
        /// </summary>
        public int Pages
        {
            get
            {
                return this.pageCount;
            }
            
            set
            {
                this.pageCount = value;
                this.SetNeedsDisplay();
            }
        }
        
        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int CurrentPage
        {
            get
            {
                return this.currentPage;
            }
            
            set
            {
                this.currentPage = value;
                this.SetNeedsDisplay();
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the indicators should be hidden when there is only 1 page
        /// </summary>
        public bool HidesForSinglePage
        {
            get
            {
                return this.hidesForSinglePage;
            }
            
            set
            {
                this.hidesForSinglePage = value;
                this.SetNeedsDisplay();
            }
        }
        
        /// <summary>
        /// Gets or sets the color of the active page indicator
        /// </summary>
        public UIColor ActivePageColor
        {
            get
            {
                return this.activePageColor;
            }
            
            set
            {
                this.activePageColor = value;
                this.SetNeedsDisplay();
            }
        }
        
        /// <summary>
        /// Gets or sets the color of the inactive page indicators
        /// </summary>
        public UIColor InactivePageColor
        {
            get
            {
                return this.inactivePageColor;
            }
            
            set
            {
                this.inactivePageColor = value;
                this.SetNeedsDisplay();
            }
        }
        
        /// <summary>
        /// Draws the page indicators
        /// </summary>
        public override void Draw(RectangleF rect)
        {
            if (!this.HidesForSinglePage || this.Pages > 1)
            {
                var context = UIGraphics.GetCurrentContext();
                
                context.SaveState();
                context.SetAllowsAntialiasing(true);
                
                var dotSize = 5;
                var dotsWidth = (dotSize * this.Pages) + (this.Pages -1) * 10;
                var offset = (this.Frame.Size.Width - dotsWidth) / 2;
                
                for (int i = 0; i < this.Pages; i++)
                {
                    var dotRect = new RectangleF(offset + (dotSize + 10) * i, (this.Frame.Size.Height / 2) - (dotSize / 2), dotSize, dotSize);

                    if (i == this.CurrentPage)
                    {
                        context.SetFillColor(this.ActivePageColor.CGColor);
                        context.FillEllipseInRect(dotRect);
                    }
                    else
                    {
                        context.SetFillColor(this.InactivePageColor.CGColor);
                        context.FillEllipseInRect(dotRect);
                    }
                    
                }
                
                context.RestoreState();
            }
        }
    }
}

