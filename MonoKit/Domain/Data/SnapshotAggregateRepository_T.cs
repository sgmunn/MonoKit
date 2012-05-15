namespace MonoKit.Domain.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    public class SnapshotAggregateRepository<T> : IAggregateRepository<T> where T : IAggregateRoot, new()
    {
        private readonly ISnapshotRepository repository;
        
        private readonly IEventBus<T> eventBus;
  
        public SnapshotAggregateRepository(ISnapshotRepository repository, IEventBus<T> eventBus)
        {
            this.repository = repository;
            this.eventBus = eventBus;
        }

        public T New()
        {
            return new T();
        }

        public T GetById(object id)
        {
            var snapshot = this.repository.GetById(id);
            
            if (snapshot == null)
            {
                return default(T);
            }
            
            var result = this.New();
   
            ((ISnapshotSupport)result).LoadFromSnapshot(snapshot);

            return result;
        }

        public IEnumerable<T> GetAll()
        {
            throw new NotSupportedException();
        }

        public void Save(T instance)
        {
            if (!instance.UncommittedEvents.Any())
            {
                return;
            }
            
            var current = this.GetById(instance.AggregateId);

            int expectedVersion = instance.UncommittedEvents.First().Version - 1;

            if ((current == null && expectedVersion != 0) || (current != null && current.Version != expectedVersion))
            {
                throw new ConcurrencyException();
            }
   
            var snapshot = ((ISnapshotSupport)instance).GetSnapshot() as ISnapshot;
            this.repository.Save(snapshot);
            
            if (this.eventBus != null)
            {
                this.eventBus.Publish(instance.UncommittedEvents.ToList());
            }
            
            instance.Commit();
        }

        public void Delete(T instance)
        {
            this.repository.DeleteId(instance.AggregateId);
        }

        public void DeleteId(object id)
        {
            this.repository.DeleteId(id);
        }

        public void Dispose()
        {
            this.repository.Dispose();
        }
    }
}

