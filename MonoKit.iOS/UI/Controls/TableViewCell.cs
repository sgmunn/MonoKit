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
using MonoTouch.Foundation;
using System.ComponentModel;
using MonoKit.DataBinding;
using System.Drawing;
using MonoKit.UI.Elements;

namespace MonoKit.UI.Controls
{
    public class ElementTableViewCell : UITableViewCell, INotifyPropertyChanged
    {
        private string text;
        
        public ElementTableViewCell() : base(UITableViewCellStyle.Default, "Element")
        {
        }
        
        public ElementTableViewCell(UITableViewCellStyle style, NSString reuseIdentifer)
            : base(style, reuseIdentifer)
        {
        }
        
        public string Text
        {
            get
            {
                return this.text;
            }
            
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    this.OnPropertyChanged("Text");
                    this.TextUpdated(value);
                }
            }
        }
 
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void TextUpdated(string newValue)
        { 
            // todo: if the original value is null or empty this doesn;t update properly
            this.TextLabel.Text = newValue;
        }
        
        protected void OnPropertyChanged(string propertyName)
        {
            var ev = this.PropertyChanged;
            if (ev != null)
            {
                ev(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class ElementTableViewCell<T> : ElementTableViewCell
    {    
        private T value;
        
        public ElementTableViewCell() : base(UITableViewCellStyle.Subtitle, new NSString("StringElement"))
        {
        }
        
        public ElementTableViewCell(UITableViewCellStyle style, NSString reuseIdentifer)
            : base(style, reuseIdentifer)
        {
        }
        
        public T Value
        {
            get
            {
                return this.value;
            }
            
            set
            {
                if (!object.Equals(this.value, value))
                {
                    this.value = value;
                    this.OnPropertyChanged("Value");
                    this.ValueUpdated(value);
                }
            }
        }
        
        protected virtual void ValueUpdated(T newValue)
        {            
        }
    }
    
    public class StringElementTableViewCell : ElementTableViewCell<string>
    {    
        public StringElementTableViewCell() : base(UITableViewCellStyle.Subtitle, new NSString("StringElement"))
        {
        }
        
        public StringElementTableViewCell(UITableViewCellStyle style, NSString reuseIdentifer)
            : base(style, reuseIdentifer)
        {
        }

        protected override void ValueUpdated(string newValue)
        {
            if (this.DetailTextLabel != null)
            {
                this.DetailTextLabel.Text = newValue;
            }
        }
    }
    
    public class DisclosureElementTableViewCell : ElementTableViewCell
    {    
        public DisclosureElementTableViewCell() : base(UITableViewCellStyle.Subtitle, new NSString("DisclosureElement"))
        {
            this.Accessory = UITableViewCellAccessory.DisclosureIndicator;
        }
        
        public DisclosureElementTableViewCell(UITableViewCellStyle style, NSString reuseIdentifer)
            : base(style, reuseIdentifer)
        {
            this.Accessory = UITableViewCellAccessory.DisclosureIndicator;
        }
    }
    
    public class BooleanElementTableViewCell : ElementTableViewCell<bool>
    {    
        private UISwitch boolSwitch;
        
        public BooleanElementTableViewCell() : base(UITableViewCellStyle.Default, new NSString("BooleanElement"))
        {
            this.ConfigureCell();
        }
        
        public BooleanElementTableViewCell(UITableViewCellStyle style, NSString reuseIdentifer)
            : base(style, reuseIdentifer)
        {
            this.ConfigureCell();
        }
          
        protected override void ValueUpdated(bool newValue)
        {
            base.ValueUpdated(newValue);
            this.boolSwitch.On = newValue;
        }
      
        protected override void Dispose (bool disposing)
        {
            if (disposing)
            {
                this.AccessoryView = null;
                this.boolSwitch.Dispose();
                this.boolSwitch = null;
            }
            
            base.Dispose (disposing);
        }
        
        private void ConfigureCell ()
        {
            this.boolSwitch = new UISwitch();
            
            this.AccessoryView = this.boolSwitch;
            
            // The 'On' property doesn't trigger a property change so we need to do it ourselves
            this.boolSwitch.ValueChanged += (sender, e) => 
            {
                this.Value = this.boolSwitch.On;
            };
        }
    }
    
    
    public class CheckboxElementTableViewCell : ElementTableViewCell<bool>
    {    
        public CheckboxElementTableViewCell() : base(UITableViewCellStyle.Default, new NSString("CheckboxElement"))
        {
        }
        
        public CheckboxElementTableViewCell(UITableViewCellStyle style, NSString reuseIdentifer)
            : base(style, reuseIdentifer)
        {
        }
        
        protected override void ValueUpdated(bool newValue)
        {
            this.Accessory = newValue ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
        }
    }
    
    
    public class ButtonElementTableViewCell : ElementTableViewCell<string>
    {    
        private UIButton button;
        
        public ButtonElementTableViewCell() : base(UITableViewCellStyle.Default, new NSString("ButtonElement"))
        {
            this.ConfigureCell();
        }
        
        public ButtonElementTableViewCell(UITableViewCellStyle style, NSString reuseIdentifer)
            : base(style, reuseIdentifer)
        {
            this.ConfigureCell();
        }

        protected override void TextUpdated(string newValue)
        {
            //base.TextUpdated(newValue);
            this.button.SetTitle(newValue, UIControlState.Normal);
        }
        
        protected override void Dispose (bool disposing)
        {
            if (disposing)
            {
                this.AccessoryView = null;
                this.button.Dispose();
                this.button = null;
            }
            
            base.Dispose (disposing);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            this.ContentView.BringSubviewToFront(this.button);
            this.button.Frame = this.ContentView.Bounds;
        }
        
        private void ConfigureCell ()
        {
            this.button = new UIButton(UIButtonType.RoundedRect);
            this.button.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
            this.button.TintColor = UIColor.Red;
            this.ContentView.AddSubview(this.button);
        }
    }

    public class TextInputElementTableViewCell : ElementTableViewCell<string>
    {       
        private UITextField textField;
        
        private string placeholder;
        
        private UIKeyboardType keyboardType;
        
        public TextInputElementTableViewCell() : base(UITableViewCellStyle.Default, new NSString("TextInputElement"))
        {
            this.ConfigureCell();
        }
        
        public TextInputElementTableViewCell(UITableViewCellStyle style, NSString reuseIdentifer)
            : base(style, reuseIdentifer)
        {
            this.ConfigureCell();
        }
        
        public string Placeholder
        {
            get
            {
                return this.placeholder;
            }
            
            set
            {
                if (value != this.placeholder)
                {
                    this.placeholder = value;
                    this.textField.Placeholder = value;
                }
            }
        }
        
        public UIKeyboardType KeyboardType
        {
            get
            {
                return this.keyboardType;
            }
            
            set
            {
                if (value != this.keyboardType)
                {
                    this.keyboardType = value;
                    this.textField.KeyboardType = value;
                }
            }
        }
        
        public UITextField TextField
        {
            get
            {
                return this.textField;
            }
        }
        
        public override void TouchesEnded (NSSet touches, UIEvent evt)
        {
            base.TouchesEnded (touches, evt);
            if (evt.Type == UIEventType.Touches)
            {
                this.textField.BecomeFirstResponder();
            }
        }
                
        public override bool BecomeFirstResponder()
        {
            return this.textField.BecomeFirstResponder();
        }

        public override bool ResignFirstResponder()
        {
            return this.textField.ResignFirstResponder();
        }

        protected override void ValueUpdated(string newValue)
        {
            base.ValueUpdated(newValue);
            this.textField.Text = newValue;
        }
        
        protected override void TextUpdated (string newValue)
        {
            base.TextUpdated (newValue);
            this.textField.Frame = this.CalculateTextFieldFrame(newValue);
            this.ContentView.BringSubviewToFront(this.textField);
        }
         
        protected override void Dispose (bool disposing)
        {
            if (disposing)
            {
                this.textField.Dispose();
                this.textField = null;
            }
            
            base.Dispose (disposing);
        }
        
        private void ConfigureCell ()
        {
            this.SelectionStyle = UITableViewCellSelectionStyle.None;
   
            this.textField = new UITextField(this.CalculateTextFieldFrame(this.Text))
            {
                Placeholder = this.Placeholder,
                KeyboardType = this.KeyboardType,
                VerticalAlignment = UIControlContentVerticalAlignment.Center,
                ClearButtonMode = UITextFieldViewMode.WhileEditing,
            };
            
            this.textField.Font = UIFont.SystemFontOfSize(17);
            this.textField.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleLeftMargin;
            
            this.ContentView.AddSubview(this.textField);
            
            // The 'Text' property doesn't trigger a property change so we need to do it ourselves, but only when the editing has ended
            this.textField.Ended += (sender, e) => 
            { 
                this.Value = this.textField.Text;
            };
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
    
    public class PasswordInputElementTableViewCell : TextInputElementTableViewCell
    {
        
        public PasswordInputElementTableViewCell() : base(UITableViewCellStyle.Default, new NSString("PasswordInputElement"))
        {
            this.TextField.SecureTextEntry = true;
        }
        
        public PasswordInputElementTableViewCell(UITableViewCellStyle style, NSString reuseIdentifer)
            : base(style, reuseIdentifer)
        {
            this.TextField.SecureTextEntry = true;
        }
    }
    
    public class DateInputElementTableViewCell : ElementTableViewCell<DateTime>
    {       
        private UIDateField dateField;
        
        public DateInputElementTableViewCell() : base(UITableViewCellStyle.Default, new NSString("DateInputElement"))
        {
            this.ConfigureCell();
        }
        
        public DateInputElementTableViewCell(UITableViewCellStyle style, NSString reuseIdentifer)
            : base(style, reuseIdentifer)
        {
            this.ConfigureCell();
        }
        
        public override void TouchesEnded (NSSet touches, UIEvent evt)
        {
            base.TouchesEnded (touches, evt);
            if (evt.Type == UIEventType.Touches)
            {
                this.dateField.BecomeFirstResponder();
            }
        }
        
        public override bool BecomeFirstResponder()
        {
            return this.dateField.BecomeFirstResponder();
        }
                
        protected override void Dispose (bool disposing)
        {
            if (disposing)
            {
                this.dateField.Dispose();
                this.dateField = null;
            }
            
            base.Dispose (disposing);
        }
        
        protected override void TextUpdated (string newValue)
        {
            base.TextUpdated (newValue);
            this.dateField.Frame = this.CalculateTextFieldFrame(newValue);
            this.ContentView.BringSubviewToFront(this.dateField);
        }

        protected override void ValueUpdated(DateTime newValue)
        {
            base.ValueUpdated(newValue);
            this.dateField.Date = newValue;
        }
        
        private void DateValueChanged(object sender, EventArgs args)
        {
            this.Value = (sender as UIDateField).Date;
        }
        
        private void ConfigureCell ()
        {
            this.SelectionStyle = UITableViewCellSelectionStyle.None;
   
            this.dateField = new UIDateField(this.CalculateTextFieldFrame(this.Text))
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleLeftMargin,
            };
            
            this.ContentView.AddSubview(this.dateField);

            var proxy = new EventProxy<DateInputElementTableViewCell, EventArgs>(this);
            proxy.Handle = (t,s,o) => { t.Value = ((UIDateField)s).Date; };
            this.dateField.ValueChanged += proxy.HandleEvent;
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
    
    public class DecimalInputElementTableViewCell : ElementTableViewCell<decimal>
    {       
        private UITextField textField;
        
        public DecimalInputElementTableViewCell() : base(UITableViewCellStyle.Default, new NSString("DecimalInputElement"))
        {
            this.ConfigureCell();
        }
        
        public DecimalInputElementTableViewCell(UITableViewCellStyle style, NSString reuseIdentifer)
            : base(style, reuseIdentifer)
        {
            this.ConfigureCell();
        }
        
        public override void TouchesEnded (NSSet touches, UIEvent evt)
        {
            base.TouchesEnded (touches, evt);
            if (evt.Type == UIEventType.Touches)
            {
                this.textField.BecomeFirstResponder();
            }
        }
        
        public override bool BecomeFirstResponder()
        {
            return this.textField.BecomeFirstResponder();
        }
                
        protected override void Dispose (bool disposing)
        {
            if (disposing)
            {
                this.textField.Dispose();
                this.textField = null;
            }
            
            base.Dispose (disposing);
        }
        
        protected override void TextUpdated (string newValue)
        {
            base.TextUpdated (newValue);
            this.textField.Frame = this.CalculateTextFieldFrame(newValue);
            this.ContentView.BringSubviewToFront(this.textField);
        }

        protected override void ValueUpdated(decimal newValue)
        {
            base.ValueUpdated(newValue);
            this.textField.Text = string.Format("{0}", newValue);
        }

        private void ConfigureCell ()
        {
            this.SelectionStyle = UITableViewCellSelectionStyle.None;
   
            this.textField = new UITextField(this.CalculateTextFieldFrame(this.Text))
            {
                KeyboardType = UIKeyboardType.DecimalPad,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleLeftMargin,
            };
            
            this.ContentView.AddSubview(this.textField);

            var proxy = new EventProxy<DecimalInputElementTableViewCell, EventArgs>(this);

            proxy.Handle = (t,s,o) => 
            { 
                try
                {
                    t.Value = Convert.ToDecimal(((UITextField)s).Text); 
                }
                catch
                {
                    t.Value = 0;
                }
            };

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
        
    public class CustomElementTableViewCell : ElementTableViewCell<string>
    {       
        private UIDateField textField;
        
        public CustomElementTableViewCell() : base(UITableViewCellStyle.Default, new NSString("CustomElement"))
        {
            this.ConfigureCell();
        }
        
        public CustomElementTableViewCell(UITableViewCellStyle style, NSString reuseIdentifer)
            : base(style, reuseIdentifer)
        {
            this.ConfigureCell();
        }
        
        public override void TouchesEnded (NSSet touches, UIEvent evt)
        {
            base.TouchesEnded (touches, evt);
            if (evt.Type == UIEventType.Touches)
            {
                if (this.textField.CanBecomeFirstResponder)
                {
                    this.textField.BecomeFirstResponder();
                }
            }
        }
        
        protected override void ValueUpdated(string newValue)
        {
            base.ValueUpdated(newValue);
        }
        
        protected override void TextUpdated (string newValue)
        {
            base.TextUpdated (newValue);
            this.textField.Frame = this.CalculateTextFieldFrame(newValue);
            this.ContentView.BringSubviewToFront(this.textField);
        }
         
        protected override void Dispose (bool disposing)
        {
            if (disposing)
            {
                this.textField.Dispose();
                this.textField = null;
            }
            
            base.Dispose (disposing);
        }
        
        private void ConfigureCell ()
        {
            this.SelectionStyle = UITableViewCellSelectionStyle.None;
   
            this.textField = new UIDateField(this.CalculateTextFieldFrame(this.Text))
            {
                InputAccessoryView = new UILabel(new RectangleF(0, 0, 100, 50)),
            };
            
            this.textField.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleLeftMargin;
            
            this.ContentView.AddSubview(this.textField);
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

