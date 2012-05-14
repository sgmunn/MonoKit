namespace MonoKit.Data.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MonoKit.Domain.Data;
    
    public class InternalEventStoreRepository<T> : IEventStoreRepository where T : IEventStoreContract, new() 
    {
        private readonly SQLiteRepository<T> repository;
        
        public InternalEventStoreRepository(SQLiteConnection connection)
        {
            this.repository = new SQLiteRepository<T>(connection);
            
            // todo: refactor out table creation to somewhere else so that it's not done everytime we create a repository, maybe even make the domain context do it
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
            return this.repository.GetAll().Cast<IEventStoreContract>().Where(x => x.AggregateId == rootId).OrderBy(x => x.Version);
        }

        public void Dispose()
        {
            this.repository.Dispose();
        }
    }
}

