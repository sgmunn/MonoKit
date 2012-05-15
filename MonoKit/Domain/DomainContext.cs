namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;
    using MonoKit.Domain.Data;
    using MonoKit.Data;
    
    public abstract class DomainContext : IDomainContext
    {
        private readonly Dictionary<Type, List<Type>> registeredDenormalizers;

        public DomainContext(IEventStoreRepository eventStore, IDomainEventBus eventBus)
        {
            this.EventBus = eventBus;
            this.EventStore = eventStore;
            this.registeredDenormalizers = new Dictionary<Type, List<Type>>();
        }

        public IEventStoreRepository EventStore{ get; private set; }

        public IDomainEventBus EventBus { get; private set; }

        public ISerializer Serializer
        {
            get
            {
                // todo: this means that the serializer can only supprt events - not commands or snapshots, we need all three to be supported
                return new DefaultSerializer<EventBase>();
            }
        }

        public virtual IUnitOfWorkScope GetScope()
        {
            return new DefaultScope();
        }

        public IAggregateRepository<T> AggregateRepository<T>() where T : IAggregateRoot, new()
        {
            // todo: should we decorate the aggregate root with an attribute to denote event sourced vs snapshot only ??
            //return new SnapshotAggregateRepository<T>(this.GetSnapshotRepository(typeof(T)), new EventBus<T>(this));
            
            return new AggregateRepository<T>(this.Serializer, this.EventStore, new EventBus<T>(this));
        }
        
        // todo: register a Func<Type, ISnapshotRepository> to create the snapshot repository
        public abstract ISnapshotRepository GetSnapshotRepository(Type aggregateType);

        public void RegisterDenormalizer(Type aggregateType, Type denormalizer)
        {
            // add to dictionary
        }
        
        public IList<IDenormalizer> GetDenormalizers(Type aggregateType)
        {
            return new List<IDenormalizer>();
            // todo: need an actual repository here to store data in
            //return new List<IDenormalizer>() { new TestDenormalizer(null) };
        }
    }
}

