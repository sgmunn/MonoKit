// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryRepository_T.cs" company="sgmunn">
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

    public abstract class DictionaryRepository<T> : IRepository<T>
    {
        private readonly Dictionary<Guid, T> storage;

        protected DictionaryRepository()
        {
            this.storage = new Dictionary<Guid, T>();
        }

        protected Dictionary<Guid, T> Storage
        {
            get
            {
                return this.storage;
            }
        }

        public T New()
        {
            return this.InternalNew();
        }

        public T GetById(Guid id)
        {
            if (this.storage.ContainsKey(id))
            {
                return this.Storage[id];
            }

            return default(T);
        }

        public IList<T> GetAll()
        {
            return this.Storage.Values.ToList();
        }

        public SaveResult Save(T instance)
        {
            return this.InternalSave(instance);
        }

        public void Delete(T instance)
        {
            this.InternalDelete(instance);
        }

        public void DeleteId(Guid id)
        {
            if (this.Storage.ContainsKey(id))
            {
                this.Storage.Remove(id);
            }
        }

        public void Dispose()
        {
        }

        protected abstract T InternalNew();

        protected abstract SaveResult InternalSave(T instance);

        protected abstract void InternalDelete(T instance);
    }
}

