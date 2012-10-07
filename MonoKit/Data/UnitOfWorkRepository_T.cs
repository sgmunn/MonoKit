// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitOfWorkRepository_T.cs" company="sgmunn">
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

namespace MonoKit.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MonoKit.Data;
    
    public class UnitOfWorkRepository<T> : IRepositoryUnitOfWork<T> where T : IId
    {
        private readonly IRepository<T> repository;

        private readonly Dictionary<Guid, T> savedItems;
        
        private readonly List<Guid> deletedItemKeys;

        public UnitOfWorkRepository(IRepository<T> repository)
        {
            this.repository = repository;

            this.savedItems = new Dictionary<Guid, T>();
            this.deletedItemKeys = new List<Guid>();
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

        public T GetById(Guid id)
        {
            if (this.deletedItemKeys.Any(x => x == id))
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

            return saved.Union(this.Repository.GetAll()).Where(x => !this.deletedItemKeys.Any(y => y == x.Identity));
        }

        public virtual SaveResult Save(T instance)
        {
            if (this.savedItems.ContainsKey(instance.Identity))
            {
                this.savedItems[instance.Identity] = instance;
                return SaveResult.Updated;
            }

            this.savedItems[instance.Identity] = instance;
            return SaveResult.Added;
        }

        public virtual void Delete(T instance)
        {
            var id = instance.Identity;
            this.DeleteId(id);
        }

        public virtual void DeleteId(Guid id)
        {
            if (!this.deletedItemKeys.Any(x => x == id))
            {
                this.deletedItemKeys.Add(id);
            }
        }

        public virtual void Commit()
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
}