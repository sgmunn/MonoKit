//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="InMemorySnapshotRepository_T.cs" company="sgmunn">
//    (c) sgmunn 2012  
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//    documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//    to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//    the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//    THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//    CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//    IN THE SOFTWARE.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Domain
{
    using System;
    using MonoKit.Data;
    using System.Collections.Generic;
    using System.Linq;
    
    public class InMemorySnapshotRepository<T> : DictionaryRepository<T>, ISnapshotRepository
        where T : class, ISnapshot, new() 
    {
        public InMemorySnapshotRepository()
        {
        }

        protected override T InternalNew()
        {
            return new T(); 
        }

        protected override SaveResult InternalSave(T snapshot)
        {
            if (this.Storage.ContainsKey(snapshot.Identity))
            {
                this.Storage[snapshot.Identity] = snapshot;
                return SaveResult.Updated;
            }
            
            this.Storage[snapshot.Identity] = snapshot;
            return SaveResult.Added;
        }

        protected override void InternalDelete(T snapshot)
        {
            if (this.Storage.ContainsKey(snapshot.Identity))
            {
                this.Storage.Remove(snapshot.Identity);
            }
        }

        public new ISnapshot New()
        {
            return this.InternalNew();
        }

        public new ISnapshot GetById(Guid id)
        {
            return base.GetById(id);
        }

        public new IList<ISnapshot> GetAll()
        {
            return base.GetAll().Cast<ISnapshot>().ToList();
        }

        public SaveResult Save(ISnapshot snapshot)
        {
            return this.InternalSave((T)snapshot);
        }

        public void Delete(ISnapshot snapshot)
        {
            this.InternalDelete((T)snapshot);
        }
    }
}

