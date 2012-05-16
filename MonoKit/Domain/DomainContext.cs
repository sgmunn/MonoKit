namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;
    using MonoKit.Domain.Data;
    using MonoKit.Data;
    
    public abstract class DomainContext : IDomainContext
    {
        private readonly Dictionary<Type, List<Func<IDomainContext, IDenormalizer>>> registeredDenormalizers;

        private readonly Dictionary<Type, Func<IDomainContext, ISnapshotRepository>> registeredSnapshotRepositories;

        public DomainContext(IEventStoreRepository eventStore, IDomainEventBus eventBus)
        {
            this.EventBus = eventBus;
            this.EventStore = eventStore;
            this.registeredDenormalizers = new Dictionary<Type, List<Func<IDomainContext, IDenormalizer>>>();
            this.registeredSnapshotRepositories = new Dictionary<Type, Func<IDomainContext, ISnapshotRepository>>();
        }

        public IEventStoreRepository EventStore{ get; private set; }

        public IDomainEventBus EventBus { get; private set; }

        public virtual ISerializer Serializer
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

        public virtual IAggregateRepository<T> AggregateRepository<T>() where T : IAggregateRoot, new()
        {
            // todo: should we decorate the aggregate root with an attribute to denote event sourced vs snapshot only ??
            //return new SnapshotAggregateRepository<T>(this.GetSnapshotRepository(typeof(T)), new EventBus<T>(this));
            
            return new AggregateRepository<T>(this.Serializer, this.EventStore, new EventBus<T>(this));
        }
        
        public virtual ISnapshotRepository GetSnapshotRepository(Type aggregateType)
        {
            if (this.registeredSnapshotRepositories.ContainsKey(aggregateType))
            {
                return this.registeredSnapshotRepositories [aggregateType](this);
            }

            return null;
        }

        public IList<IDenormalizer> GetDenormalizers(Type aggregateType)
        {
            var result = new List<IDenormalizer>();

            if (this.registeredDenormalizers.ContainsKey(aggregateType))
            {
                foreach (var factory in this.registeredDenormalizers[aggregateType])
                {
                    result.Add(factory(this));
                }
            }

            return result;
        }

        public void RegisterSnapshot<T>(Func<IDomainContext, ISnapshotRepository> createSnapshotRepository) where T : IAggregateRoot
        {
            this.registeredSnapshotRepositories[typeof(T)] = createSnapshotRepository;
        }

        public void RegisterDenormalizer<T>(Func<IDomainContext, IDenormalizer> createDenormalizer) where T : IAggregateRoot
        {
            if (!this.registeredDenormalizers.ContainsKey(typeof(T)))
            {
                this.registeredDenormalizers [typeof(T)] = new List<Func<IDomainContext, IDenormalizer>>();
            }

            this.registeredDenormalizers [typeof(T)].Add(createDenormalizer);
        }
    }
}

