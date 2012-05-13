namespace MonoKit.Domain.Data
{
    using System;

    public interface IEventStoreContract
    {
        Guid EventId { get; set; }

        Guid AggregateId { get; set; }

        int Version { get; set; }

        string Event { get; set; }
    }
}

