// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregateRoot.cs" company="sgmunn">
//   (c) sgmunn 2012  
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using MonoKit.Domain.Commands;
using MonoKit.Domain.Events;

namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;
    using System.Reflection;

    public abstract class AggregateRoot : IAggregateRoot
    {
        private static MethodExecutor Executor = new MethodExecutor();

        private readonly List<IEvent> uncommittedEvents;

        public AggregateRoot()
        {
            this.uncommittedEvents = new List<IEvent>();
        }

        public Guid AggregateId { get; protected set; }

        public int Version { get; protected set; }

        public bool Created { get; private set; }

        public IEnumerable<IEvent> UncommittedEvents
        {
            get
            {
                return this.uncommittedEvents;
            }
        }
        
        public virtual void Execute(CreateCommand command)
        {
            if (this.Created)
            {
                throw new InvalidOperationException(string.Format("The Aggregate {0} has already been created.", this.GetType().Name));
            }

            this.AggregateId = command.AggregateId;
            this.NewEvent(new CreatedEvent());
        }
        
        public void Apply(CreatedEvent @event)
        {
            this.AggregateId = @event.AggregateId;
        }

        public void Commit()
        {
            this.uncommittedEvents.Clear();
        }

        protected void ApplyEvents(IList<IEvent> events)
        {
            foreach (var @event in events)
            {
                this.ApplyEvent(@event);
                this.Version = @event.Version;
            }
        }

        protected void NewEvent(EventBase @event)
        {
            this.Version++;

            @event.AggregateId = this.AggregateId;
            @event.Version = this.Version;
            @event.Timestamp = DateTime.UtcNow;

            this.ApplyEvent(@event);
            this.uncommittedEvents.Add(@event);
        }

        private void ApplyEvent(IEvent @event)
        {
            Console.WriteLine("{0} - Apply {1} event {2}",  @event.Version,  @event.GetType().Name,  @event.Timestamp);

            if (!Executor.ExecuteMethodForSingleParam(this, @event))
            {
                throw new MissingMethodException(string.Format("Aggregate {0} does not support a method that can be called with {1}", this, @event));
            }
        }
    }

    public abstract class AggregateRoot<TState> : AggregateRoot, ISnapshotSupport where TState : class, ISnapshot, new()
    {
        public AggregateRoot()
        {
            this.InternalState = new TState();
        }

        protected TState InternalState { get; private set; }
                
        public override void Execute(CreateCommand command)
        {
            base.Execute(command);
        }


        public void LoadFromSnapshot(ISnapshot snapshot)
        {
            this.InternalState = snapshot as TState;
            this.Version = snapshot.Version;
            this.AggregateId = snapshot.Id;
        }

        public ISnapshot GetSnapshot()
        {
            var snapshot = this.InternalState;
            snapshot.Id = this.AggregateId;
            snapshot.Version = this.Version;

            return snapshot;
        }
    }

    ////public class StoredDomainRepository: IDomainRepository
    ////{
    ////    private Func<IRepository<StoredDomainAggregate>> factory;
        
    ////    public StoredDomainRepository(Func<IRepository<StoredDomainAggregate>> factory)
    ////    {
    ////        this.factory = factory;
    ////    }
        
    ////    public void Load(IAggregateRoot aggregate)
    ////    {
    ////        var repository = this.factory();
    ////        var storedAggregate = repository.Get(aggregate.AggregateId);
            
    ////        var state = JsonConvert.DeserializeObject(storedAggregate.Aggregate, new JsonSerializerSettings
    ////                {
    ////                    TypeNameHandling = TypeNameHandling.All
    ////                });
           
    ////        ((ILoadFromState)aggregate).LoadFromState(state, storedAggregate.Version);
    ////    }
        
    ////    // todo: move to uow
    ////    public void Save(IAggregateRoot aggregate, int expectedVersion)
    ////    {
    ////        // todo: we need to lock here I think,
    ////        // get current version of the root (from db), concurrency version check
    ////        // then we need a unit of work in this repository to ...
            
            
    ////        // todo: do the save, check for version
    ////        var repository = this.factory();
   
    ////        var state = ((ILoadFromState)aggregate).GetInternalState();

    ////        var storedAggregate = new StoredDomainAggregate
    ////        {
    ////            AggregateId = aggregate.AggregateId,
    ////            Version = aggregate.Version,
    ////            Aggregate = JsonConvert.SerializeObject(state, Formatting.Indented, new JsonSerializerSettings
    ////                {
    ////                    TypeNameHandling = TypeNameHandling.All
    ////                }),
    ////        };
            
    ////        repository.Save(storedAggregate);    
    ////    }
    ////}
    
    //public class EventSourcedDomainRepository : IDomainRepository
    //{
    //    private Func<Guid, IRepository<IDomainEvent>> factory;
        
    //    public EventSourcedDomainRepository(Func<Guid, IRepository<IDomainEvent>> factory)
    //    {
    //        this.factory = factory;
    //    }
        
    //    public void Load(IAggregateRoot aggregate)
    //    {
    //        var repository = this.factory(aggregate.AggregateId);
    //        ((IEventSourced)aggregate).LoadFromHistory(repository.GetAll().ToList());
    //    }
        
    //    // todo: move to uow
    //    public void Save(IAggregateRoot aggregate, int expectedVersion)
    //    {
    //        var repository = this.factory(aggregate.AggregateId);
    //        //repository.Save();
    //    }
    //}

    
    
    
    //public class DomainRepositoryFactory : IDomainRepositoryFactory
    //{
    //    public IDomainRepository GetRepository(Guid id)
    //    {
    //        // 
    //        throw new NotImplementedException();
    //        //return new DomainRepository<T>((r) => new SqliteEventStore(r, null));
    //    }
    //}
        
    //public class StoredDomainAggregate
    //{
    //    //[MonoKit.Data.SQLite.PrimaryKey]
    //    public Guid AggregateId { get; set; }

    //    public int Version { get; set; }
        
    //    public string Aggregate { get; set; }
    //}

    
    //public abstract class EventStore : IRepository<IDomainEvent>
    //{
    //    public EventStore(Guid aggregateId)
    //    {
    //        this.AggregateId = aggregateId;
    //    }

    //    public Guid AggregateId { get; private set; }
        
    //    public IDomainEvent Get(Guid id)
    //    {
    //        // returns a single event, probably not that useful
    //        throw new NotSupportedException();
    //    }
        
    //    public abstract IEnumerable<IDomainEvent> GetAll();
        
    //    public void Save(IDomainEvent domainEvent)
    //    {
    //        // saves a new event for the aggregate
    //        if (this.AggregateId != domainEvent.AggregateId)
    //        {
    //            throw new InvalidOperationException("Cannot save an event for a different aggregate");
    //        }
            
    //        this.InternalSave(domainEvent);
    //    }
        
    //    protected abstract void InternalSave(IDomainEvent domainEvent);
    //}
    
    //public class StoredDomainEvent
    //{
    //    //[MonoKit.Data.SQLite.PrimaryKey]
    //    public Guid EventId { get; set; }
        
    //    //[MonoKit.Data.SQLite.Indexed]
    //    public Guid AggregateId { get; set; }

    //    public int Version { get; set; }
        
    //    public string Event { get; set; }
    //}
    
    //public class SqliteEventStore : EventStore
    //{
    //    private static bool NeedsInitialization = true;
        
    //    private MonoKit.Data.SQLite.SQLiteConnection connection;
        
    //    public SqliteEventStore(Guid aggregateId, MonoKit.Data.SQLite.SQLiteConnection connection) : base(aggregateId)
    //    {
    //        this.connection = connection;
    //    }
        
    //    public override IEnumerable<IDomainEvent> GetAll()
    //    {
    //        var storedEvents = new List<StoredDomainEvent>();
            
    //        var repo = new MonoKit.Data.SQLiteRepository<StoredDomainEvent>(this.connection, NeedsInitialization);
    //        using (repo)
    //        {
    //            NeedsInitialization = false;

    //            // get all events for the aggregate sorted by version
    //            storedEvents.AddRange(repo.GetAll().Where(x => x.AggregateId == this.AggregateId).OrderBy(x => x.Version).ToList());
    //        }
            
    //        var events = from ev in storedEvents
    //            select (JsonConvert.DeserializeObject(ev.Event, new JsonSerializerSettings
    //                {
    //                    TypeNameHandling = TypeNameHandling.All
    //                }) as IDomainEvent);
            
    //        return events;
    //    }
        
    //    protected override void InternalSave(IDomainEvent domainEvent)
    //    {
    //        var repo = new MonoKit.Data.SQLiteRepository<StoredDomainEvent>(this.connection, NeedsInitialization);
    //        using (repo)
    //        {
    //            NeedsInitialization = false;
                
    //            var storedEvent = new StoredDomainEvent
    //            {
    //                AggregateId = domainEvent.AggregateId,
    //                EventId = domainEvent.EventId,
    //                Version = domainEvent.Version,
    //                Event = JsonConvert.SerializeObject(domainEvent, Formatting.Indented, new JsonSerializerSettings
    //                {
    //                    TypeNameHandling = TypeNameHandling.All
    //                }),
    //            };
                
    //            repo.Insert(storedEvent);
    //        }
    //    }
    //}
}

