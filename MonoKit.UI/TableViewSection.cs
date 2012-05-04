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

    public class TableViewSection : TableViewSection<Element>
    {
        private static IViewDefinition[] ViewDefinitions()
        {
            return new IViewDefinition[]
            {
                DefaultElementViewDefintions.StringElementViewDefintion,
                DefaultElementViewDefintions.SubtitleStringElementViewDefintion,
                DefaultElementViewDefintions.Value1StringElementViewDefintion,
                DefaultElementViewDefintions.Value2StringElementViewDefintion,
                DefaultElementViewDefintions.DisclosureElementViewDefintion,
                DefaultElementViewDefintions.BooleanElementViewDefintion,
                DefaultElementViewDefintions.CheckboxElementViewDefintion,
                DefaultElementViewDefintions.TextInputElementViewDefintion,
                DefaultElementViewDefintions.PasswordInputElementViewDefintion,
                DefaultElementViewDefintions.DateInputElementViewDefintion,
            };
        }
        
        public TableViewSection(TableViewSource source) : base(source, ViewDefinitions())
        {
        }
    }
}

