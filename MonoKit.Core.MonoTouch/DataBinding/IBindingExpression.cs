//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="IBindingExpression.cs" company="sgmunn">
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

    public interface IBindingExpression : IDisposable
    {
        /// <summary>
        /// Gets the target of the binding
        /// </summary>
        object Target { get; }
        
        /// <summary>
        /// Gets the name of the target property.
        /// </summary>
        string TargetProperty { get; }
        
        /// <summary>
        /// Gets the source of the binding.
        /// </summary>
        object Source { get; }
        
        /// <summary>
        /// Gets the binding for Source.
        /// </summary>
        Binding Binding { get; }
        
        /// <summary>
        /// Gets or sets the property accessor for getting and setting property values
        /// </summary>
        IPropertyAccessor PropertyAccessor { get; set; }
        
        /// <summary>
        /// Updates the target object from the source object.
        /// </summary>
        void UpdateTarget(object sourceObject);
        
        /// <summary>
        /// Updates the source object from the Target object.
        /// </summary>
        void UpdateSource(object targetObject);
        
        /// <summary>
        /// Updates the source object from the Target object.
        /// </summary>
        void UpdateSource();
    }
}
