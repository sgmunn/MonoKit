// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectedProperty.cs" company="sgmunn">
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
    /// Represents the definition of an property that can be injected into an object that supports IPropertyInjection
    /// </summary>
    public sealed class InjectedProperty
    {
        private static Dictionary<string, InjectedProperty> registeredProperties = new Dictionary<string, InjectedProperty>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.InjectedProperty"/> class.
        /// </summary>
        private InjectedProperty()
        {
        }
        
        public static InjectedProperty Register(
            string propertyName,
            Type propertyType
            )
        {
            return InjectedProperty.Register(propertyName, propertyType, new InjectedPropertyMetadata());
        }
        
        public static InjectedProperty Register(
            string propertyName,
            Type propertyType,
            InjectedPropertyMetadata metadata
            )
        {
            InjectedProperty property = null;
            if (registeredProperties.TryGetValue(propertyName, out property))
            {
                // check type
                if (propertyType != property.PropertyType)
                {
                    throw new InvalidOperationException(string.Format("InjectedProperty \"{0}\" has already been registered as a different type \"{1}\"", propertyName, property.PropertyType.FullName));
                }
            }
            else
            {
                property = new  InjectedProperty
                {
                    PropertyName = propertyName,
                    PropertyType = propertyType,
                    Metadata = metadata,
                };

                registeredProperties.Add(propertyName, property);
            }

            return property;
        }
        
        public string PropertyName {get; private set;}
        
        public Type PropertyType {get; private set;}

        public InjectedPropertyMetadata Metadata {get; private set;}
    }
}

