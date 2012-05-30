// --------------------------------------------------------------------------------------------------------------------
// <copyright file=".cs" company="sgmunn">
//   (c) sgmunn 2012  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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

        private readonly IEventSerializer serializer;
        
        private readonly IEventBus<T> eventBus;
  
        public AggregateRepository(IEventSerializer serializer, IEventStoreRepository repository, IEventBus<T> eventBus)
        {
            this.serializer = serializer;
            this.repository = repository;
            this.eventBus = eventBus;
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
            // todo:
        }

        public void DeleteId(object id)
        {
            // todo:
        }

        public void Dispose()
        {
            // todo:
        }
    }
}

