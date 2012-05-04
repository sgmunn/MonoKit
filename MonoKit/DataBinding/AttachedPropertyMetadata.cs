namespace MonoKit.DataBinding
{
    using System;
 
    /// <summary>
    /// Represents additional metadata about an AttachedProperty.
    /// </summary>
    public class AttachedPropertyMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.AttachedPropertyMetadata"/> class.
        /// </summary>
        public AttachedPropertyMetadata()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.AttachedPropertyMetadata"/> class.
        /// </summary>
        /// <param name='defaultValue'>
        /// The default value to return if a value has not been set for an object
        /// </param>
        public AttachedPropertyMetadata(object defaultValue)
        {
            this.DefaultValue = defaultValue;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.AttachedPropertyMetadata"/> class.
        /// </summary>
        /// <param name='changeCallback'>
        /// Callback for when the value is changed for an object
        /// </param>
        public AttachedPropertyMetadata(PropertyChangedCallback changeCallback)
        {
            this.ChangeCallback = changeCallback;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.AttachedPropertyMetadata"/> class.
        /// </summary>
        /// <param name='defaultValue'>
        /// The default value to return if a value has not been set for an object
        /// </param>
        /// <param name='changeCallback'>
        /// Callback for when the value is changed for an object
        /// </param>
        public AttachedPropertyMetadata(object defaultValue, PropertyChangedCallback changeCallback)
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

