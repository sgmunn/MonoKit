namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;
    using MonoKit.Data;
    using MonoKit.Domain.Data;
    
    public interface IEventBus 
    {
        // these events could be for multiple roots and root types.
        void Publish(IList<IDomainEvent> events); // this needs to be a differnt type (has to include the aggregate type, as well as the event)
    }
    

    // this is of T so that we know what the aggregate type is for mapping events to aggregate types
    // -- remember that any command can be applied to any aggregate eg CreateCommand and CreatedEvent
    public interface IEventBus<T> where T : IAggregateRoot
    {
        // this architecture dictates that these events are for a single aggregate type - perhaps multiple actual roots,
        // but a single type.
        // It does matter what type the event was applied to so that we get the correct denormalizers registered
        void Publish(IList<IDomainEvent> domainEvents);
    }
    
    public class EventBus<T> : IEventBus<T> where T : IAggregateRoot
    {
        // todo: wire up some read model denormalizers of T that will build their read models
        // from the events
        public void Publish(IList<IDomainEvent> domainEvents)
        {
            // we need to get the denormalizers for this type
            var denormalizers = new List<IDenormalizer>();
            
            // this could be done async
            foreach (var denormalizer in denormalizers)
            {
                denormalizer.Handle(domainEvents);
            }
        }
    }
    
    public class UnitOfWorkEventBus<T> : IEventBus<T>, IUnitOfWork where T : IAggregateRoot
    {
        private readonly IEventBus<T> bus;
        
        private readonly List<IDomainEvent> events;
        
        public UnitOfWorkEventBus(IUnitOfWorkScope scope, IEventBus<T> bus)
        {
            scope.Add(this);
            this.bus = bus;
            this.events = new List<IDomainEvent>();
        }
        
        public void Publish(IList<IDomainEvent> domainEvents)
        {
            this.events.AddRange(domainEvents);
        }
        
        public void Dispose()
        {
            this.events.Clear();
        }
        
        public void Commit()
        {
            this.bus.Publish(this.events);
        }
    }
}

