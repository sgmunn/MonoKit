//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file=".cs" company="sgmunn">
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
    using System.Collections.Generic;
    using System.Linq;
    using MonoKit.Data;
    
    public class UnitOfWorkEventBus : IEventBus, IUnitOfWork
    {
        private readonly IEventBus bus;

        private readonly List<IEvent> events;

        private readonly List<IReadModelChange> readModels;
        
        public UnitOfWorkEventBus(IEventBus bus)
        {
            this.events = new List<IEvent>();
            this.readModels = new List<IReadModelChange>();
            this.bus = bus;
        }
        
        public void Publish(IEvent evt)
        {
            this.events.Add(evt);
        }

        public void Publish(IReadModelChange readModelChange)
        {
            this.readModels.Add(readModelChange);
        }

        public void Dispose()
        {
            this.events.Clear();
            this.readModels.Clear();
        }

        public void Commit()
        {
            if (this.bus != null)
            {
                foreach (var evt in this.events)
                {
                    this.bus.Publish(evt);
                }

                // todo: just need to check that we don't need type info here and that Id will be
                // enough

                // only publish each read model once and then the last one
                var distinctIds = this.readModels.Select(x => x.Identity).Distinct().ToList();

                foreach (var readModelId in distinctIds)
                {
                    this.bus.Publish(this.readModels.Last(x => x.Identity == readModelId));
                }
            }
        }
    }
}
