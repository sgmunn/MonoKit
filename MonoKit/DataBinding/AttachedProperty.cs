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

