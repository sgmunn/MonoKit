namespace MonoKit.Interactivity
{
    using System;
    using MonoKit.DataBinding;
    
    /// <summary>
    /// Defines an instance of an abstract behaviour that is attached to an arbitrary object.
    /// </summary>
    public abstract class Behaviour
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.Behaviour"/> class.
        /// </summary>
        public Behaviour()
        {
        }

        /// <summary>
        /// Defines the property that attaches the behaviour to the target object
        /// </summary>
        /// <value>
        /// The attached object property.
        /// </value>
        public static AttachedProperty AttachedObjectProperty
        {
            get
            {
                return AttachedProperty.Register("AttachedProperty", typeof(object), typeof(Behaviour), new AttachedPropertyMetadata(AttachedPropertyChanged));
            }
        }
        
        /// <summary>
        /// Handles changes to the attached object and calls Attach or Detatch
        /// </summary>
        /// <param name='targetObject'>
        /// The object to which we are attaching the behaviour
        /// </param>
        /// <param name='e'>
        /// The changed property arguments
        /// </param>
        public static void AttachedPropertyChanged(object targetObject, AttachedPropertyChangedEventArgs e)
        {
            var behaviour = targetObject as Behaviour;
            if (behaviour != null)
            {
                if (e.OldValue != null)
                {
                    behaviour.Detach(e.OldValue);
                }
                
                behaviour.Attach(e.NewValue);
            }
        }
        
        /// <summary>
        /// Gets the object that the behaviour is attached to.
        /// </summary>
        /// <value>
        /// The attached object.
        /// </value>
        public Object AttachedObject
        {
            get
            {
                return this.GetValue(AttachedObjectProperty);
            }
            
            set
            {
                this.SetValue(AttachedObjectProperty, value);
            }
        }
        
        /// <summary>
        /// Attach the behaviour to the specified instance.
        /// </summary>
        /// <param name='instance'>
        /// The object to attach this behaviour to.
        /// </param>
        protected void Attach(object instance)
        {
            this.OnAttach(instance);
        }
        
        /// <summary>
        /// Dettach this instance from the behaviour.
        /// </summary>
        protected void Detach(object instance)
        {
            this.OnDetach(instance);
        }
        
        /// <summary>
        /// Is called when the behaviour attaches to the instance.
        /// </summary>
        /// <param name='instance'>
        /// The object that has just been attached.
        /// </param>
        protected abstract void OnAttach(object instance);
        
        /// <summary>
        /// Is called when the behaviour dettaches from the instance.
        /// </summary>
        /// <param name='instance'>
        /// The object that has just been dettached.
        /// </param>
        protected abstract void OnDetach(object instance);
    }
}

