using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace MonoKit.UI.PagedViews
{
    public class MetroPage
    {
        public string Title { get; set; }
    }
    
    public class MetroPageView : UIView, IScrollingPageViewDelegate
    {
        private string fontStyle = "Optima-Regular";
        
        private float fontSize = 48;
        
        private ScrollingPageView pgTitle;
        
        private UIButton mainLabel;
        
        private readonly List<MetroPage> pages = new List<MetroPage>();
        
        private readonly List<MetroPageInstance> pageInstances = new List<MetroPageInstance>();
        
        
        
        public MetroPageView(RectangleF frame, IEnumerable<MetroPage> pages) : base(frame)
        {
            var f = frame;
            f.Y = 100;
            this.pgTitle = new ScrollingPageView(f, true);
            this.pgTitle.Delegate = this;
            this.pgTitle.AutoresizingMask = UIViewAutoresizing.All;
            this.AddSubview(this.pgTitle);
            
            this.Load(pages);
        }
        
        private void Load(IEnumerable<MetroPage> pages)
        {
            this.pages.AddRange(pages);
            this.pgTitle.ReloadPages();
            this.CreateButtons();
        }
        
        private void CreateButtons()
        {
            float left = 0;
            int index = 0;
            foreach (var page in this.pages)
            {
                var instance = new MetroPageInstance();
                instance.Button = this.CreateButton(page.Title, left, index);
                this.AddSubview(instance.Button);
                this.pageInstances.Add(instance);
                index++;
                left += instance.Button.Frame.Width;
            }
        }
        
        private UIButton CreateButton(string title, float left, int index)
        {
            var fnt = UIFont.FromName(this.fontStyle, this.fontSize);
            if (fnt == null)
            {
                fnt = UIFont.SystemFontOfSize(this.fontSize);
            }
            
            var sz = new NSString(title).StringSize(fnt);
            
            RectangleF frame = new RectangleF(0, 0, sz.Width + 20, sz.Height + 20);
            
            var label = new UILabel(frame);
            label.Text = title;
            label.Font = fnt;
            label.TextAlignment = UITextAlignment.Center;
            label.TextColor = UIColor.LightGray;
            
            //label.BackgroundColor = UIColor.Clear;
   
            frame.X = left;
            frame.Y = 0;
            
            var button = UIButton.FromType(UIButtonType.Custom);
            
            button.Frame = frame;
            
            button.AddSubview(label);
            //button.BackgroundColor = UIColor.Blue;
            button.TouchUpInside += HandleMainLabelhandleTouchUpInside;
            button.Tag = index;
            
            return button;
        }
        
        void HandleMainLabelhandleTouchUpInside (object sender, EventArgs e)
        {
            Console.WriteLine("click");
            //this.test.TextColor = UIColor.Black;
            var newIndex = ((UIButton)sender).Tag;
            
            this.pgTitle.ScrollToPage(newIndex);
            this.UpdateFocusedButtonIndex(newIndex);
        }
        
        private void UpdateFocusedButtonIndex(int index)
        {
            foreach (var instance in this.pageInstances)
            {
                var label = (instance.Button.Subviews[0] as UILabel);
                label.TextColor = instance.Button.Tag == index ? UIColor.Black : UIColor.LightGray;
            }
        }
        
        public UIView CreateView(string pageTypeKey, RectangleF frame)
        {
            return new TitleView(frame);
        }
  
        public void UpdatePage(int index, UIView view)
        {
           (view as TitleView).TitleLabel.Text = "test page " + index.ToString();   
        }
        
        public void PageIndexChanged(int index)
        {
            this.UpdateFocusedButtonIndex(index);
            //throw new NotImplementedException();
        }
        
        public string GetPageTypeKey(int index)
        {
            switch (index)
            {
                case 0:
                    return "key1";
                case 1:
                    return "key2";
                case 2:
                    return "key3";
            }
            
            return string.Empty;
        }

        public int PageCount
        {
            get
            {
                return this.pages.Count;
            }
        }
        
        private class MetroPageInstance
        {
            public UIButton Button { get; set; }
            
            
        }

    }
}

