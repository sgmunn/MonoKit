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
    
    /// <summary>
    /// Represents the definition of an property that can be attached to another object
    /// </summary>
    public class AttachedProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.AttachedProperty"/> class.
        /// </summary>
        private AttachedProperty()
        {
        }
        
        /// <summary>
        /// Registers an AttachedProperty
        /// </summary>
        /// <param name='propertyName'>
        /// The name of the property
        /// </param>
        /// <param name='propertyType'>
        /// The type of the property
        /// </param>
        /// <param name='ownerType'>
        /// The owning type of the property
        /// </param>
        public static AttachedProperty Register(
            string propertyName,
            Type propertyType,
            Type ownerType
            )
        {
            return new  AttachedProperty
            {
                PropertyName = propertyName,
                PropertyType = propertyType,
                OwnerType = ownerType,
                Metadata = new AttachedPropertyMetadata(),
            };
        }
        
        /// <summary>
        /// Registers an AttachedProperty
        /// </summary>
        /// <param name='propertyName'>
        /// The name of the property
        /// </param>
        /// <param name='propertyType'>
        /// The type of the property
        /// </param>
        /// <param name='ownerType'>
        /// The owning type of the property
        /// </param>
        /// <param name='metadata'>Additional Metadata about the property</param
        public static AttachedProperty Register(
            string propertyName,
            Type propertyType,
            Type ownerType,
            AttachedPropertyMetadata metadata
            )
        {
            return new  AttachedProperty
            {
                PropertyName = propertyName,
                PropertyType = propertyType,
                OwnerType = ownerType,
                Metadata = metadata,
            };
        }
        
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string PropertyName {get; private set;}
        
        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public Type PropertyType {get; private set;}

        /// <summary>
        /// Gets the type of the owner of the property
        /// </summary>
        public Type OwnerType {get; private set;}

        /// <summary>
        /// Gets the metadata about the property
        /// </summary>
        public AttachedPropertyMetadata Metadata {get; private set;}
        
        /// <summary>
        /// Gets the key used to index the property
        /// </summary>
        internal string PropertyKey
        {
            get
            {
                return string.Format("{0}-{1}", OwnerType, PropertyName);
            }
        }
    }
    
}

