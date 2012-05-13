namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;

    public interface IEventBus<T> where T : IAggregateRoot
    {
        void Publish(IList<IEvent> events);
    }
}

