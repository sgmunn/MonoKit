namespace MonoKit.Domain.Data
{
    using System;
    using System.Collections.Generic;
    using MonoKit.Data;

    public interface IEventStoreRepository : IRepository<IDomainEventContract>
    {
        IEnumerable<IDomainEventContract> GetAllAggregateEvents(Guid rootId);
    }
}

