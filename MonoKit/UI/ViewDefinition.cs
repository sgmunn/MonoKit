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

