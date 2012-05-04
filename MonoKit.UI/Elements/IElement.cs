namespace MonoKit.UI.Elements
{
    using System;

    public interface IElement
    {
        string Text { get; set; }
    }
    
    public interface IElement<T> : IElement
    {
        T Value { get; set; }
    }
}
