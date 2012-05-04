namespace MonoKit.UI.Controls
{
    using System;
    using System.Drawing;
    using MonoTouch.UIKit;
    using MonoTouch.Foundation;
    
    [Register("UIDateField")]
    public class UIDateField : UIControl
    {
        private DateTime date;
        private UIDatePicker picker;
        private UILabel label;
        private string dateFormat;
        private UIView accessoryView;
        
        public UIDateField(RectangleF frame) : base(frame)
        {
            this.dateFormat = "D";

            this.picker = new UIDatePicker();
            this.picker.Mode = UIDatePickerMode.Date;
            this.picker.TimeZone = NSTimeZone.FromAbbreviation("GMT");
            this.picker.ValueChanged += this.PickerValueChanged;
            
            this.label = new UILabel(this.Bounds);
            this.label.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleLeftMargin;
            this.label.BackgroundColor = UIColor.Clear;
            this.label.UserInteractionEnabled = false;
            this.AddSubview(this.label);
            
            this.Date = DateTime.Today;
        }
        
        public override bool CanBecomeFirstResponder
        {
            get
            {
                return this.Enabled;
            }
        }
        
        public override UIView InputView
        {
            get
            {
                return this.picker;
            }
        }
        
        [Export("inputAccessoryView")]
        public new UIView InputAccessoryView
        {
            get
            {
                return this.accessoryView;
            }
            
            set
            {
                this.accessoryView = value;
            }
        }
        
        public DateTime Date
        {
            get
            {
                return this.date;
            }
            
            set
            {
                if (value != this.date)
                {
                    this.date = value;
                    this.picker.Date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                    this.label.Text = this.date.ToString(this.DateFormat);
                }
            }
        }
        
        public string DateFormat
        {
            get
            {
                return this.dateFormat;
            }
            
            set
            {
                this.dateFormat = value;
                this.label.Text = this.date.ToString(this.DateFormat);
            }
        }
        
        public override void TouchesEnded(MonoTouch.Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            if (evt.Type == UIEventType.Touches)
            {
                if (this.IsFirstResponder)
                {
                    this.ResignFirstResponder();
                }
                else
                {
                    this.BecomeFirstResponder();
                }
            }
        } 
        
        protected override void Dispose(bool disposing)
        {
            Console.WriteLine("Dispose UiDateField");
            if (disposing)
            {
                this.picker.Dispose();
                this.picker = null;
                this.label.Dispose();
                this.label = null;
            }

            base.Dispose(disposing);
        }
        
        private void PickerValueChanged(object sender, EventArgs args)
        {
            this.Date = DateTime.SpecifyKind(this.picker.Date, DateTimeKind.Unspecified);
            this.SendActionForControlEvents(UIControlEvent.ValueChanged);
        }
    }
}

