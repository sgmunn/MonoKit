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

namespace MonoKit.Data.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading;
    
    public class SQLiteRepository<T> : IRepository<T> where T: new()
    {
        private readonly SQLiteConnection connection;
        
        public SQLiteRepository(SQLiteConnection connection)
        {
            this.connection = connection;
        }

        public SQLiteConnection Connection
        {
            get
            {
                return this.connection;
            }
        }

        public void Dispose()
        {
        }

        public T New()
        {
            return new T();
        }

        public T GetById(object id)
        {
            try
            {
                return GetSync(() => this.connection.Get<T>(id));
            }
            catch
            {
                return default(T);
            }
        }

        public IEnumerable<T> GetAll()
        {
            return GetSync(() => this.connection.Table<T>().AsEnumerable());
        }

        public void Save(T instance)
        {
            DoSync
                (() => 
                 {
                    if (this.connection.Update(instance) == 0)
                    {
                        this.connection.Insert(instance);
                    }
                });
        }

        public void Delete(T instance)
        {
            DoSync(() => this.connection.Delete(instance));
        }

        public void DeleteId(object id)
        {
            throw new NotImplementedException();
            // todo: have to get table name and primary key 
        }

        // todo: using SyncScheduler to ensure that only one thread accesses the connection as per gist but..
        // I'm using this to block the current thread so not really async.  more sqlite threading investigation
        protected static TTask GetSync<TTask>(Func<TTask> task)
        {
            return Task.Factory.StartNew<TTask>
                (() => 
                 { 
                    return task(); 
                }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                SyncScheduler.TaskScheduler).Result;
        }

        protected static void DoSync(Action task)
        {
            Task.Factory.StartNew
                (() => 
                 { 
                    task(); 
                }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                SyncScheduler.TaskScheduler).Wait();
        }
    }    
}
