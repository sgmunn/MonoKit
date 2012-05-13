namespace MonoKit.Domain.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    public class InMemoryEventStoreRepository<T> : DictionaryRepository<IEventStoreContract>, IEventStoreRepository where T : IEventStoreContract, new() 
    {
        protected override IEventStoreContract InternalNew()
        {
            return new T(); 
        }

        protected override void InternalSave(IEventStoreContract instance)
        {
            this.Storage[instance.EventId] = instance;
        }

        protected override void InternalDelete(IEventStoreContract instance)
        {
            if (this.Storage.ContainsKey(instance.EventId))
            {
                this.Storage.Remove(instance.EventId);
            }
        }

        public IEnumerable<IEventStoreContract> GetAllAggregateEvents(Guid rootId)
        {
            return this.GetAll().Where(x => x.AggregateId == rootId).OrderBy(x => x.Version);
        }
    }
}

