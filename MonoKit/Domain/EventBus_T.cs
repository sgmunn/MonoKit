namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;
    
    public class EventBus<T> : IEventBus<T> where T : IAggregateRoot
    {
        private readonly IDomainContext context;
        
        public EventBus(IDomainContext context)
        {
            this.context = context;
        }
        
        public void Publish(IList<IEvent> events)
        {
            var denormalizers = this.context.GetDenormalizers(typeof(T));
            
            // todo: this can be done async
            foreach (var denormalizer in denormalizers)
            {
                denormalizer.Handle(events);
            }
            
            if (this.context.EventBus != null)
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
                
                this.context.EventBus.Publish(domainEvents);
            }
        }
    }
}

