namespace MonoKit.Data.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    public class SQLiteRepository<T> : IRepository<T> where T: new()
    {
        private readonly SQLiteConnection connection;
        
        public SQLiteRepository(SQLiteConnection connection)
        {
            this.connection = connection;
        }
        
        public void Dispose()
        {
        }

        public T New()
        {
            return new T();
        }

        public T GetById(object id)
        {
            return this.connection.Get<T>(id);
        }

        public IEnumerable<T> GetAll()
        {
            return this.connection.Table<T>().AsEnumerable();
        }

        public void Save(T instance)
        {
            if (this.connection.Update(instance) == 0)
            {
                this.connection.Insert(instance);
            }
        }

        public void Delete(T instance)
        {
            this.connection.Delete(instance);
        }

        public void DeleteId(object id)
        {
            throw new NotImplementedException();
            // have to get table name
        }
    }    
}
