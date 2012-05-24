// --------------------------------------------------------------------------------------------------------------------
// <copyright file=".cs" company="sgmunn">
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

namespace MonoKit.UI.PagedViews
{
    using System;
    using System.Linq;
    using MonoTouch.UIKit;
    using System.Drawing;
    using System.Collections.Generic;
    using MonoTouch.Foundation;

    /// <summary>
    /// Defines a class for handling pages that scroll from left to right (like the photo browser)
    /// </summary>
    public class ScrollingPageView : UIView
    {
        /// <summary>
        /// The scrolling page delegate.
        /// </summary>
        private WeakReference scrollingPageDelegate;
  
        /// <summary>
        /// The background view.
        /// </summary>
        protected UIView backgroundView;
        
        /// <summary>
        /// The scroll view that our pages are hosted in
        /// </summary>
        protected UIScrollView pageScrollView;
        
        /// <summary>
        /// A list of the pages that are visible.
        /// </summary>
        private List<ScrollPage> visiblePages;
        
        /// <summary>
        /// The list of pages that have been recycled.
        /// </summary>
        private List<ScrollPage> recycledPages;
        
        /// <summary>
        /// The index of the current page.
        /// </summary>
        private int currentIndex = -1;
        
        /// <summary>
        /// Determines if we show the left part of the next page
        /// </summary>
        private bool showNextPagePreview;
        
        /// <summary>
        /// Stores the number of pages returned from the delegate.
        /// </summary>
        private int pageCount;
        
        private PageControl pageControl;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.UI.PagedViews.ScrollingPageView"/> class.
        /// </summary>
        /// <param name='frame'>
        /// The frame that the view should be in.
        /// </param>
        /// <param name='showNextPagePreview'>
        /// Determines if the view should show a little bit of the left side of the next page
        /// </param>
        public ScrollingPageView(RectangleF frame, bool showNextPagePreview) : base(frame)
        {
            this.showNextPagePreview = showNextPagePreview;
            this.visiblePages = new List<ScrollPage>();
            this.recycledPages = new List<ScrollPage>();
            
            if (!showNextPagePreview)
            {
                var fm = frame;
                fm.X = 0;
                fm.Width = frame.Width;
                fm.Y = frame.Height - 20;
                fm.Height = 20;
                
                this.pageControl = new PageControl(fm);
                this.pageControl.AutoresizingMask = UIViewAutoresizing.All;
                this.AddSubview(this.pageControl);
            }
            
            this.Initialize(frame, showNextPagePreview);
        }
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            if (this.visiblePages.Count > 0)
            {
                foreach (var page in this.visiblePages)
                {
                    page.View.RemoveFromSuperview();
                    page.View.Dispose();
                    page.View = null;
                }
                
                this.visiblePages.Clear();
            }
            
            if (this.recycledPages.Count > 0)
            {
                foreach (var page in this.recycledPages)
                {
                    page.View.Dispose();
                    page.View = null;
                }
                
                this.recycledPages.Clear();
            }
        }
        
        /// <summary>
        /// Gets or sets the delegate to use as a controller for showing pages
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public IScrollingPageViewDelegate Delegate
        {
            get
            {
                if (this.scrollingPageDelegate == null)
                {
                    return null;
                }
                
                object t = this.scrollingPageDelegate.Target;
                return t as IScrollingPageViewDelegate;
            }
            
            set 
            {
                this.scrollingPageDelegate = new WeakReference(value);
            }
        }
        
        /// <summary>
        /// Reloads the pages and updates the view
        /// </summary>
        public void ReloadPages()
        {
            this.pageCount = this.GetPageCount();
            
            this.currentIndex = -1;

            var pageFrame = new RectangleF(0, 0, this.Frame.Size.Width, this.Frame.Size.Height);
            if (this.showNextPagePreview)
            {
                pageFrame.Width -= 30;
            }
            
            this.pageScrollView.Frame = pageFrame;
            this.pageScrollView.ContentSize = new SizeF(pageFrame.Size.Width * this.pageCount, pageFrame.Size.Height);
            var index = this.PagesScrolled(true);
            if (this.pageCount > 0)
            {
                this.NotifyIndexChanged(index, true);
            }
            
            if (this.pageControl != null)
            {
                this.pageControl.Pages = this.pageCount;
            }
        }
        
        /// <summary>
        /// Scrolls to a particular page
        /// </summary>
        /// <param name='index'>
        /// The page index to scroll to
        /// </param>
        public void ScrollToPage(int index)
        {
            if (index < 0)
            {
                index = 0;
            }
            
            if (index > this.pageCount - 1)
            {
                index = this.pageCount - 1;
            }
            
            var frame = this.pageScrollView.Frame;
            frame.X = frame.Size.Width * index;
            this.pageScrollView.ScrollRectToVisible(frame, true);
            this.currentIndex = index;
        }

