namespace MonoKit.UI.PagedViews
{
    using System;
    using System.Drawing;
    using MonoTouch.UIKit;

    /// <summary>
    /// Defines the delegate for the scolling page view
    /// </summary>
    public interface IScrollingPageViewDelegate
    {
        /// <summary>
        /// Gets the page count.
        /// </summary>
        int PageCount
        {
            get;
        }

        /// <summary>
        /// Gets the page type key for the page at the specified index
        /// </summary>
        /// <returns>
        /// Returns a string identifying the type of view that is required for the page.  this can be empty.
        /// </returns>
        /// <param name='index'>
        /// The page index
        /// </param>
        string GetPageTypeKey(int index);
        
        /// <summary>
        /// Creates the view that is required for the given pageTypeKey
        /// </summary>
        /// <returns>
        /// Returns a view 
        /// </returns>
        /// <param name='pageTypeKey'>
        /// The type of view that is required to be created, may be null or empty
        /// </param>
        /// <param name='frame'>
        /// The frame in which to create the view
        /// </param>
        UIView CreateView(string pageTypeKey, RectangleF frame);
        
        /// <summary>
        /// Updates the page (view) with information required at the page with the given index.
        /// </summary>
        /// <param name='index'>
        /// The index of the page that needs to be displayed
        /// </param>
        /// <param name='view'>
        /// The view that is to show the pages information
        /// </param>
        void UpdatePage(int index, UIView view);
        
        /// <summary>
        /// Notifies that the current page index has changed, can be used to update other ui elements
        /// </summary>
        /// <param name='index'>
        /// The index of the current page
        /// </param>
        void PageIndexChanged(int index);
    }
}

