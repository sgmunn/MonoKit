namespace MonoKit.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MonoKit.Tasks;
    
    public class SyncRepository<T> : IRepository<T> where T: new()
    {
        private readonly IRepository<T> repository;
        
        private readonly TaskScheduler scheduler;
        
        public SyncRepository(IRepository<T> repository, TaskScheduler scheduler)
        {
            this.repository = repository;
            this.scheduler = scheduler;
        }
        
        public void Dispose()
        {
            this.repository.Dispose();
        }

        public T New()
        {
            return Task.Factory.StartNew<T>(
                () => { return this.repository.New(); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                scheduler).Result;
        }

        public T GetById(object id)
        {
            return Task.Factory.StartNew<T>(
                () => { return this.repository.GetById(id); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                scheduler).Result;
        }

        public IEnumerable<T> GetAll()
        {
            return Task.Factory.StartNew<IEnumerable<T>>(
                () => { return this.repository.GetAll(); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                scheduler).Result;
        }

        public void Save(T instance)
        {
            Task.Factory.StartNew(
                () => { this.repository.Save(instance); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                scheduler).Wait();
        }

        public void Delete(T instance)
        {
            Task.Factory.StartNew(
                () => { this.repository.Delete(instance); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                scheduler).Wait();
        }

        public void DeleteId(object id)
        {
            Task.Factory.StartNew(
                () => { this.repository.DeleteId(id); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                scheduler).Wait();
        }
    }
}

