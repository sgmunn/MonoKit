// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Behaviour.cs" company="sgmunn">
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
    /// Defines an instance of an abstract behaviour that is attached to an arbitrary object.
    /// </summary>
    public abstract class Behaviour
    {
        private object attachedObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.Behaviour"/> class.
        /// </summary>
        protected Behaviour()
        {
        }

        /// <summary>
        /// Gets the object that the behaviour is attached to.
        /// </summary>
        public object AttachedObject
        {
            get
            {
                return this.attachedObject;
            }
            
            set
            {
                if (value != this.attachedObject)
                {
                    if (this.attachedObject != null)
                    {
                        this.Detach(this.attachedObject);
                    }

                    this.attachedObject = value;

                    if (this.attachedObject != null)
                    {
                        this.Attach(this.attachedObject);
                    }
                }
            }
        }
        
        /// <summary>
        /// Attach the behaviour to the specified instance.
        /// </summary>
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
        protected abstract void OnAttach(object instance);
        
        /// <summary>
        /// Is called when the behaviour dettaches from the instance.
        /// </summary>
        protected abstract void OnDetach(object instance);
    }
}

