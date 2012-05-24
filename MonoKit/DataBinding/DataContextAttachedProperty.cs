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

    public static class DataContextAttachedProperty
    {
        public static AttachedProperty DataContextProperty
        {
            get
            {
                return AttachedProperty.Register("DataContext", typeof(object), typeof(object), new AttachedPropertyMetadata(DataContextChanged));
            }
        }
        
        public static object GetDataContext(this object owner)
        {
            return owner.GetValue(DataContextAttachedProperty.DataContextProperty);
        }
        
        public static void SetDataContext(this object owner, object value)
        {
            owner.SetValue(DataContextAttachedProperty.DataContextProperty, value);
        }
        
        public static void DataContextChanged(object target, AttachedPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                // bind "target" to new value
                // target.Bind(e.NewValue, --template--)
                
                // how do we get the binding information for "target" in an easy manner
                // in xaml this would be defined in the view.  perhaps we do the same
                // by either using attributes on target or having the view register
                // perhaps the view can register define a way of binding
            }
        }
    }
}

