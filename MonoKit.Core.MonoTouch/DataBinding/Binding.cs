// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Binding.cs" company="sgmunn">
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

namespace MonoKit.DataBinding
{
    using System;
    using System.Reflection;
    using System.Globalization;

    /// <summary>
    /// Defines a binding to a (source) property.
    /// </summary>
    public sealed class Binding : IDisposable
    {
        public Binding(string propertyName)
        {
            this.Mode = BindingMode.TwoWay;
            this.PropertyName = propertyName;
        }

        public Binding(string propertyName, BindingAssistant bindingAssistant)
        {
            this.Mode = BindingMode.TwoWay;
            this.PropertyName = propertyName;
            this.BindingAssistant = bindingAssistant;
        }

        public string PropertyName
        {
            get;
            private set;
        }
        
        public BindingMode Mode
        {
            get;
            set;
        }
        
        public IValueConverter Converter 
        { 
            get; 
            set; 
        }
        
        public object ConverterParameter
        {
            get;
            set;
        }

        public BindingAssistant BindingAssistant { get; private set; }

        public object GetSourceValue(object sourceObject, Type targetType)
        {
            if (sourceObject == null)
            {
                return null;
            }

            if (this.BindingAssistant != null && this.BindingAssistant.PropertyGetter != null)
            {
                return this.ConvertValue(this.BindingAssistant.PropertyGetter(sourceObject), targetType);
            }
            else
            {
            
            var result = GetSourceValue(sourceObject, sourceObject.GetPropertyInfo(this.PropertyName));
            return this.ConvertValue(result, targetType);
            }
        }
        
        public void UpdateSourceValue(object sourceObject, object newValue)
        {
            if (sourceObject == null)
            {
                return;
            }

            if (this.Converter != null)
            {
                var propInfo = sourceObject.GetPropertyInfo(this.PropertyName);
                newValue = this.ConvertBackValue(newValue, propInfo.GetType());
            }

            if (this.BindingAssistant != null && this.BindingAssistant.PropertySetter != null)
            {
                this.BindingAssistant.PropertySetter(sourceObject, newValue);
            }
            else
            {
                SetSourceValue(sourceObject, sourceObject.GetPropertyInfo(this.PropertyName), newValue);
            }
        }
        
        public void Dispose()
        {
        }
        
        private object ConvertValue(object value, Type targetType)
        {
            object result = value;
            
            if (this.Converter != null)
            {
                result = this.Converter.Convert(result, targetType, this.ConverterParameter, CultureInfo.CurrentUICulture);
            }
            
            return result;
        }
                
        private object ConvertBackValue(object value, Type targetType)
        {
            return this.Converter.ConvertBack(value, targetType, this.ConverterParameter, CultureInfo.CurrentUICulture);
        }

        private static object GetSourceValue(object o, PropertyInfo propertyInfo)
        {
            if (o != null && propertyInfo != null)
            {
                return propertyInfo.GetValue(o, null);
            }
            
            return null;
        }

        private static void SetSourceValue(object o, PropertyInfo propertyInfo, object newValue)
        {
            if (o != null && propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(o, newValue, null);
            }
        }
    }
}

