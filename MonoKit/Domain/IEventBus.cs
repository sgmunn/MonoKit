namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;
    using MonoKit.Data;
    using MonoKit.Domain.Data;
    
    public interface IDomainEvent
    {
        Type AggregateType { get; }
        IEvent Event { get; }
    }
    
    public class DomainEvent : IDomainEvent
    {
        public Type AggregateType { get; set; }
        public IEvent Event { get; set; }
    }
    
    public interface IDomainEventBus 
    {
        void Publish(IList<IDomainEvent> events); 
    }
    

    // this is of T so that we know what the aggregate type is for mapping events to aggregate types
    // -- remember that any command can be applied to any aggregate eg CreateCommand and CreatedEvent
    public interface IEventBus<T> where T : IAggregateRoot
    {
        // this architecture dictates that these events are for a single aggregate type - perhaps multiple actual roots,
        // but a single type.
        // It does matter what type the event was applied to so that we get the correct denormalizers registered
        void Publish(IList<IEvent> events);
    }
    
    public class EventBus<T> : IEventBus<T> where T : IAggregateRoot
    {
        private readonly IDomainEventBus domainBus;
        
        public EventBus(IDomainEventBus domainBus)
        {
            this.domainBus = domainBus;
        }
        
        // todo: wire up some read model denormalizers of T that will build their read models
        // from the events
        public void Publish(IList<IEvent> events)
        {
            // we need to get the denormalizers for this type
            var denormalizers = new List<IDenormalizer>();
            
            // this could be done async
            foreach (var denormalizer in denormalizers)
            {
                denormalizer.Handle(events);
            }
            
            if (this.domainBus != null)
            {
                var domainEvents = new List<IDomainEvent>(); 
                
                foreach (var @event in events)
                {
                    var domainEvent = new DomainEvent 
                    {
                        AggregateType = typeof(T),
                        Event = @event,
                    };
                }
                
                this.domainBus.Publish(domainEvents);
            }
        }
    }
    
    // using this means that publication happens during the commit of the scope, which probably isn't quite right/
    // commits shouldn't really do alot - denormalizer logic should have been done prior
    // we also can't fire commands off from a denormalizer during the scope.
    
//    public class UnitOfWorkEventBus<T> : IEventBus<T>, IUnitOfWork where T : IAggregateRoot
//    {
//        private readonly IEventBus<T> bus;
//        
//        private readonly List<IEvent> allEvents;
//        
//        public UnitOfWorkEventBus(IUnitOfWorkScope scope, IEventBus<T> bus)
//        {
//            scope.Add(this);
//            this.bus = bus;
//            this.allEvents = new List<IEvent>();
//        }
//        
//        public void Publish(IList<IEvent> events)
//        {
//            this.allEvents.AddRange(events);
//        }
//        
//        public void Dispose()
//        {
//            this.allEvents.Clear();
//        }
//        
//        public void Commit()
//        {
//            this.bus.Publish(this.allEvents);
//        }
//    }
}

