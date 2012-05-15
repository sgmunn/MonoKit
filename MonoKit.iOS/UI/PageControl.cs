namespace MonoKit.UI
{
    using System;
    using System.Drawing;
    using MonoTouch.UIKit;
    
    /// <summary>
    /// Implements a replacement for UIPageControl.
    /// </summary>
    public class PageControl : UIControl
    {
        /// <summary>
        /// property backing field
        /// </summary>
        private int pageCount;
        
        /// <summary>
        /// property backing field
        /// </summary>
        private int currentPage;
        
        /// <summary>
        /// property backing field
        /// </summary>
        private bool hidesForSinglePage;
        
        /// <summary>
        /// property backing field
        /// </summary>
        private UIColor activePageColor;
        
        /// <summary>
        /// property backing field
        /// </summary>
        private UIColor inactivePageColor;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.UI.PageControl"/> class.
        /// </summary>
        /// <param name='frame'>
        /// The PageControls frame
        /// </param>
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
        /// <param name='rect'>
        /// The rect that should be drawn in
        /// </param>
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

