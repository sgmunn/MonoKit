namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;

    public interface IDenormalizer
    {
        void Handle(IList<IEvent> events);
    }
}

