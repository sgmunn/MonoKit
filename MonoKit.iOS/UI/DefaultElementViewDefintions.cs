//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file=".cs" company="sgmunn">
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
//

namespace MonoKit.UI
{
    using System;
    using MonoKit.DataBinding;
    using MonoKit.UI.Controls;
    using MonoKit.UI.Elements;
    using MonoTouch.UIKit;
    
    public static class DefaultElementViewDefintions
    {
        public static IViewDefinition StringElementViewDefintion
        {
            get
            {
                return new UIViewDefinition<StringElementTableViewCell, StringElement>(SimpleElementBinding) { Param = UITableViewCellStyle.Default };
            }
        }

        public static IViewDefinition SubtitleStringElementViewDefintion
        {
            get
            {
                return new UIViewDefinition<StringElementTableViewCell, SubtitleStringElement>(SimpleElementBinding) { Param = UITableViewCellStyle.Subtitle };
            }
        }

        public static IViewDefinition Value1StringElementViewDefintion
        {
            get
            {
                return new UIViewDefinition<StringElementTableViewCell, Value1StringElement>(SimpleElementBinding) { Param = UITableViewCellStyle.Value1 };
            }
        }

        public static IViewDefinition Value2StringElementViewDefintion
        {
            get
            {
                return new UIViewDefinition<StringElementTableViewCell, Value2StringElement>(SimpleElementBinding) { Param = UITableViewCellStyle.Value2 };
            }
        }

        public static IViewDefinition DisclosureElementViewDefintion
        {
            get
            {
                return new UIViewDefinition<DisclosureElementTableViewCell, DisclosureElement>(SimpleElementBinding) { Param = UITableViewCellStyle.Default };
            }
        }

        public static IViewDefinition BooleanElementViewDefintion
        {
            get
            {
                return new UIViewDefinition<BooleanElementTableViewCell, BooleanElement>(SimpleElementBinding);
            }
        }

        public static IViewDefinition CheckboxElementViewDefintion
        {
            get
            {
                return new UIViewDefinition<CheckboxElementTableViewCell, CheckboxElement>(SimpleElementBinding);
            }
        }

        public static IViewDefinition TextInputElementViewDefintion
        {
            get
            {
                return new UIViewDefinition<TextInputElementTableViewCell, TextInputElement>(TextInputElementBinding, typeof(TextInputBehaviour));
            }
        }

        public static IViewDefinition PasswordInputElementViewDefintion
        {
            get
            {
                return new UIViewDefinition<PasswordInputElementTableViewCell, PasswordInputElement>(SimpleElementBinding, typeof(TextInputBehaviour));
            }
        }

        public static IViewDefinition DateInputElementViewDefintion
        {
            get
            {
                return new UIViewDefinition<DateInputElementTableViewCell, DateInputElement>(SimpleElementBinding);
            }
        }

        public static IViewDefinition DecimalInputElementViewDefintion
        {
            get
            {
                return new UIViewDefinition<DecimalInputElementTableViewCell, DecimalInputElement>(SimpleElementBinding);
            }
        }
        
        public static void SimpleElementBinding(UIView view, object data)
        {
            view.SetBinding("Text", data, "Text");
            view.SetBinding(data, "Value");  
        }
        
        public static void TestElementBinding(UIView view, object data)
        {
            view.SetBinding("Text", data, new Binding("Value") { Converter = new BooleanToStringConverter()});
            view.SetBinding(data, "Value");  
        }
        
        public static void TextInputElementBinding(UIView view, object data)
        {
            view.SetBinding(data, "Text");
            view.SetBinding(data, "Value");  
            view.SetBinding(data, "Placeholder");  
            view.SetBinding(data, "KeyboardType");  
        }
    }
    
}
