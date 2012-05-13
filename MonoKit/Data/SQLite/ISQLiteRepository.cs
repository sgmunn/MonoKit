namespace MonoKit.Data
{
    using System;
    using System.Collections.Generic;
    using MonoKit.Data.SQLite;
    
    // todo: rename this interface from IRepository to ISQLiteRepository, IRepository should be more generic
    public interface ISQLiteRepository<T> : IDisposable where T: new() 
    {
        T Get(object key);
        TableQuery<T> GetAll();
        void Insert(T obj);
        void InsertAll(IEnumerable<T> objects);
        void Delete(T obj);
        void Update(T obj);
    }
}

