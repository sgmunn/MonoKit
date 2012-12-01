//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="BooleanTableViewCell.cs" company="sgmunn">
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
    using MonoTouch.UIKit;
    
    public class BooleanTableViewCell : TableViewCell
    {    
        private bool isChecked;

        private UISwitch boolSwitch;

        private IDisposable eventHandler;
        
        public BooleanTableViewCell(UITableViewCellStyle style, string reuseIdentifer)
            : base(style, reuseIdentifer)
        {
            this.ConfigureCell();
        }
        
        public UISwitch Switch
        {
            get
            {
                return this.boolSwitch;
            }
        }   

        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }
            
            set
            {
                if (value != this.isChecked)
                {
                    this.isChecked = value;
                    this.NotifyPropertyChanged("IsChecked");
                    this.CheckedChanged(value);
                }
            }
        }
        
        protected virtual void CheckedChanged(bool newValue)
        {
            this.boolSwitch.On = newValue;
        }

        protected override void Dispose (bool disposing)
        {
            if (disposing)
            {
                this.eventHandler.Dispose();
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
            var weakHandler = new WeakEventWrapper<BooleanTableViewCell, EventArgs>(this, (t,s,a) => {t.IsChecked = t.boolSwitch.On; });
            this.boolSwitch.ValueChanged += weakHandler.HandleEvent;
            this.eventHandler = weakHandler;
        }
    }
}