        /// <summary>
        /// Creates a page with the given typeKey.  The typeKey is used to allow pages to be different view types, and not all the same
        /// view class.
        /// </summary>
        /// <returns>
        /// Returns a ScrollPage containing the view, and reuse key
        /// </returns>
        /// <param name='pageTypeKey'>
        /// Page type key.
        /// </param>
        /// <param name='frame'>
        /// Frame.
        /// </param>
        private ScrollPage CreatePage(string pageTypeKey, RectangleF frame)
        {
            var d = this.Delegate;
            if (d != null)
            {
                var v = d.CreateView(pageTypeKey, frame);
                v.Frame = frame;
                return new ScrollPage() { View = v, ReuseKey = pageTypeKey };
            }
         
            throw new NullReferenceException("Delegate is null");
        }
        
        /// <summary>
        /// Gets the page count from the Delegate
        /// </summary>
        /// <returns>
        /// The page count.
        /// </returns>
        private int GetPageCount()
        {
            var d = this.Delegate;
            if (d != null)
            {
                return d.PageCount;
            }
            
            return 0;
        }
        
        /// <summary>
        /// Notifies that the current page has changed.
        /// </summary>
        /// <param name='index'>
        /// The index of the page that is now the current page
        /// </param>
        /// <param name='refreshed'>
        /// Indicates that the delegate should be notified, even if the page is the same as last time.
        /// </param>
        private void NotifyIndexChanged(int index, bool refreshed)
        {
            if (refreshed || this.currentIndex != index)
            {
                this.currentIndex = index;

                var d = this.Delegate;
                if (d != null)
                {
                    d.PageIndexChanged(index);
                }
            }
            
            if (this.pageControl != null)
            {
                this.pageControl.CurrentPage = index;
            }
        }
        
        /// <summary>
        /// Handles the event when the scroll viewer has scrolled and returns the current page index
        /// </summary>
        /// <returns>
        /// Returns the index of the page that is now the current page
        /// </returns>
        /// <param name='refreshed'>
        /// Indicates that the scroll is a result of the pages being refeshed
        /// </param>
        private int PagesScrolled(bool refreshed)
        {
            // Calculate which pages are visible
            var visibleBounds = this.pageScrollView.Bounds;
            
            var contentBounds = this.pageScrollView.Frame;
            
            // get the first and last pages of the scroll view that are visible.  we add 1 to the last because
            // we display the second page unclipped outside the scroll view
            int firstNeededPageIndex = (int)Math.Floor(visibleBounds.Left / contentBounds.Width);
            int lastNeededPageIndex = (int)Math.Floor((visibleBounds.Right - 1) / contentBounds.Width) + 1;
            
            // make sure these fit inside our page array
            firstNeededPageIndex = Math.Max(firstNeededPageIndex, 0);
            lastNeededPageIndex = Math.Min(lastNeededPageIndex, this.pageCount - 1);
            
            this.RecyclePagesNotVisible(firstNeededPageIndex, lastNeededPageIndex);
            
            // make sure the visible pages are being displayed 
            for (int index = firstNeededPageIndex; index <= lastNeededPageIndex; index++)
            {
                if (!this.IsDisplayingPageForIndex(index) || refreshed)
                {
                    this.UpdatePage(index);
                }
            }  
            
            return firstNeededPageIndex;
        }
        
        /// <summary>
        /// Recycles the pages are not visible.
        /// </summary>
        /// <param name='firstNeededPageIndex'>
        /// First needed page index.
        /// </param>
        /// <param name='lastNeededPageIndex'>
        /// Last needed page index.
        /// </param>
        private void RecyclePagesNotVisible(int firstNeededPageIndex, int lastNeededPageIndex)
        {
            foreach (var view in this.visiblePages)
            {
                if (view.Index < firstNeededPageIndex || view.Index > lastNeededPageIndex)
                {
                    this.recycledPages.Add(view);
                    view.View.RemoveFromSuperview();
                }
            }
            
            foreach (var view in this.recycledPages)
            {
                this.visiblePages.Remove(view);
            }
        }
        
        /// <summary>
        /// Updates the page at the given index
        /// </summary>
        /// <param name='index'>
        /// The page index
        /// </param>
        private void UpdatePage(int index)  
        {
            var d = this.Delegate;
            if (d != null)
            {
                var key = d.GetPageTypeKey(index);
                var page = this.DequeueRecycledView(key);
                if (page == null) 
                {
                    page = this.CreatePage(key, this.pageScrollView.Frame);
                }
                
                this.UpdatePage(page, index);
                this.pageScrollView.AddSubview(page.View);
                
                this.visiblePages.Add(page);
            }
        }
       
