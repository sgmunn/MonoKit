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
//
namespace MonoKit.Metro
{
    using System;
    using MonoTouch.UIKit;

    /// <summary>
    /// Controller for panorama views
    /// </summary>
    public class UIPanoramaViewController : UIViewController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.Metro.UIPanoramaViewController"/> class.
        /// </summary>
        public UIPanoramaViewController()
        {
            this.View = new UIPanoramaView();
        }

        /// <summary>
        /// Gets or sets the title of the panorama
        /// </summary>
        public string Title
        {
            get
            {
                return this.Panorama.TitleText;
            }

            set
            {
                this.Panorama.TitleText = value;
            }
        }

        public UIPanoramaView Panorama
        {
            get
            {
                return ((UIPanoramaView)this.View);
            }
        }

        /// <summary>
        /// Gets the background view of the panorama
        /// </summary>
        public UIView BackgroundView
        {
            get
            {
                return ((UIPanoramaView)this.View).BackgroundView;
            }
        }

        /// <summary>
        /// Adds a new content item to the panorama
        /// </summary>
        /// <param name='item'>
        /// The item to add 
        /// </param>
        public void Add(ContentItem item)
        {
            this.Panorama.ContentItems.Add(item);
        }
    }
}

