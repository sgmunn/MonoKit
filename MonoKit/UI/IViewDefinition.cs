namespace MonoKit.UI
{
    using System;
    using System.Collections.Generic;

    public interface IViewDefinition
    {
        Type ViewType { get; }
        void Bind(object target, object dataContext);
        object Param { get; }
        List<Type> Behaviours { get; }
        bool Renders(object data);
    }
}

