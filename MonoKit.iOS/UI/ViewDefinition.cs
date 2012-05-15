namespace MonoKit.UI
{
    using System;
    using System.Collections.Generic;
    using MonoTouch.UIKit;
    
    public class UIViewDefinition<TView, TData> : ViewDefinition<TView> where TView : UIView
    {
        public UIViewDefinition() : base()
        {
            this.ViewType = typeof(TView);
            this.DataType = typeof(TData);
        }
        
        public UIViewDefinition(Action<UIView, object> bindingAction) : base()
        {
            this.ViewType = typeof(TView);
            this.DataType = typeof(TData);
            this.BindToSourceAction = bindingAction;
        }
        
        public UIViewDefinition(params Type[] behaviours) : base(behaviours)
        {
            this.ViewType = typeof(TView);
            this.DataType = typeof(TData);
        }
        
        public UIViewDefinition(Action<UIView, object> bindingAction, params Type[] behaviours) : base(behaviours)
        {
            this.ViewType = typeof(TView);
            this.DataType = typeof(TData);
            this.BindToSourceAction = bindingAction;
        }
    }
}

