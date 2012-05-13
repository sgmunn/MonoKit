namespace MonoKit.Domain
{
    using System;

    public class DomainEvent : IDomainEvent
    {
        public Type AggregateType { get; set; }
        public IEvent Event { get; set; }
    }
}

