//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ReflectionPropertyAccessor.cs" company="sgmunn">
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

namespace MonoKit.DataBinding
{
    using System;

    public sealed class ReflectionPropertyAccessor : IPropertyAccessor
    {
        public ReflectionPropertyAccessor(string propertyPath)
        {
            if (string.IsNullOrEmpty(propertyPath))
            {
                throw new ArgumentNullException("propertyPath");
            }
            
            this.PropertyPath = propertyPath;
        }
        
        public string PropertyPath { get; private set; }

        public bool CanGetValue(object obj) 
        { 
            if (obj != null)
            {
                var propertyInfo = obj.GetPropertyInfo(this.PropertyPath);
                return (propertyInfo != null && propertyInfo.CanRead);
            }
            
            return false;
        }
        
        public bool CanSetValue(object obj) 
        { 
            if (obj != null)
            {
                var propertyInfo = obj.GetPropertyInfo(this.PropertyPath);
                return (propertyInfo != null && propertyInfo.CanWrite);
            }

            return false;
        }
        
        public object GetValue(object obj)
        {
            if (obj != null)
            {
                var propertyInfo = obj.GetPropertyInfo(this.PropertyPath);
                if (propertyInfo != null && propertyInfo.CanRead)
                {
                    return propertyInfo.GetValue(obj, null);
                }
            }

            return null;
        }
        
        public void SetValue(object obj, object value)
        {
            if (obj != null)
            {
                var propertyInfo = obj.GetPropertyInfo(this.PropertyPath);
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(obj, value, null);
                }
            }
        }
    }
}
