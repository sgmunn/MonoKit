namespace MonoKit.Domain
{
    using System;
    
    public interface ISnapshot
    {
        int Version { get; }
    }
}

