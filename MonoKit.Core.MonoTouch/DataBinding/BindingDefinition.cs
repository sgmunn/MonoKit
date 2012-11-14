// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingDefinition.cs" company="sgmunn">
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
    /// Defines a binding between a target property and a binding to a source property
    /// </summary>
    public sealed class BindingDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.BindingDefinition"/> class.
        /// </summary>
        public BindingDefinition(string property)
        {
            this.PropertyName = property;
            this.Binding = new Binding(property);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.BindingDefinition"/> class.
        /// </summary>
        public BindingDefinition(string targetProperty, string sourceProperty)
        {
            this.PropertyName = targetProperty;
            this.Binding = new Binding(sourceProperty);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.BindingDefinition"/> class.
        /// </summary>
        public BindingDefinition(string targetProperty, Binding binding)
        {
            this.PropertyName = targetProperty;
            this.Binding = binding;
        }
        
        public string PropertyName
        {
            get;
            private set;
        }
        
        public Binding Binding
        {
            get;
            private set;
        }
    }
}

