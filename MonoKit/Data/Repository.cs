namespace MonoKit.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using MonoKit.Data.SQLite;
    
    public class SQLiteRepository<T> : IRepository<T> where T: new()
    {
        private bool isDisposed;
        
        private SQLiteConnection connection;
        
        public SQLiteRepository(SQLiteConnection connection)
        {
            this.connection = connection;
            Monitor.Enter(this.connection);
            
            this.connection.CreateTable<T>();
        }
        
        public SQLiteRepository(SQLiteConnection connection, bool createTable)
        {
            this.connection = connection;
            Monitor.Enter(this.connection);
           
            if (createTable)
            {
                this.connection.CreateTable<T>();
            }
        }
        
        ~SQLiteRepository()
        {
            this.Dispose(false);
        }
        
        public void Dispose()
        {
            this.Dispose(true);
        }
        
        public T Get(object key)
        {
            this.CheckDisposed();
            return this.connection.Get<T>(key);
        }
        
        public TableQuery<T> GetAll()
        {
            this.CheckDisposed();
            return this.connection.Table<T>();
        }
        
        public void Insert(T obj)
        {
            this.CheckDisposed();
            this.connection.Insert(obj);
        }
        
        public void InsertAll(IEnumerable<T> objects)
        {
            this.CheckDisposed();
            this.connection.InsertAll(objects);
        }
        
        public void Delete(T obj)
        {
            this.CheckDisposed();
            this.connection.Delete(obj);
        }
        
        public void Update(T obj)
        {
            this.CheckDisposed();
            this.connection.Update(obj);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                Monitor.Exit(this.connection);
                this.isDisposed = true;
            }
        }
        
        private void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException("SQLiteRepository");
            }
        }
    }
}
