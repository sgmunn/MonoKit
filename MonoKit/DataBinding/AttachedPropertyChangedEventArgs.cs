namespace MonoKit.DataBinding
{
    using System;
 
    /// <summary>
    /// Occurs when the attached property value changes
    /// </summary>
    public class AttachedPropertyChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.AttachedPropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name='property'>
        /// The AttachedProperty that changed value
        /// </param>
        /// <param name='newValue'>
        /// The new value of the property
        /// </param>
        /// <param name='oldValue'>
        /// The old value of the property
        /// </param>
        public AttachedPropertyChangedEventArgs(AttachedProperty property, object newValue, object oldValue)
        {
            this.Property = property;
            this.NewValue = newValue;
            this.OldValue = oldValue;
        }
        
        /// <summary>
        /// Gets the new value of the property
        /// </summary>
        public object NewValue {get; private set;}

        /// <summary>
        /// Gets the old value of the property
        /// </summary>
        public object OldValue {get; private set;}
        
        /// <summary>
        /// Gets the AttachedProperty that changed value
        /// </summary>
        public AttachedProperty Property {get; private set;}
    }
}

