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
        
        IUnitOfWorkScope GetScope();
        
        ISerializer Serializer { get; }
        
        void RegisterDenormalizer(Type aggregateType, Type denormalizer);
        
        IList<IDenormalizer> GetDenormalizers(Type aggregateType);
    }
}

