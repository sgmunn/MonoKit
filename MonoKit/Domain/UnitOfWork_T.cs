// --------------------------------------------------------------------------------------------------------------------
// <copyright file=".cs" company="sgmunn">
//   (c) sgmunn 2012  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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