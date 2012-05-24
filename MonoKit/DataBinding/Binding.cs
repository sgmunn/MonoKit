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

namespace MonoKit.DataBinding
{
    using System;
    using System.Reflection;
    using System.Globalization;
    
    /// <summary>
    /// Binding mode.
    /// </summary>
    public enum BindingMode
    {
        TwoWay,
        OneWay,
        OneWayToSource
    }
 
    /// <summary>
    /// Defines a binding to a property.
    /// </summary>
    public class Binding : IDisposable
    {
        public Binding(string propertyName)
        {
            this.Mode = BindingMode.TwoWay;
            this.PropertyName = propertyName;
        }
        
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
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
        
        /// <summary>
        /// Gets or sets the converter for converting the value of the property
        /// </summary>
        /// <value>
        /// The converter.
        /// </value>
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
        
        public object GetSourceValue(object sourceObject, Type targetType)
        {
            if (sourceObject == null)
            {
                return null;
            }
            
            var result = this.InternalGetSourceValue(sourceObject, targetType);
            return this.ConvertValue(result, targetType);
        }
        
        public void UpdateSourceValue(object sourceObject, object newValue)
        {
            if (sourceObject == null)
            {
                return;
            }
            
            newValue = this.ConvertBackValue(newValue, sourceObject.GetPropertyInfo(this.PropertyName).GetType());
            
            this.InternalSetSourceValue(sourceObject, newValue);
        }
        
        public virtual void Dispose()
        {
        }
        
        protected virtual object InternalGetSourceValue(object sourceObject, Type targetType)
        {
            return GetSourceValue(sourceObject, sourceObject.GetPropertyInfo(this.PropertyName));
        }
        
        protected virtual void InternalSetSourceValue(object sourceObject, object newValue)
        {
            SetSourceValue(sourceObject, sourceObject.GetPropertyInfo(this.PropertyName), newValue);
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
            object convertedValue = value;
            
            if (this.Converter != null)
            {
                convertedValue = this.Converter.ConvertBack(value, targetType, this.ConverterParameter, CultureInfo.CurrentUICulture);
            }
            
            return convertedValue;
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
    
    public class Binding<T> : Binding
    {
        public Binding(string propertyName) : base(propertyName)
        {
        }
        
        public Func<T, object> GetValue
        {
            get;
            set;
        }
        
        public Action<T, object> SetValue
        {
            get;
            set;
        }
        
        public override void Dispose()
        {
            base.Dispose();
            this.GetValue = null;
            this.SetValue = null;
        }
        
        protected override object InternalGetSourceValue(object sourceObject, Type targetType)
        {
            if (this.GetValue != null && sourceObject is T)
            {
                return this.GetValue((T)sourceObject);
            }
            
            return base.InternalGetSourceValue(sourceObject, targetType);
        }
        
        protected override void InternalSetSourceValue(object sourceObject, object newValue)
        {
            if (this.SetValue != null && sourceObject is T)
            {
                this.SetValue((T)sourceObject, newValue);
                return;
            }
            
            base.InternalSetSourceValue(sourceObject, newValue);
        }
    }
}

