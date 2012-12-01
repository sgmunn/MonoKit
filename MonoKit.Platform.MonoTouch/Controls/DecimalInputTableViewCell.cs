//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="DecimalInputTableViewCell.cs" company="sgmunn">
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

namespace MonoKit.Controls
{
    using System;
    using System.Drawing;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    
    public class DecimalInputTableViewCell : TableViewCell
    {       
        private UITextField textField;

        private decimal inputValue;
        
        public DecimalInputTableViewCell(UITableViewCellStyle style, string reuseIdentifer)
            : base(style, reuseIdentifer)
        {
            this.ConfigureCell();
        }
        
        public UITextField TextField
        {
            get
            {
                return this.textField;
            }
        }
        
        public decimal InputValue
        {
            get
            {
                return this.inputValue;
            }
            
            set 
            {
                if (value != this.inputValue)
                {
                    this.inputValue = value;
                    this.NotifyPropertyChanged("InputValue");
                    this.InputValueChanged(value);
                }
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.AccessoryView = null;
                this.textField.Dispose();
                this.textField = null;
            }
            
            base.Dispose (disposing);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded (touches, evt);
            if (evt.Type == UIEventType.Touches)
            {
                // todo: make a "Resign on touch" behaviour that can toggle first responder instead of this.
                if (this.textField.IsFirstResponder)
                {
                    this.textField.ResignFirstResponder();
                }
                else
                {
                    this.textField.BecomeFirstResponder();
                }
            }
        }
        
        public override bool BecomeFirstResponder()
        {
            return this.textField.BecomeFirstResponder();
        }
        
        protected override void TextChanged(string newValue)
        {
            base.TextChanged(newValue);
            this.textField.Frame = this.CalculateTextFieldFrame(newValue);
        }

        protected virtual void InputValueChanged(decimal newValue)
        {
            this.textField.Text = string.Format("{0}", newValue);
        }
        
        private void ConfigureCell()
        {
            this.SelectionStyle = UITableViewCellSelectionStyle.None;
            
            this.textField = new UITextField(this.CalculateTextFieldFrame(this.Text))
            {
                KeyboardType = UIKeyboardType.DecimalPad,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleLeftMargin,
            };
            
            this.AccessoryView = this.textField;
            
            var proxy = new WeakEventWrapper<DecimalInputTableViewCell, EventArgs>(this, (t,s,o) => {
                try
                {
                    var val = ((UITextField)s).Text;
                    if (!string.IsNullOrEmpty(val) && !string.IsNullOrWhiteSpace(val))
                    {
                        t.InputValue = Convert.ToDecimal(val); 
                    }
                    else
                    {
                        t.InputValue = 0;
                    }
                }
                catch
                {
                    t.InputValue = 0;
                }
            });

            this.textField.Ended += proxy.HandleEvent;
        }
        
        private RectangleF CalculateTextFieldFrame(string textValue)
        {
            float margin = 10;
            
            var textSize = new RectangleF (margin, 10, this.ContentView.Bounds.Width - (margin * 2), this.ContentView.Bounds.Height - (margin * 2)); 
            
            if (!String.IsNullOrEmpty(textValue))
            {
                var sz = this.CalculateEntrySize(null);
                textSize = new RectangleF (sz.Width, (this.ContentView.Bounds.Height - sz.Height) / 2 - 1, sz.Width * 2 - margin, sz.Height);
            }
            
            return textSize;
        }
        
        private SizeF CalculateEntrySize (UITableView tv)
        {
            var sz = this.StringSize("W", UIFont.SystemFontOfSize (17));
            float w = this.ContentView.Bounds.Width / 3;
            
            return new SizeF(w, sz.Height);
        }
    }
}