        /// <summary>
        /// Dequeues a recycled view with the given pageTypeKey
        /// </summary>
        /// <returns>
        /// The recycled view.
        /// </returns>
        /// <param name='pageTypeKey'>
        /// The type of the page to dequeue
        /// </param>
        private ScrollPage DequeueRecycledView(string pageTypeKey)
        {
            ScrollPage page = this.recycledPages.FirstOrDefault(p => p.ReuseKey == pageTypeKey);
            if (page != null) 
            {
                this.recycledPages.Remove(page);
            }
            
            return page;
        }
        
        /// <summary>
        /// Determines whether this instance is displaying page for index the specified index.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance is displaying page for index the specified index; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='index'>
        /// If set to <c>true</c> index.
        /// </param>
        private bool IsDisplayingPageForIndex(int index)
        {
            foreach (var page in this.visiblePages) 
            {
                if (page.Index == index) 
                {
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Updates the page.
        /// </summary>
        /// <param name='page'>
        /// The scroll page to update
        /// </param>
        /// <param name='index'>
        /// The page index that the page is now at.
        /// </param>
        private void UpdatePage(ScrollPage page,  int index)
        {
            page.Index = index;
            page.View.Frame = this.FrameForPageAtIndex(index);
            
            var d = this.Delegate;
            if (d != null)
            {
                d.UpdatePage(index, page.View);
            }
        }
        
        /// <summary>
        /// Gets the frame for the page at the specified index
        /// </summary>
        /// <returns>
        /// Returns the frame for the page
        /// </returns>
        /// <param name='index'>
        /// The index of the page that we need to calculate the frame for
        /// </param>
        private RectangleF FrameForPageAtIndex(int index)
        {
            var pagingScrollViewFrame = this.pageScrollView.Frame;
            pagingScrollViewFrame.X = (pagingScrollViewFrame.Size.Width * index);
            return pagingScrollViewFrame;
        }
                
        /// <summary>
        /// Initialize the view with the frame
        /// </summary>
        /// <param name='frame'>
        /// The frame to initialize the view in
        /// </param>
        /// <param name='showNextPagePreview'>
        /// Allow space to preview the next page
        /// </param>
        private void Initialize(RectangleF frame, bool showNextPagePreview)
        {
            var newFrame = new RectangleF(0, 0, frame.Size.Width, frame.Size.Height);
            
            this.backgroundView = new UIView(newFrame);
            this.backgroundView.ClipsToBounds = true;
            this.backgroundView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            
            var pageFrame = newFrame;
            if (showNextPagePreview)
            {
                pageFrame.Width -= 30;
            }
            
            this.pageScrollView = new UIScrollView(pageFrame);
            this.pageScrollView.PagingEnabled = true;
            this.pageScrollView.ShowsVerticalScrollIndicator = false;
            this.pageScrollView.ShowsHorizontalScrollIndicator = true;
            this.pageScrollView.ContentSize = new SizeF(pageFrame.Size.Width * 0, pageFrame.Size.Height);
            this.pageScrollView.ShowsHorizontalScrollIndicator = false;
                     
            this.pageScrollView.ClipsToBounds = false;
            this.pageScrollView.Delegate = new ScrollDelegate(this);
            
            this.backgroundView.AddSubview(this.pageScrollView);
            this.AddSubview(this.backgroundView);
            
            if (this.pageControl != null)
            {
                this.BringSubviewToFront(this.pageControl);
            }
        }

        /// <summary>
        /// Stores information about a view, it's current index (when active) and it's reuse key
        /// </summary>
        private class ScrollPage
        {
            public UIView View { get; set; }
            public int Index { get; set; }
            public string ReuseKey{ get; set; }
        }
        
        /// <summary>
        /// Responds to scrolling events of the scroll viewer
        /// </summary>
        private class ScrollDelegate : UIScrollViewDelegate
        {
            private WeakReference parent;
            
            public ScrollDelegate(ScrollingPageView parent)
            {
                this.parent = new WeakReference(parent);
            }
            
            public override void Scrolled(UIScrollView scrollView)
            {
                var p = this.parent.Target;
                if (p != null && this.parent.IsAlive)
                {
                    (p as ScrollingPageView).PagesScrolled(false);
                }
            }
            
            public override void DecelerationEnded(UIScrollView scrollView)
            {
                var p = this.parent.Target;
                if (p != null && this.parent.IsAlive)
                {
                    var realP = p as ScrollingPageView;
                    var visibleBounds = realP.pageScrollView.Bounds;
                    
                    var contentBounds = realP.pageScrollView.Frame;
                    
                    // get the first and last pages of the scroll view that are visible.  we add 1 to the last because
                    // we display the second page unclipped outside the scroll view
                    int firstNeededPageIndex = (int)Math.Floor(visibleBounds.Left / contentBounds.Width);
                    firstNeededPageIndex = Math.Max(firstNeededPageIndex, 0);
                    
                    realP.NotifyIndexChanged(firstNeededPageIndex, false);
                }
            }
        }
    }
}

