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
using System.Linq;
using MonoTouch.UIKit;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace MonoKit.Metro
{
    public static class PanoramaConstants
    {
        public static float NextContentItemPreviewSize = 30;

        public static string DefaultFontName = "Thonburi";

        public static float DefaultFontSize = 46;

        public static UIColor DefaultTextColor = UIColor.White;

        public static float LeftMargin = 5;
    }

    public sealed class ContentItem
    {
        // todo: make disposable to let go of view ??

        /// <summary>
        /// function to create the view
        /// </summary>
        private Func<UIView> create;

        public ContentItem(string title, Func<UIView> create, float width)
        {
            this.Title = title;
            this.create = create;

            this.Width = width != 0 ? width : UIScreen.MainScreen.Bounds.Width - PanoramaConstants.NextContentItemPreviewSize;
        }

        public string Title { get; private set; }

        public float Width { get; private set; }

        public UIView CreateView()
        {
            this.Content = this.create();
            return this.Content;
        }

        public UIView Content { get ; private set; }
    }

    /// <summary>
    /// Implements a metro style panorama.
    /// todo: free up unused views when requested, dispose etc
    /// </summary>
    public class UIPanoramaView : UIView
    {

        private readonly UIScrollView scrollView;

        private readonly Dictionary<ContentItem, UILabel> labels;
        
        private UIFont font;

        private UIColor textColor;

        private string titleText;

        private float titleRate;

        private float backgroundRate;

        private bool animateBackground;

        private float leftMargin = PanoramaConstants.LeftMargin;

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

        public ObservableCollection<ContentItem> ContentItems
        {
            get;
            private set;
        }

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

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            this.LayoutContent();
        }

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

        [Export("scrollViewWillEndDragging:withVelocity:targetContentOffset:")]
        private void WillEndDragging(UIScrollView sView, PointF velocity, ref PointF target)
        {
            // BUG: setting the target.X doesn't always have an effect.
            // 3 items, scroll item 2 to just past midway -- doesn't change
            // also, setting X = 0 doesn't work
            target.X = this.NearestItemLeftFromOffset(target.X);
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

        private float CalculateItemWidth(ContentItem item)
        {
            return item.Width == 0 ? this.Frame.Width : item.Width;
        }

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

