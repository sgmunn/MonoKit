// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventStoreRepository_T.cs" company="sgmunn">
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
    using System.Threading.Tasks;
    using System.Threading;
    using MonoKit.Data.SQLite;
    using MonoKit.Data;
    using MonoKit.Tasks;
    
    public class EventStoreRepository<T> : IEventStoreRepository where T : ISerializedEvent, new() 
    {
        private readonly IEventStoreRepository repository;
        
        public EventStoreRepository(SQLiteConnection connection)
        {
            this.repository = new InternalEventStoreRepository<T>(connection);
        }

        public ISerializedEvent New()
        {
            return Task.Factory.StartNew<ISerializedEvent>(
                () => { return this.repository.New(); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                SyncScheduler.TaskScheduler).Result;
        }

        public ISerializedEvent GetById(IUniqueIdentity id)
        {
            return Task.Factory.StartNew<ISerializedEvent>(
                () => { return this.repository.GetById(id); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                SyncScheduler.TaskScheduler).Result;
        }

        public IEnumerable<ISerializedEvent> GetAll()
        {
            return Task.Factory.StartNew<IEnumerable<ISerializedEvent>>(
                () => { return this.repository.GetAll(); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                SyncScheduler.TaskScheduler).Result;
        }

        public void Save(ISerializedEvent instance)
        {
            Task.Factory.StartNew(
                () => { this.repository.Save(instance); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                SyncScheduler.TaskScheduler).Wait();
        }

        public void Delete(ISerializedEvent instance)
        {
            Task.Factory.StartNew(
                () => { this.repository.Delete(instance); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                SyncScheduler.TaskScheduler).Wait();
        }

        public void DeleteId(IUniqueIdentity id)
        {
            Task.Factory.StartNew(
                () => { this.repository.DeleteId(id); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                SyncScheduler.TaskScheduler).Wait();
        }

        public IEnumerable<ISerializedEvent> GetAllAggregateEvents(IUniqueIdentity rootId)
        {
            return Task.Factory.StartNew<IEnumerable<ISerializedEvent>>(
                () => { return this.repository.GetAllAggregateEvents(rootId); }, 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                SyncScheduler.TaskScheduler).Result;
        }

        public void Dispose()
        {
            this.repository.Dispose();
        }
    }
}

