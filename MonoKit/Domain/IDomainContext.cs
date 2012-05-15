namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;
    using MonoKit.Data;
    using MonoKit.Domain.Data;

    public interface IDomainContext
    {
        IEventStoreRepository EventStore { get; }
        
        IDomainEventBus EventBus { get; }

        ISerializer Serializer { get; }
        
        IUnitOfWorkScope GetScope();
        
        IAggregateRepository<T> AggregateRepository<T>() where T : IAggregateRoot, new();

        IList<IDenormalizer> GetDenormalizers(Type aggregateType);
    }
}

