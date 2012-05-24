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

    /// <summary>
    /// Defines a binding between two properties
    /// </summary>
    public class BindingDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.BindingDefinition"/> class.
        /// </summary>
        /// <param name='property'>
        /// The name of the property to create a binding definition for.  The target and source properties are assumed to be the
        /// same name.
        /// </param>
        public BindingDefinition(string property)
        {
            this.PropertyName = property;
            this.Binding = new Binding(property);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.BindingDefinition"/> class.
        /// </summary>
        /// <param name='targetProperty'>
        /// The name of the target property to bind to.
        /// </param>
        /// <param name='sourceProperty'>
        /// The name of the source property to bind to.
        /// </param>
        public BindingDefinition(string targetProperty, string sourceProperty)
        {
            this.PropertyName = targetProperty;
            this.Binding = new Binding(sourceProperty);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.BindingDefinition"/> class.
        /// </summary>
        /// <param name='targetProperty'>
        /// The name of the target property to bind to.
        /// </param>
        /// <param name='binding'>
        /// The source binding.
        /// </param>
        public BindingDefinition(string targetProperty, Binding binding)
        {
            this.PropertyName = targetProperty;
            this.Binding = binding;
        }
        
        /// <summary>
        /// Gets the name of the target property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets the source binding.
        /// </summary>
        /// <value>
        /// The binding.
        /// </value>
        public Binding Binding
        {
            get;
            private set;
        }
    }
}

