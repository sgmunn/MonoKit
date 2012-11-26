//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="TableViewCell.cs" company="sgmunn">
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
    using System.ComponentModel;
    using MonoKit.DataBinding;
    using MonoTouch.UIKit;

    public class TableViewCell : UITableViewCell, INotifyPropertyChanged, IBindingScope, IPropertyInjection
    {
        private readonly BindingScope bindings;

        private string text;

        public TableViewCell(UITableViewCellStyle style, string reuseIdentifer)
            : base(style, reuseIdentifer)
        {
            this.InjectedProperties = new InjectedPropertyStore(this);
            this.bindings = new BindingScope();
        }

        ~TableViewCell()
        {
            Console.WriteLine("~TableViewCell {0}", this.GetType().Name);
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        public string Text
        {
            get
            {
                return this.text;
            }
            
            set
            {
                if (value != this.text)
                {
                    this.text = value;
                    this.NotifyPropertyChanged("Text");
                    this.TextChanged(value);
                }
            }
        }

        public IInjectedPropertyStore InjectedProperties
        {
            get;
            private set;
        }

        public void AddBinding(IBindingExpression expression)
        {
            this.bindings.AddBinding(expression);
        }

        public void RemoveBinding(IBindingExpression expression)
        {
            this.bindings.RemoveBinding(expression);
        }

        public void ClearBindings()
        {
            this.bindings.ClearBindings();
        }

        public IBindingExpression[] GetBindingExpressions()
        {
            return this.bindings.GetBindingExpressions();
        }
        
        protected virtual void TextChanged(string newValue)
        {
            this.TextLabel.Text = newValue;
            // this.LayoutSubviews();
            //this.SetNeedsDisplay();
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            var ev = this.PropertyChanged;
            if (ev != null)
            {
                ev(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
