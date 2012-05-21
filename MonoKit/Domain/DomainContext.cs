using System.Linq;

namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;
    using MonoKit.Domain.Data;
    using MonoKit.Data;
    
    public abstract class DomainContext : IDomainContext
    {
        private readonly Dictionary<Type, List<Func<IDomainContext, IReadModelBuilder>>> registeredBuilders;

        private readonly Dictionary<Type, Func<IDomainContext, ISnapshotRepository>> registeredSnapshotRepositories;

        public DomainContext(IEventStoreRepository eventStore, IDomainEventBus eventBus)
        {
            this.EventBus = eventBus;
            this.EventStore = eventStore;
            this.EventSerializer = new DefaultEventSerializer();

            this.registeredBuilders = new Dictionary<Type, List<Func<IDomainContext, IReadModelBuilder>>>();
            this.registeredSnapshotRepositories = new Dictionary<Type, Func<IDomainContext, ISnapshotRepository>>();
        }

        public IEventStoreRepository EventStore{ get; private set; }

        public IDomainEventBus EventBus { get; private set; }

        public IEventSerializer EventSerializer { get; protected set; }

        public virtual IUnitOfWorkScope BeginUnitOfWork()
        {
            return new DefaultScope();
        }

        public virtual IAggregateRepository<T> AggregateRepository<T>() where T : IAggregateRoot, new()
        {
            if (typeof(T).GetInterfaces().Contains(typeof(IEventSourced)))
            {
                return new AggregateRepository<T>(this.EventSerializer, this.EventStore, new EventBus<T>(this));
            }

            return new SnapshotAggregateRepository<T>(this.GetSnapshotRepository(typeof(T)), new EventBus<T>(this));
        }
        
        public virtual ISnapshotRepository GetSnapshotRepository(Type aggregateType)
        {
            if (this.registeredSnapshotRepositories.ContainsKey(aggregateType))
            {
                return this.registeredSnapshotRepositories [aggregateType](this);
            }

            return null;
        }

        public IList<IReadModelBuilder> GetReadModelBuilders(Type aggregateType)
        {
            var result = new List<IReadModelBuilder>();

            if (this.registeredBuilders.ContainsKey(aggregateType))
            {
                foreach (var factory in this.registeredBuilders[aggregateType])
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

        public void RegisterBuilder<T>(Func<IDomainContext, IReadModelBuilder> createBuilder) where T : IAggregateRoot
        {
            if (!this.registeredBuilders.ContainsKey(typeof(T)))
            {
                this.registeredBuilders [typeof(T)] = new List<Func<IDomainContext, IReadModelBuilder>>();
            }

            this.registeredBuilders [typeof(T)].Add(createBuilder);
        }
    }
}

