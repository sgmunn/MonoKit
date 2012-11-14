// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingOperations.cs" company="sgmunn">
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
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Methods for adding bindings between objects
    /// </summary>
    public static class BindingOperations
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
        /// Sets a binding between target and source.
        /// </summary>
        public static BindingExpression AddBinding(this IBindingScope target, object source, string propertyName)
        {
            return target.AddBinding(propertyName, source, new Binding(propertyName));
        }
        
        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        public static BindingExpression AddBinding(this IBindingScope target, string targetPropertyName, object source, string sourcePropertyName)
        {
            return target.AddBinding(targetPropertyName, source, new Binding(sourcePropertyName));
        }
  
        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        public static BindingExpression AddBinding(this IBindingScope target, object source, Binding binding)
        {
            return target.AddBinding(binding.PropertyName, source, binding);
        }
        
        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        public static BindingExpression AddBinding(this IBindingScope target, string propertyName, object source, Binding binding)
        {
            var expression = new BindingExpression(target, propertyName, source, binding);
            target.AddBinding(expression);
            return expression;
        }
        
        /// <summary>
        /// Sets a binding between target and source.
        /// </summary>
        public static BindingExpression AddBinding(this IPropertyInjection target, object source, string propertyName)
        {
            var scope = target.GetBindingScopeOrDefault();

            return scope.AddBinding(propertyName, source, new Binding(propertyName));
        }
        
        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        public static BindingExpression AddBinding(this IPropertyInjection target, string targetPropertyName, object source, string sourcePropertyName)
        {
            var scope = target.GetBindingScopeOrDefault();
            
            return scope.AddBinding(targetPropertyName, source, new Binding(sourcePropertyName));
        }
        
        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        public static BindingExpression AddBinding(this IPropertyInjection target, object source, Binding binding)
        {
            var scope = target.GetBindingScopeOrDefault();
            
            return scope.AddBinding(binding.PropertyName, source, binding);
        }
        
        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        public static BindingExpression AddBinding(this IPropertyInjection target, string propertyName, object source, Binding binding)
        {
            var scope = target.GetBindingScopeOrDefault();

            var expression = new BindingExpression(target, propertyName, source, binding);
            scope.AddBinding(expression);
            return expression;
        }
        
        /// <summary>
        /// Replaces the source object that is bound to target.
        /// </summary>
        public static void ReplaceBindingSource(this IBindingScope target, object source)
        {
            target.ReplaceBindingSource(source, string.Empty);
        }

        /// <summary>
        /// Replaces the source object that is bound to target.
        /// </summary>
        public static void ReplaceBindingSource(this IBindingScope target, object source, string targetPropertyName)
        {
            var bindingExpressions = target.GetBindingExpressions();
            
            var newExpressions = new List<BindingExpression>();
            
            foreach (var expression in bindingExpressions.Where(x => string.IsNullOrEmpty(targetPropertyName) || x.TargetProperty.Equals(targetPropertyName)))
            {
                var newExpression = new BindingExpression(target, expression.TargetProperty, source, expression.Binding);
                newExpressions.Add(newExpression);
            }
            
            target.ClearBindings();
            foreach (var expression in newExpressions)
            {
                target.AddBinding(expression);
            }
        }

        /// <summary>
        /// Gets the binding expression for a target
        /// </summary>
        public static BindingExpression[] GetBindingExpressions(this IBindingScope target, string propertyName)
        {
            var bindingExpressions = target.GetBindingExpressions();

            return bindingExpressions.Where(bx => bx.TargetProperty.Equals(propertyName) && bx.Target == target).ToArray();
        } 
        
        /// <summary>
        /// Updates any source object that is bound to the target's property
        /// </summary>
        public static void UpdateBindingSource(this IBindingScope target, string propertyName)
        {
            var bx = target.GetBindingExpressions(propertyName);
            foreach (var expression in bx)
            {
                expression.UpdateSource();
            }
        }
    }
}

