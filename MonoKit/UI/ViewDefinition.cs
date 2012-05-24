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

namespace MonoKit.UI
{
    using System;
    using System.Collections.Generic;
    using MonoKit.DataBinding;
    
    /// <summary>
    /// Represents the definition of which view type to use for a specific data type and the behaviours to attach
    /// </summary>
    public abstract class ViewDefinition<TView> : IViewDefinition where TView : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.UI.ViewDefinition"/> class.
        /// </summary>
        public ViewDefinition()
        {
            this.Behaviours = new List<Type>();
            this.ViewType = typeof(TView);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.UI.ViewDefinition"/> class.
        /// </summary>
        /// <param name='behaviours'>
        /// The behaviours that are to be constructed and attached to the view when the view is constructed
        /// </param>
        public ViewDefinition(params Type[] behaviours)
        {
            this.Behaviours = new List<Type>(behaviours);
            this.ViewType = typeof(TView);
        }
        
        public ViewDefinition(Action<TView, object> bindingAction, params Type[] behaviours)
        {
            this.Behaviours = new List<Type>(behaviours);
            this.ViewType = typeof(TView);
            this.BindToSourceAction = bindingAction;
        }
        
        /// <summary>
        /// Gets or sets the type of the view that will be used for an instance of DataType
        /// </summary>
        public Type ViewType
        {
            get;
            protected set;
        }
        
        /// <summary>
        /// Gets or sets the type of the data for which the view is to be used.
        /// </summary>
        public Type DataType
        {
            get;
            protected set;
        }
        
        /// <summary>
        /// Gets the behaviours that are to be constructed and attached to the view when the view is constructed.
        /// </summary>
        public List<Type> Behaviours
        {
            get;
            private set;
        }
         
        /// <summary>
        /// Gets or sets the action to perform when the view needs to be bound to the data
        /// </summary>
        public Action<TView, object> BindToSourceAction
        {
            get;
            protected set;
        }
        
        public object Param
        {
            get;
            set;
        }
        
        public void Bind(object target, object dataContext)
        {
            if (this.BindToSourceAction != null && target is TView)
            {
                target.ClearBindings();
                this.BindToSourceAction((TView)target, dataContext);
            }
        }
        
        public bool Renders(object data)
        {
            return this.DataType.IsInstanceOfType(data);            
        }
    }
}

