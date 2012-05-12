namespace MonoKit.Domain.Data
{
    using System;
    using System.Collections.Generic;
    using MonoKit.Data;

    public abstract class DictionaryRepository<T> : IRepository<T>
    {
        private readonly Dictionary<object, T> storage;

        protected DictionaryRepository()
        {
            this.storage = new Dictionary<object, T>();
        }

        protected Dictionary<object, T> Storage
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

        public T GetById(object id)
        {
            if (this.storage.ContainsKey(id))
            {
                return this.Storage[id];
            }

            return default(T);
        }

        public IEnumerable<T> GetAll()
        {
            return this.Storage.Values;
        }

        public void Save(T instance)
        {
            this.InternalSave(instance);
        }

        public void Delete(T instance)
        {
            this.InternalDelete(instance);
        }

        public void DeleteId(object id)
        {
            if (this.Storage.ContainsKey(id))
            {
                this.Storage.Remove(id);
            }
        }

        public void Dispose()
        {
            // todo
        }

        protected abstract T InternalNew();

        protected abstract void InternalSave(T instance);

        protected abstract void InternalDelete(T instance);
    }
}

