namespace MonoKit.Domain.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    public class InMemoryDomainEventRepository<T> : DictionaryRepository<IDomainEventContract>, IEventStoreRepository where T : IDomainEventContract, new() 
    {
        protected override IDomainEventContract InternalNew()
        {
            return new T(); 
        }

        protected override void InternalSave(IDomainEventContract instance)
        {
            this.Storage[instance.EventId] = instance;
        }

        protected override void InternalDelete(IDomainEventContract instance)
        {
            if (this.Storage.ContainsKey(instance.EventId))
            {
                this.Storage.Remove(instance.EventId);
            }
        }

        public IEnumerable<IDomainEventContract> GetAllAggregateEvents(Guid rootId)
        {
            return this.GetAll().Where(x => x.AggregateId == rootId).OrderBy(x => x.Version);
        }
    }
}

