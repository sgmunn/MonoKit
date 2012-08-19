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

namespace MonoKit.Domain.Data.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MonoKit.Data.SQLite;
    using MonoKit.Data;
    using MonoKit.Tasks;
    
    public class EventStoreRepository : SqlRepository<SerializedEvent>, IEventStoreRepository
    {
        public EventStoreRepository(SQLiteConnection connection) : base(connection)
        {
        }

        ISerializedEvent IRepository<ISerializedEvent, IUniqueIdentity>.New()
        {
            return (ISerializedEvent)base.New();
        }

        ISerializedEvent IRepository<ISerializedEvent, IUniqueIdentity>.GetById(IUniqueIdentity id)
        {
            return (ISerializedEvent)base.GetById(id);
        }

        IEnumerable<ISerializedEvent> IRepository<ISerializedEvent, IUniqueIdentity>.GetAll()
        {
            return (IEnumerable<ISerializedEvent>)base.GetAll();
        }

        public void Save(ISerializedEvent instance)
        {
            base.Save((SerializedEvent)instance);
        }

        public void Delete(ISerializedEvent instance)
        {
            base.Delete((SerializedEvent)instance);
        }

        public IEnumerable<ISerializedEvent> GetAllAggregateEvents(IUniqueIdentity rootId)
        {
            var result = SynchronousTask.GetSync(() =>
                this.Connection.Table<SerializedEvent>().Where(x => x.AggregateId == rootId.Id).OrderBy(x => x.Version).AsEnumerable());

            return result.Cast<ISerializedEvent>();
        }
    }
}

