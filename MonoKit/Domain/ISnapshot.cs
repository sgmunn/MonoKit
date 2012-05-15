namespace MonoKit.Domain
{
    using System;
    
    public interface ISnapshot
    {
        Guid Id { get; set; }
        int Version { get; set; }
    }
}

