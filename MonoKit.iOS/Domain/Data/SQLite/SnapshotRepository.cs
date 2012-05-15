namespace MonoKit.Domain.Data.SQLite
{
    using System;
    using MonoKit.Domain;
    using System.Collections.Generic;
    using System.Linq;
    using MonoKit.Domain.Data;
    using MonoKit.Data;
    using MonoKit.Data.SQLite;
    
    public class SnapshotRepository<T> : ISnapshotRepository where T : class, ISnapshot, new() 
    {
        private readonly SyncRepository<T> repository;
        
        public SnapshotRepository(SQLiteConnection connection)
        {
            var repo = new SQLiteRepository<T>(connection);
            this.repository = new SyncRepository<T>(repo, SyncScheduler.TaskScheduler);
        }

        public ISnapshot New()
        {
            return new T();
        }

        public ISnapshot GetById(object id)
        {
            return ((T)this.repository.GetById(id));
        }

        public IEnumerable<ISnapshot> GetAll()
        {
            return this.repository.GetAll().Cast<ISnapshot>();
        }

        public void Save(ISnapshot instance)
        {
            this.repository.Save((T)instance);
        }

        public void Delete(ISnapshot instance)
        {
            this.repository.Delete((T)instance);
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

