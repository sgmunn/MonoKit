namespace MonoKit.Data.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MonoKit.Tasks;

    public class SyncSQLiteRepository<T> : IRepository<T> where T: new()
    {
        private readonly SQLiteRepository<T> repository;
        
        private static SyncTaskScheduler TaskScheduler = new SyncTaskScheduler();
        
        public SyncSQLiteRepository(SQLiteConnection connection)
        {
            this.repository = new SQLiteRepository<T>(connection);
        }
        
        public void Dispose()
        {
            this.repository.Dispose();
        }

        public T New()
        {
            return this.repository.New();
        }

        public T GetById(object id)
        {
            return Task.Factory.StartNew<T>(
                () => { return this.repository.GetById(id); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                TaskScheduler).Result;
        }

        public IEnumerable<T> GetAll()
        {
            return Task.Factory.StartNew<IEnumerable<T>>(
                () => { return this.repository.GetAll(); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                TaskScheduler).Result;
        }

        public void Save(T instance)
        {
            Task.Factory.StartNew(
                () => { this.repository.Save(instance); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                TaskScheduler).Wait();
        }

        public void Delete(T instance)
        {
            Task.Factory.StartNew(
                () => { this.repository.Delete(instance); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                TaskScheduler).Wait();
        }

        public void DeleteId(object id)
        {
            throw new NotImplementedException();
            // have to get table name
        }
    }
}

