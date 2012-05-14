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
        
        IAggregateRepository<T> AggregateRepository<T>() where T : IAggregateRoot, new();
        
        IUnitOfWorkScope GetScope();
        
        ISerializer Serializer { get; }
        
        void RegisterDenormalizer(Type aggregateType, Type denormalizer);
        
        IList<IDenormalizer> GetDenormalizers(Type aggregateType);
    }
}

