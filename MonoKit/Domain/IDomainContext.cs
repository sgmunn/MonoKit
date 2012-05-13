namespace MonoKit.Domain
{
    using System;
    using MonoKit.Data;
    using MonoKit.Domain.Data;

    public interface IDomainContext
    {
        IEventStoreRepository EventStore { get; }
        
        IDomainEventBus EventBus { get; }
        
        IUnitOfWorkScope GetScope();
        
        ISerializer Serializer { get; }
    }
}

