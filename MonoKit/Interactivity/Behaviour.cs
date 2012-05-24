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

