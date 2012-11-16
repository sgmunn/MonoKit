// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectedPropertyBindingExtensions.cs" company="sgmunn">
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

    /// <summary>
    /// Methods for adding bindings between objects
    /// </summary>
    public static class InjectedPropertyBindingExtensions
    {
        /// <summary>
        /// Sets a binding between target and source.
        /// </summary>
        public static IBindingExpression AddBinding(this IPropertyInjection target, object source, string propertyName)
        {
            var scope = target.GetBindingScopeOrDefault();

            return scope.AddBinding(target, propertyName, source, new Binding(propertyName));
        }
        
        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        public static IBindingExpression AddBinding(this IPropertyInjection target, string targetPropertyName, object source, string sourcePropertyName)
        {
            var scope = target.GetBindingScopeOrDefault();
            
            return scope.AddBinding(target, targetPropertyName, source, new Binding(sourcePropertyName));
        }
        
        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        public static IBindingExpression AddBinding(this IPropertyInjection target, object source, Binding binding)
        {
            var scope = target.GetBindingScopeOrDefault();
            
            return scope.AddBinding(target, binding.PropertyName, source, binding);
        }
        
        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        public static IBindingExpression AddBinding(this IPropertyInjection target, string propertyName, object source, Binding binding)
        {
            var scope = target.GetBindingScopeOrDefault();

            var expression = new WeakBindingExpression(target, propertyName, source, binding);
            scope.AddBinding(expression);
            return expression;
        }
    }
}

