// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlSnapshotRepository.cs" company="sgmunn">
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

namespace MonoKit.Domain.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MonoKit.Data;
    using MonoKit.Data.SQLite;

    public class SqlSnapshotRepository<T> : ISnapshotRepository 
        where T : class, ISnapshot, new() 
    {
        private readonly SyncRepository<T> repository;
        
        public SqlSnapshotRepository(SQLiteConnection connection)
        {
            var repo = new SqlRepository<T>(connection);
            this.repository = new SyncRepository<T>(repo);
        }

        public ISnapshot New()
        {
            return new T();
        }

        public ISnapshot GetById(Guid id)
        {
            return ((T)this.repository.GetById(id));
        }

        public IEnumerable<ISnapshot> GetAll()
        {
            return this.repository.GetAll().Cast<ISnapshot>();
        }

        public SaveResult Save(ISnapshot instance)
        {
            return this.repository.Save((T)instance);
        }

        public void Delete(ISnapshot instance)
        {
            this.repository.Delete((T)instance);
        }

        public void DeleteId(Guid id)
        {
            this.repository.DeleteId(id);
        }

        public void Dispose()
        {
            this.repository.Dispose();
        }
    }
}

