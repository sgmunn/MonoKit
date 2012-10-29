// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryEventStoreRepository_T.cs" company="sgmunn">
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

namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MonoKit.Data;
    
    public class InMemoryEventStoreRepository<T> : DictionaryRepository<ISerializedAggregateEvent>, IEventStoreRepository 
        where T : ISerializedAggregateEvent, new() 
    {
        protected override ISerializedAggregateEvent InternalNew()
        {
            return new T(); 
        }

        protected override SaveResult InternalSave(ISerializedAggregateEvent evt)
        {
            if (this.Storage.ContainsKey(evt.Identity))
            {
                this.Storage[evt.Identity] = evt;
                return SaveResult.Updated;
            }

            this.Storage[evt.Identity] = evt;
            return SaveResult.Added;
        }

        protected override void InternalDelete(ISerializedAggregateEvent evt)
        {
            if (this.Storage.ContainsKey(evt.Identity))
            {
                this.Storage.Remove(evt.Identity);
            }
        }

        public IList<ISerializedAggregateEvent> GetAllAggregateEvents(Guid rootId)
        {
            return this.GetAll().Where(x => x.AggregateId == rootId).OrderBy(x => x.Version).ToList();
        }
    }
}

