//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="UIPanoramaView.cs" company="sgmunn">
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
namespace MonoKit.Metro
{
    using System;
    using System.Linq;
    using MonoTouch.UIKit;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Drawing;
    using MonoTouch.Foundation;
    using System.Collections.Generic;

    /// <summary>
    /// Implements a metro style panorama.
    /// todo: free up unused views when requested, dispose etc
    /// </summary>
    public sealed class UIPanoramaView : UIView
    {
        /// <summary>
        /// The scrollview that scrolls the panorama
        /// </summary> 
        private readonly UIScrollView scrollView;

        /// <summary>
        /// Dictionary of labels for each content item
        /// </summary>
        private readonly Dictionary<ContentItem, UILabel> labels;

        /// <summary>
        /// The font used to display the titles
        /// </summary>
        private UIFont font;

        /// <summary>
        /// The color of the title text
        /// </summary>
        private UIColor textColor;

        /// <summary>
        /// property backing field
        /// </summary>
        private string titleText;

        /// <summary>
        /// The rate at which the title slides in relation to the scrollview
        /// </summary>
        private float titleRate;

        /// <summary>
        /// The rate at which the background slides in relation to the scrollview
        /// </summary>
        private float backgroundRate;

        /// <summary>
        /// property backing field
        /// </summary>
        private bool animateBackground;

        /// <summary>
        /// The space between the title and content items, and the left edge of the view  
        /// </summary>
        private float leftMargin = PanoramaConstants.LeftMargin;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.Metro.UIPanoramaView"/> class.
        /// </summary>
        public UIPanoramaView()
        {
            this.scrollView = new UIScrollView();
            this.BackgroundView = new UIView();

            this.Title = new UILabel();
            this.labels = new Dictionary<ContentItem, UILabel>();

            this.ContentItems = new ObservableCollection<ContentItem>();

            this.animateBackground = true;

            this.Initialize();

            this.Font = UIFont.FromName(PanoramaConstants.DefaultFontName, PanoramaConstants.DefaultFontSize);
            this.TextColor = PanoramaConstants.DefaultTextColor;
        }

