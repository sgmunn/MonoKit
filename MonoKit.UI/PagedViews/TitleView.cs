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

