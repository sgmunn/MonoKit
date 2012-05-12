namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MonoKit.Data;
    
    public class UnitOfWork<T> : IUnitOfWork<T> where T : IAggregateRoot
    {
        private readonly IRepository<T> repository;

        private readonly Dictionary<object, T> savedItems;
        
        private readonly List<object> deletedItemKeys;

        public UnitOfWork(IUnitOfWorkScope scope, IRepository<T> repository)
        {
            scope.Add(this);
            this.repository = repository;

            this.savedItems = new Dictionary<object, T>();
            this.deletedItemKeys = new List<object>();
        }

        protected IRepository<T> Repository
        {
            get
            {
                return this.repository;
            }
        }

        public void Dispose()
        {
            this.Repository.Dispose();
            this.savedItems.Clear();
            this.deletedItemKeys.Clear();
        }

        public T New()
        {
            return this.repository.New();
        }

        public T GetById(object id)
        {
            if (this.deletedItemKeys.Contains(id))
            {
                return default(T);
            }

            if (this.savedItems.ContainsKey(id))
            {
                return this.savedItems[id];
            }

            return this.repository.GetById(id);
        }

        public IEnumerable<T> GetAll()
        {
            var saved = this.savedItems.Values;

            return saved.Union(this.Repository.GetAll()).Where(x => !this.deletedItemKeys.Contains(x.AggregateId));
        }

        public void Save(T instance)
        {
            this.savedItems[instance.AggregateId] = instance;
        }

        public void Delete(T instance)
        {
            var id = instance.AggregateId;
            this.DeleteId(id);
        }

        public void DeleteId(object id)
        {
            if (!this.deletedItemKeys.Contains(id))
            {
                this.deletedItemKeys.Add(id);
            }
        }

        public void Commit()
        {
            foreach (var item in this.savedItems.Values)
            {
                this.Repository.Save(item);
            }

            foreach (var id in this.deletedItemKeys)
            {
                this.Repository.DeleteId(id);
            }
        }
    }



//    // in this sqlite implementation of this, lock the connection so that we don't get threading issues 
//    public interface IConnection
//    {
//        void BeginTransaction();
//
//        void Commit();
//
//        void Rollback();
//    }
//
//    public class ConnectionUnitOfWork<T> : IUnitOfWork<T>
//    {
//        private IConnection connection;
//
//        private IRepository<T> repository;
//
//        public ConnectionUnitOfWork(IConnection connection, IRepository<T> repository)
//        {
//            this.connection = connection;
//            this.repository = repository;
//
//            this.connection.BeginTransaction();
//        }
//
//        public void Dispose()
//        {
//            this.connection.Rollback();
//        }
//
//        public void Commit()
//        {
//            this.connection.Commit();
//        }
//
//        public T New()
//        {
//            return this.repository.New();
//        }
//
//        public T GetById(object id)
//        {
//            return this.repository.GetById(id);
//        }
//
//        public IEnumerable<T> GetAll()
//        {
//            return this.GetAll();
//        }
//
//        public void Save(T instance)
//        {
//            this.repository.Save(instance);
//        }
//
//        public void Delete(T instance)
//        {
//            this.repository.Delete(instance);
//        }
//
//        public void DeleteId(object id)
//        {
//            this.repository.DeleteId(id);
//        }
//    }
}