//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ContentItem.cs" company="sgmunn">
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
    using MonoTouch.UIKit;

    /// <summary>
    /// An item of content in a Panorama or Pivot
    /// </summary>
    public sealed class ContentItem
    {
        // todo: make disposable to let go of view ??

        /// <summary>
        /// function to create the view
        /// </summary>
        private Func<UIView> create;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.Metro.ContentItem"/> class.
        /// </summary>
        /// <param name='title'>The title of the content</param>
        /// <param name='create'>A function to create the view that represents the content</param>
        /// <param name='width'>The desired width of the content, zero defaults to standard</param>
        public ContentItem(string title, Func<UIView> create, float width)
        {
            this.Title = title;
            this.create = create;

            this.Width = width != 0 ? width : UIScreen.MainScreen.Bounds.Width - PanoramaConstants.NextContentItemPreviewSize;
        }

        /// <summary>
        /// Gets the title for the content
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the width of the item
        /// </summary>
        public float Width { get; private set; }

        // todo: get rid of this
        /// <summary>
        /// Gets the content that was previously created
        /// </summary>
        public UIView Content { get ; private set; }

        /// <summary>
        /// Creates the view for the item.
        /// </summary>
        /// <returns>
        /// Returns a new UIView
        /// </returns>
        public UIView CreateView()
        {
            this.Content = this.create();
            return this.Content;
        }
    }
}
