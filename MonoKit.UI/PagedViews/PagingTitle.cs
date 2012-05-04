using System;
using MonoTouch.UIKit;

namespace MonoKit.UI.PagedViews
{
    public class PagingTitle
    {
        public PagingTitle()
        {
        }
        
        public PagingTitle(string title, string subtitle)
        {
            this.Title = title;
            this.Subtitle = subtitle;
        }
        
        public string Title
        {
            get;
            set;
        }
        
        public string Subtitle
        {
            get;
            set;
        }
    }
}