        /// <summary>
        /// Gets the collection of items contained in the panorama.
        /// Each item represents a view with a title
        /// </summary>
        public ObservableCollection<ContentItem> ContentItems
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the font that is used to display the main title and title for each content item
        /// </summary>
        public UIFont Font
        {
            get
            {
                return this.font;
            }

            set
            {
                this.font = value;
                this.Title.Font = value;

                foreach (var label in this.labels.Values)
                {
                    label.Font = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the text color that is used to display the main title and title for each content item
        /// </summary>
        public UIColor TextColor
        {
            get
            {
                return this.textColor;
            }

            set
            {
                this.textColor = value;
                this.Title.TextColor = value;

                foreach (var label in this.labels.Values)
                {
                    label.TextColor = value;
                }
            }
        }

        /// <summary>
        /// Gets the title view
        /// </summary>
        public UILabel Title { get; private set; } 

        /// <summary>
        /// Gets the background view
        /// </summary>
        public UIView BackgroundView { get; private set; } 

        /// <summary>
        /// Gets or sets the title text.
        /// </summary>
        public string TitleText
        {
            get
            {
                return this.titleText;
            }

            set
            {
                this.titleText = value;
                this.Title.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to animate the background when the user pans the panorama
        /// </summary>
        public bool AnimateBackground
        {
            get
            {
                return this.animateBackground;
            }

            set
            {
                this.animateBackground = value;
                this.LayoutContent();
            }
        }

        /// <summary>
        /// Layouts the content items and titles
        /// </summary>
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            this.LayoutContent();
        }

        /// <summary>
        /// Handles the scrollview did scroll delegate method and slides the title and background views
        /// </summary>
        [Export("scrollViewDidScroll:")]
        private void DidScroll(UIScrollView sView)
        {
            var offset = this.scrollView.ContentOffset.X * this.titleRate;
            var f = this.Title.Frame;
            f.X = this.leftMargin - offset;
            this.Title.Frame = f;

            if (this.AnimateBackground)
            {
                offset = this.scrollView.ContentOffset.X * this.backgroundRate;
                f = this.Title.Frame;
                f.X = 0 - offset;
                this.BackgroundView.Frame = f;
            }
        }

        /// <summary>
        /// Handles the scrollview will end dragging delegate method and calculates the correct offset
        /// </summary>
        [Export("scrollViewWillEndDragging:withVelocity:targetContentOffset:")]
        private void WillEndDragging(UIScrollView sView, PointF velocity, ref PointF target)
        {
            // BUG: setting the target.X doesn't always have an effect.
            // 3 items, scroll item 2 to just past midway -- doesn't change
            // also, setting X = 0 doesn't work << actually it might just be the one bug (setting zero)

            // if we set to zero, just bump it to 1 to make it actually scroll to there - zero won't do
            if (target.X > 0)
            {
                target.X = Math.Max(this.NearestItemLeftFromOffset(target.X), 1);
            }
        }

        /// <summary>
        /// Calculates the left edge of the item that offset is closest to 
        /// </summary>
        /// <returns>
        /// Returns a float containing the left edge of the item (exclduing margin)
        /// </returns>
        /// <param name='offset'>The current offset</param>
        private float NearestItemLeftFromOffset(float offset)
        {
            var chunks = new List<float>();

            var items = from item in this.ContentItems
                select this.CalculateItemWidth(item);

            foreach (var item in items)
            {
                var w = item;
                while (w > this.Frame.Width)
                {
                    var part = this.Frame.Width - PanoramaConstants.NextContentItemPreviewSize;

                    chunks.Add(part);
                    w -= part;
                }

                if (w > 0)
                {
                    chunks.Add(w);
                }
            }

            // each item is a variable width, lets add 'em up
            float x = 0;
            foreach (var chunk in chunks)
            {
                if (offset < (x + (chunk / 2)))
                {
                    return x;
                }

                x += chunk;
            }

            return x;
        }

        /// <summary>
        /// Lays out the content resetting the scollviewer to the left and all titles
        /// </summary>
        private void LayoutContent()
        {
            if (this.ContentItems.Count == 0 || this.Frame.Width == 0) 
            {
                return;
            }

            this.scrollView.ContentSize = new SizeF(this.CalculateContentWidth(), this.Frame.Height);

            this.scrollView.ContentOffset = new PointF(0, 0);

            // set title location
            var titleSize = this.StringSize(this.TitleText, this.Title.Font);
            this.Title.Frame = new RectangleF(this.leftMargin, 0, this.Frame.Width - this.leftMargin, titleSize.Height);

            // reset background
            var frm = this.BackgroundView.Frame;
            frm.X = 0;
            this.BackgroundView.Frame = frm;

            // calculate motion speeds
            this.backgroundRate = 0;
            this.titleRate = 0; 
            var w = this.CalculateContentWidth();
            var titleWidth = this.StringSize(this.TitleText, this.Title.Font).Width + this.leftMargin;
            if (w != 0 && titleWidth != 0)
            {
                this.backgroundRate = titleWidth / w; 

                // title should disappear just before the last page
                // titleWidth * rate = (totalWidth - titleWidth)
                // rate = (totalWidth - titleWidth) / titleWidth

                this.titleRate = 1 / ((w - 2 * titleWidth) / titleWidth);
            }

            // layout each item's view and a label associated with it
            float x = this.leftMargin;
            foreach (var item in this.ContentItems)
            {
                var itemWidth = this.CalculateItemWidth(item);

                // first the label for the item
                UILabel label;
                if (this.labels.ContainsKey(item))
                {
                    label = this.labels[item];
                }
                else
                {
                    label = new UILabel();
                    this.labels.Add(item, label);
                    this.scrollView.AddSubview(label);
                }

                label.Text = item.Title;
                label.Font = this.Font;
                label.TextColor = this.TextColor;
                label.BackgroundColor = UIColor.Clear;

                // it's location
                var labelSize = this.StringSize(item.Title, label.Font);
                var r = new RectangleF(x, this.Title.Frame.Height, itemWidth - this.leftMargin,  labelSize.Height); 
                label.Frame = r;
                x += itemWidth;

                // and now the item itself
                if (item.Content == null)
                {
                    this.scrollView.AddSubview(item.CreateView());
                }

                r.Y = r.Height + r.Y;
                r.Height = this.Frame.Height - r.Y;
                item.Content.Frame = r;
            }
        }

        /// <summary>
        /// Handles changes in the collection items and updates the layout of the items
        /// </summary>
        private void HandleCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
        {
            // handle content items being added or removed
            // for now just re-layout everything and put the scollviewer back to the left

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    if (this.labels.ContainsKey((ContentItem)item))
                    {
                        var label = this.labels[(ContentItem)item];
                        label.RemoveFromSuperview();
                        label.Dispose();
                    }
                }
            }

            this.LayoutContent();
        }

        /// <summary>
        /// Calculates the with of the given item
        /// </summary>
        /// <returns>
        /// Returns a float that is the width the item should be
        /// </returns>
        /// <param name='item'>
        /// The content item to calculate the width for
        /// </param>
        private float CalculateItemWidth(ContentItem item)
        {
            return item.Width == 0 ? this.Frame.Width : item.Width;
        }

        /// <summary>
        /// Calculates the width of the content that is contained in the scrollviewer
        /// </summary>
        /// <returns>
        /// returns a float that is the total width of the content for the scrollviewer
        /// </returns>
        private float CalculateContentWidth()
        {
            if (this.ContentItems.Count == 0)
            {
                return this.Frame.Width;
            }

            // calulate the total width of all items
            float totalWidth = 0;
            foreach (var item in this.ContentItems)
            {
                totalWidth += this.leftMargin + this.CalculateItemWidth(item);
            }

            // round up to nearest multiple of frame width
            var x = Math.Truncate(totalWidth / this.Frame.Width);
            if (x * this.Frame.Width < totalWidth) 
            {
                x++;
            }

            var res = x * this.Frame.Width;
            return (float)res;
        }

        /// <summary>
        /// Sets up initial state for the views
        /// </summary>
        private void Initialize()
        {
            // background
            this.BackgroundView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            this.AddSubview(this.BackgroundView);
            this.BackgroundView.BackgroundColor = UIColor.White;

            // scrollviewer
            this.scrollView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            this.AddSubview(this.scrollView);
            this.scrollView.BackgroundColor = UIColor.Clear;
            this.scrollView.WeakDelegate = this;

            // title
            this.Title.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            this.Title.BackgroundColor = UIColor.Clear;
            this.AddSubview(this.Title);

            // observe items being added into the collection
            this.ContentItems.CollectionChanged += this.HandleCollectionChanged;
        }
    }
}

