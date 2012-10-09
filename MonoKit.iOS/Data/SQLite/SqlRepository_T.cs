// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlRepository_T.cs" company="sgmunn">
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
    using MonoKit.Reactive.Subjects;
    using MonoKit.Tasks;

    // todo: using SyncScheduler to ensure that only one thread accesses the connection as per gist but..
    // I'm using this to block the current thread so not really async.  more sqlite threading investigation

    public class SqlRepository<T> : IRepository<T>, IConnectedRepository, IObservableRepository 
        where T: IId, new()
    {
        private readonly SQLiteConnection connection;

        private readonly Subject<IDataChangeEvent> changes;
        
        public SqlRepository(SQLiteConnection connection)
        {
            this.connection = connection;
            this.changes = new Subject<IDataChangeEvent>();
        }

        object IConnectedRepository.Connection
        {
            get
            {
                return this.Connection;
            }
        }

        public IObservable<IDataChangeEvent> Changes
        {
            get
            {
                return this.changes;
            }
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

        public virtual T New()
        {
            return new T();
        }

        public virtual T GetById(Guid id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id", "Cannot Get an entity by id with a null id");
            }

            try
            {
                return SynchronousTask.GetSync(() => this.Connection.Get<T>(id));
            }
            catch
            {
                return default(T);
            }
        }

        public virtual IEnumerable<T> GetAll()
        {
            return SynchronousTask.GetSync(() => this.Connection.Table<T>().AsEnumerable());
        }

        public virtual SaveResult Save(T instance)
        {
            var result = SaveResult.Updated;

            SynchronousTask.DoSync(() => {
                Console.WriteLine(string.Format("SqlRepo - Save {0}", instance));

                if (this.Connection.Update(instance) == 0)
                {
                    this.Connection.Insert(instance);
                    result = SaveResult.Added;
                }
            });

            IDataChangeEvent modelChange = null;
            switch (result)
            {
                case SaveResult.Added: 
                    modelChange = new DataChangeEvent<T>(instance.Identity, instance, DataChangeKind.Added);
                    break;
                case SaveResult.Updated:
                    modelChange = new DataChangeEvent<T>(instance.Identity, instance, DataChangeKind.Changed);
                    break;
            }

            this.changes.OnNext(modelChange);

            return result;
        }

        public virtual void Delete(T instance)
        {
            SynchronousTask.DoSync(() => this.Connection.Delete(instance));
            var modelChange = new DataChangeEvent<T>(instance.Identity, DataChangeKind.Deleted);
            this.changes.OnNext(modelChange);
        }

        public virtual void DeleteId(Guid id)
        {
            SynchronousTask.DoSync(() => {
                Console.WriteLine(string.Format("SqlRepo - Delete {0} {1}", typeof(T), id));
                var map = this.Connection.GetMapping(typeof(T));
                var pk = map.PK;

                if (pk == null) 
                {
                    throw new NotSupportedException ("Cannot delete " + map.TableName + ": it has no PK");
                }

                var q = string.Format ("delete from \"{0}\" where \"{1}\" = ?", map.TableName, pk.Name);
                this.Connection.Execute (q, id);

                var modelChange = new DataChangeEvent<T>(id, DataChangeKind.Deleted);
                this.changes.OnNext(modelChange);
            });
        }
    }    
}
