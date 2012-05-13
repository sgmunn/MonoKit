namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;

    public interface IEventSourced
    {
        void LoadFromEvents(IList<IEvent> events);
    }
}

