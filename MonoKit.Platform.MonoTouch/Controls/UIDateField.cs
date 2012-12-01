// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIDateField.cs" company="sgmunn">
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

namespace MonoKit.Controls
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
        
        public override void TouchesEnded(NSSet touches, UIEvent evt)
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

