namespace MonoKit.Data
{
    using System;
    using System.Collections.Generic;
    using MonoKit.Data.SQLite;
    
    public interface IRepository<T> : IDisposable where T: new() 
    {
        T Get(object key);
        TableQuery<T> GetAll();
        void Insert(T obj);
        void InsertAll(IEnumerable<T> objects);
        void Delete(T obj);
        void Update(T obj);
    }
}

