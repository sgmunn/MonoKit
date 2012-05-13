namespace MonoKit.Data.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MonoKit.Domain.Data;
    
    public class SQLiteEventStoreRepository<T> : IEventStoreRepository where T : IEventStoreContract, new() 
    {
        private readonly SyncSQLiteRepository<T> repository;
        
        public SQLiteEventStoreRepository(SQLiteConnection connection)
        {
            this.repository = new SyncSQLiteRepository<T>(connection);
            
            // todo: we should probably ensure that this is sync'd 
            connection.CreateTable<T>();
        }

        public IEventStoreContract New()
        {
            return new T();
        }

        public IEventStoreContract GetById(object id)
        {
            return ((T)this.repository.GetById(id));
        }

        public IEnumerable<IEventStoreContract> GetAll()
        {
            return this.repository.GetAll().Cast<IEventStoreContract>();
        }

        public void Save(IEventStoreContract instance)
        {
            this.repository.Save((T)instance);
        }

        public void Delete(IEventStoreContract instance)
        {
            this.repository.Delete((T)instance);
        }

        public void DeleteId(object id)
        {
            this.repository.DeleteId(id);
        }

        public IEnumerable<IEventStoreContract> GetAllAggregateEvents(Guid rootId)
        {
            // todo: use the connection to pass the query to the db and not in memory
            return this.repository.GetAll().Cast<IEventStoreContract>().Where(x => x.AggregateId == rootId).OrderBy(x => x.Version);
        }

        public void Dispose()
        {
            this.repository.Dispose();
        }
    }
}

