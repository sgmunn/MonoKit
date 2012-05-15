namespace MonoKit.Domain.Data.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using MonoKit.Data;
    using MonoKit.Data.SQLite;
    
    public class EventStoreRepository<T> : IEventStoreRepository where T : IEventStoreContract, new() 
    {
        private readonly IEventStoreRepository repository;
        
        public EventStoreRepository(SQLiteConnection connection)
        {
            this.repository = new InternalEventStoreRepository<T>(connection);
        }

        public IEventStoreContract New()
        {
            return Task.Factory.StartNew<IEventStoreContract>(
                () => { return this.repository.New(); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                SyncScheduler.TaskScheduler).Result;
        }

        public IEventStoreContract GetById(object id)
        {
            return Task.Factory.StartNew<IEventStoreContract>(
                () => { return this.repository.GetById(id); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                SyncScheduler.TaskScheduler).Result;
        }

        public IEnumerable<IEventStoreContract> GetAll()
        {
            return Task.Factory.StartNew<IEnumerable<IEventStoreContract>>(
                () => { return this.repository.GetAll(); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                SyncScheduler.TaskScheduler).Result;
        }

        public void Save(IEventStoreContract instance)
        {
            Task.Factory.StartNew(
                () => { this.repository.Save(instance); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                SyncScheduler.TaskScheduler).Wait();
        }

        public void Delete(IEventStoreContract instance)
        {
            Task.Factory.StartNew(
                () => { this.repository.Delete(instance); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                SyncScheduler.TaskScheduler).Wait();
        }

        public void DeleteId(object id)
        {
            Task.Factory.StartNew(
                () => { this.repository.DeleteId(id); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                SyncScheduler.TaskScheduler).Wait();
        }

        public IEnumerable<IEventStoreContract> GetAllAggregateEvents(Guid rootId)
        {
            return Task.Factory.StartNew<IEnumerable<IEventStoreContract>>(
                () => { return this.repository.GetAllAggregateEvents(rootId); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                SyncScheduler.TaskScheduler).Result;
        }

        public void Dispose()
        {
            this.repository.Dispose();
        }
    }
}

