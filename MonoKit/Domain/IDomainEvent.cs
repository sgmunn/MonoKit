namespace MonoKit.Domain
{
    using System;

    public interface IDomainEvent
    {
        Type AggregateType { get; }
        IEvent Event { get; }
    }
}

