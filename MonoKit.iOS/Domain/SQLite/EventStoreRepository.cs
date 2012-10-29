// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventStoreRepository.cs" company="sgmunn">
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
    using MonoKit.Data.SQLite;
    using MonoKit.Data;
    using MonoKit.Tasks;
    
    public class EventStoreRepository : SqlRepository<SerializedAggregateEvent>, IEventStoreRepository
    {
        public EventStoreRepository(SQLiteConnection connection) : base(connection)
        {
        }

        ISerializedAggregateEvent IRepository<ISerializedAggregateEvent, Guid>.New()
        {
            return (ISerializedAggregateEvent)base.New();
        }

        ISerializedAggregateEvent IRepository<ISerializedAggregateEvent, Guid>.GetById(Guid id)
        {
            return (ISerializedAggregateEvent)base.GetById(id);
        }

        IList<ISerializedAggregateEvent> IRepository<ISerializedAggregateEvent, Guid>.GetAll()
        {
            return (IList<ISerializedAggregateEvent>)base.GetAll();
        }

        public SaveResult Save(ISerializedAggregateEvent instance)
        {
            return base.Save((SerializedAggregateEvent)instance);
        }

        public void Delete(ISerializedAggregateEvent instance)
        {
            base.Delete((SerializedAggregateEvent)instance);
        }

        public IList<ISerializedAggregateEvent> GetAllAggregateEvents(Guid rootId)
        {
            //var result = SynchronousTask.GetSync(() =>
            //    this.Connection.Table<SerializedAggregateEvent>().Where(x => x.AggregateId == rootId).OrderBy(x => x.Version).AsEnumerable());

            lock (this.Connection)
            {
                var result = this.Connection.Table<SerializedAggregateEvent>().Where(x => x.AggregateId == rootId).OrderBy(x => x.Version).AsEnumerable();
                return result.Cast<ISerializedAggregateEvent>().ToList();
            }
        }
    }
}

