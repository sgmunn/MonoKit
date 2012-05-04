namespace MonoKit.UI
{
    using System;

    public interface IDataViewWrapper
    {
        IViewDefinition ViewDefinition { get; }
        object Data { get; }
    }
}

