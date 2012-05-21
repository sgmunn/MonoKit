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
            var builders = this.context.GetReadModelBuilders(typeof(T));
            
            // todo: this coud be done async, but because we should only have one thread to the db at any one time it's not really worth it.
            // just don't take too long in any one builder and don't make assumptions on the order of builders being executed.
            foreach (var builder in builders)
            {
                builder.Handle(events);
            }

            if (this.context.EventBus != null)
            {
                this.context.EventBus.Publish(events);
            }
        }
    }
}

