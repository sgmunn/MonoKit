namespace MonoKit.UI.Elements
{
    using System;
    
    public class ElementDataViewWrapper : Element, IDataViewWrapper
    {
        public ElementDataViewWrapper(IViewDefinition viewDefintion, object data) : base(data, null)
        {
            this.ViewDefinition = viewDefintion;
        }
        
        public IViewDefinition ViewDefinition
        {
            get;
            private set;
        }
    }
}

