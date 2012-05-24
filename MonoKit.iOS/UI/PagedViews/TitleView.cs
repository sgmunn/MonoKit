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

using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace MonoKit.UI.PagedViews
{
    public class TitleView : UIView
    {
        public TitleView(RectangleF frame) : base(frame)
        {
            this.Initialize(frame);
        }
        
        public int Index
        {
            get;
            set;
        }
        
        public UILabel TitleLabel
        {
            get;
            private set;
        }

        public UILabel SubtitleLabel
        {
            get;
            private set;
        }
        
        public virtual void DisplayTitle(PagingTitle title)
        {
            this.TitleLabel.Text = title.Title;
            this.SubtitleLabel.Text = title.Subtitle;
        }
        
        public void Initialize(RectangleF frame)
        {
            //Console.WriteLine("title view frame " + frame.ToString());
            this.TitleLabel = new UILabel(new RectangleF(5, 2, frame.Size.Width - 10, 20));
            this.TitleLabel.Font = UIFont.BoldSystemFontOfSize(17);
            this.TitleLabel.Text = "assdfsdf";
            //Console.WriteLine("title view frame - title " + this.titleLabel.Frame.ToString());
            
            this.SubtitleLabel = new UILabel(new RectangleF(5, 27, frame.Size.Width - 10, frame.Size.Height - 29));
            this.SubtitleLabel.Font = UIFont.SystemFontOfSize(10);
            this.SubtitleLabel.Text = @"this is some subtitle this is some subtitle this is some subtitle this is some subtitle this is some subtitle this is some subtitle this is some subtitle xxx";
            this.SubtitleLabel.Lines = 4;
            //Console.WriteLine("title view frame - subtitle " + this.subtitleLabel.Frame.ToString());
            
            this.AddSubview(this.TitleLabel);
            this.AddSubview(this.SubtitleLabel);
        }
    }
}

