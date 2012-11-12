// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectedPropertyMetadata.cs" company="sgmunn">
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
    /// Represents additional metadata about an InjectedProperty.
    /// </summary>
    public class InjectedPropertyMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.InjectedPropertyMetadata"/> class.
        /// </summary>
        public InjectedPropertyMetadata()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.InjectedPropertyMetadata"/> class.
        /// </summary>
        public InjectedPropertyMetadata(object defaultValue)
        {
            this.DefaultValue = defaultValue;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.InjectedPropertyMetadata"/> class.
        /// </summary>
        public InjectedPropertyMetadata(PropertyChangedCallback changeCallback)
        {
            this.ChangeCallback = changeCallback;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.InjectedPropertyMetadata"/> class.
        /// </summary>
        public InjectedPropertyMetadata(object defaultValue, PropertyChangedCallback changeCallback)
        {
            this.DefaultValue = defaultValue;
            this.ChangeCallback = changeCallback;
        }
        
        /// <summary>
        /// Gets or sets the default value to return if a value has not been set for an object
        /// </summary>
        public object DefaultValue
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the callback for when the value is changed for an object
        /// </summary>
        public PropertyChangedCallback ChangeCallback
        {
            get;
            private set;
        }
    }
}

