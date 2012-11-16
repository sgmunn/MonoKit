//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="OneTimeBindingExtensions.cs" company="sgmunn">
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

    public static class OneTimeBindingExtensions
    {
        /// <summary>
        /// Sets a one-time binding between target and source.
        /// </summary>
        public static void OneTimeBind(this object target, object source, string propertyName)
        {
            target.OneTimeBind(propertyName, source, new Binding(propertyName));
        }
        
        /// <summary>
        /// Sets a one-time binding between target and source.
        /// </summary>
        public static void OneTimeBind(this object target, string targetPropertyName, object source, string sourcePropertyName)
        {
            target.OneTimeBind(targetPropertyName, source, new Binding(sourcePropertyName));
        }
        
        /// <summary>
        /// Sets a one-time binding between target and source.
        /// </summary>
        public static void OneTimeBind(this object target, object source, Binding binding)
        {
            target.OneTimeBind(binding.PropertyName, source, binding);
        }
        
        /// <summary>
        /// Sets a one-time binding between target and source.
        /// </summary>
        public static void OneTimeBind(this object target, string propertyName, object source, Binding binding)
        {
            var expression = new BindingExpression(target, propertyName, source, binding);
            expression.Dispose();
        }
        
        /// <summary>
        /// Sets a one-time binding between target and source.
        /// </summary>
        public static void OneTimeBind(this object target, string propertyName, IPropertyAccessor accessor, object source, Binding binding)
        {
            var expression = new BindingExpression(target, propertyName, accessor, source, binding);
            expression.Dispose();
        }
    }
}
