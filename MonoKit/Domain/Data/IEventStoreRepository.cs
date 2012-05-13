namespace MonoKit.Domain.Data
{
    using System;
    using System.Collections.Generic;
    using MonoKit.Data;

    public interface IEventStoreRepository : IRepository<IEventStoreContract>
    {
        IEnumerable<IEventStoreContract> GetAllAggregateEvents(Guid rootId);
    }
}

