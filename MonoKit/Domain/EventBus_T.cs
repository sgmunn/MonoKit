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
            
            // todo: this coud be done async, but because we should only have one thread to the db at any one time it's not really worth it.
            // just don't take too long in any one denormalizer and don't make assumptions on the order of denormalizers being executed.
            foreach (var denormalizer in denormalizers)
            {
                denormalizer.Handle(events);
            }

            if (this.context.EventBus != null)
            {
                this.context.EventBus.Publish(events);
            }
        }
    }
}

