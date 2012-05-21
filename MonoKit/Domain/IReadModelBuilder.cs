namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;

    public interface IReadModelBuilder
    {
        void Handle(IList<IEvent> events);
    }
}

