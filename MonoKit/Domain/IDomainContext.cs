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

        IEventSerializer EventSerializer { get; }
        
        IUnitOfWorkScope BeginUnitOfWork();

        IDomainCommandExecutor<T> NewCommandExecutor<T>() where T : class, IAggregateRoot, new();
        
        IAggregateRepository<T> AggregateRepository<T>() where T : IAggregateRoot, new();

        IList<IReadModelBuilder> GetReadModelBuilders(Type aggregateType);
    }
}

