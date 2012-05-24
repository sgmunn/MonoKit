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
    using System.Collections.Generic;
    using System.Linq;
    
    /// <summary>
    /// Defines Binding operations and maintains binding state.
    /// </summary>
    public static class BindingOperations
    {
        // todo: perform background check for stale bindings
        // todo: add support for binding to attached properties - should be fast compared to SetValue / GetValue thru reflection
        // todo: add support for binding to use lambda's instead of INPC changes, maybe have INPC to trigger updates though.
        
        // prioritise source to target.  perhaps 
        // (o,v) => o.Property = v
        // (o) => o.Property
        
        /// <summary>
        /// The list of all binding expressions.
        /// </summary>
        //private static List<BindingExpression> bindingExpressions = new List<BindingExpression>();
        private static Dictionary<string, List<BindingExpression>> ExpressionDictionary = new Dictionary<string, List<BindingExpression>>();
        
        /// <summary>
        /// Sets a binding between target and source.
        /// </summary>
        /// <returns>
        /// The binding expression.
        /// </returns>
        /// <param name='target'>
        /// The Target to bind to.
        /// </param>
        /// <param name='source'>
        /// The Source to bind to.
        /// </param>
        /// <param name='property'>
        /// The Property name to bind in both target and source.
        /// </param>
        public static BindingExpression SetBinding(this object target, object source, string property)
        {
            return target.SetBinding(property, source, new Binding(property));
        }
        
        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        /// <returns>
        /// The binding expression.
        /// </returns>
        /// <param name='target'>
        /// The Target to bind to.
        /// </param>
        /// <param name='targetProperty'>
        /// The Target property to bind.
        /// </param>
        /// <param name='source'>
        /// The Source to bind to.
        /// </param>
        /// <param name='sourceProperty'>
        /// The Source property to bind.
        /// </param>
        public static BindingExpression SetBinding(this object target, string targetProperty, object source, string sourceProperty)
        {
            return target.SetBinding(targetProperty, source, new Binding(sourceProperty));
        }
  
        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        /// <returns>
        /// The binding expression.
        /// </returns>
        /// <param name='target'>
        /// The Target to bind to.
        /// </param>
        /// <param name='source'>
        /// The Source to bind to.
        /// </param>
        /// <param name='binding'>
        /// The source Binding.
        /// </param>
        public static BindingExpression SetBinding(this object target, object source, Binding binding)
        {
            return target.SetBinding(binding.PropertyName, source, binding);
        }
        
        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        /// <returns>
        /// The binding expression.
        /// </returns>
        /// <param name='target'>
        /// The Target to bind to.
        /// </param>
        /// <param name='property'>
        /// The target Property to bind.
        /// </param>
        /// <param name='source'>
        /// The Source to bind to.
        /// </param>
        /// <param name='binding'>
        /// The source Binding.
        /// </param>
        public static BindingExpression SetBinding(this object target, string property, object source, Binding binding)
        {
            var expression = new BindingExpression(target, property, source, binding);
            
            var key = GetDicionaryKey(target);
            if (!ExpressionDictionary.ContainsKey(key))
            {
                ExpressionDictionary.Add(key, new List<BindingExpression>() { expression });
            }
            else
            {
                ExpressionDictionary[key].Add(expression); 
            }

            return expression;
        }
        
        /// <summary>
        /// Clears all bindings for target
        /// </summary>
        /// <param name='target'>
        /// The Target object to clear bindings for.
        /// </param>
        public static void ClearBindings(this object target)
        {
            var key = GetDicionaryKey(target);
            
            if (ExpressionDictionary.ContainsKey(key))
            {
                var stale = new List<BindingExpression>();
                var bindingExpressions = ExpressionDictionary[key]; 

                foreach (var ex in bindingExpressions)
                {
                    var t = ex.Target;
                    
                    if (t == target)
                    {
                        stale.Add(ex);
                    }
                }
                
                foreach (var expression in stale)
                {
                    expression.Dispose();
                    bindingExpressions.Remove(expression);
                }
                
                if (bindingExpressions.Count == 0)
                {
                    ExpressionDictionary.Remove(key);
                }
            }
        }
        
        public static void ClearBindings(string targetKey)
        {
        }
        
        /// <summary>
        /// Gets the binding expression for a target
        /// </summary>
        /// <returns>
        /// The first binding expression that matches the target and property.
        /// </returns>
        /// <param name='target'>
        /// The Target to get the binding for.
        /// </param>
        /// <param name='property'>
        /// The Property name in target to get the binding for.
        /// </param>
        public static BindingExpression[] GetBindingExpressions(this object target, string property)
        {
            var key = GetDicionaryKey(target);
            
            if (ExpressionDictionary.ContainsKey(key))
            {
                var bindingExpressions = ExpressionDictionary[key];

                // check target explicity, just because the hashcode is the same doesn't make it the same object
                return bindingExpressions.Where(bx => bx.TargetProperty.Equals(property) && bx.Target == target).ToArray();
            }

            return new BindingExpression[0];
        } 
        
        /// <summary>
        /// Updates any source object that is bound to the target's property
        /// </summary>
        /// <param name='target'>
        /// The Target object that should update source.
        /// </param>
        /// <param name='property'>
        /// The name of the Property that source is bound to in target.
        /// </param>
        public static void UpdateSource(this object target, string property)
        {
            var bx = target.GetBindingExpressions(property);
            foreach (var expression in bx)
            {
                expression.UpdateSource();
            }
        }
        
        /// <summary>
        /// Gets the key for the given object.
        /// </summary>
        /// <returns>
        /// Returns a string representing the key for given target
        /// </returns>
        private static string GetDicionaryKey(object target)
        {
            return string.Format("{0}-{1}", target.GetType(), target.GetHashCode());
        }
        
        /// <summary>
        /// Removes any stale bindings that no longer have references to target or source objects.
        /// </summary>
        public static void RemoveStaleBindings()
        {
            var stale = new List<BindingExpression>();
            var keys = ExpressionDictionary.Keys.ToList();
            foreach (var key in keys)
            {
                stale.Clear();
                
                var bindingExpressions = ExpressionDictionary[key];
                foreach (var ex in bindingExpressions)
                {
                    if (ex.Target == null || ex.Source == null)
                    {
                        stale.Add(ex);
                    }
                }
                                
                foreach (var expression in stale)
                {
                    expression.Dispose();
                    bindingExpressions.Remove(expression);
                }
    
                if (ExpressionDictionary[key].Count == 0)
                {
                    ExpressionDictionary.Remove(key);
                }
            }
        }
    }
}

