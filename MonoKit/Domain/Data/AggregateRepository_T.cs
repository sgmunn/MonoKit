namespace MonoKit.Domain.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MonoKit.Data;
    
    // todo: pass in a repository of state and handle snaspshots
    public class AggregateRepository<T> : IAggregateRepository<T> where T : IAggregateRoot, new()
    {
        private readonly IEventStoreRepository repository;

        private readonly ISerializer serializer;
        
        private readonly IEventBus<T> eventBus;

        public AggregateRepository(ISerializer serializer, IEventStoreRepository repository, IEventBus<T> eventBus)
        {
            this.serializer = serializer;
            this.repository = repository;
        }

        public T New()
        {
            return new T();
        }

        public T GetById(object id)
        {
            var allEvents = this.repository.GetAllAggregateEvents((Guid)id).ToList();

            if (allEvents.Count == 0)
            {
                return default(T);
            }

            var history = new List<IEvent>();

            foreach (var storedEvent in allEvents)
            {
                history.Add(this.serializer.DeserializeFromString(storedEvent.Event) as IEvent);
            }

            var result = this.New();

            ((IEventSourced)result).LoadFromEvents(history);

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

            int expectedVersion = instance.UncommittedEvents.First().Version - 1;

            var allEvents = this.repository.GetAllAggregateEvents(instance.AggregateId).ToList();

            var lastEvent = allEvents.LastOrDefault();
            if ((lastEvent == null && expectedVersion != 0) || (lastEvent != null && lastEvent.Version != expectedVersion))
            {
                throw new ConcurrencyException();
            }

            foreach (var @event in instance.UncommittedEvents.ToList())
            {
                var storedEvent = this.repository.New();
                storedEvent.AggregateId = instance.AggregateId;
                storedEvent.EventId = @event.EventId;
                storedEvent.Version = @event.Version;
                storedEvent.Event = this.serializer.SerializeToString(@event);

                this.repository.Save(storedEvent);
            }
   
            if (this.eventBus != null)
            {
                this.eventBus.Publish(instance.UncommittedEvents.ToList());
            }
            
            instance.Commit();
        }

        public void Delete(T instance)
        {
        }

        public void DeleteId(object id)
        {
        }

        public void Dispose()
        {
        }
    }
}

