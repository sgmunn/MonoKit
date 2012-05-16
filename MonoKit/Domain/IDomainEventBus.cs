namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;

    public interface IDomainEventBus 
    {
        void Publish(IList<IEvent> events); 
    }
}

