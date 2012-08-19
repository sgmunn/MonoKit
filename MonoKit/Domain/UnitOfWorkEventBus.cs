//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="UnitOfWorkEventBus.cs" company="sgmunn">
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
    using MonoKit.Data;
    
    public class UnitOfWorkEventBus : IDataModelEventBus, IUnitOfWork
    {
        private readonly IDataModelEventBus bus;

        private readonly List<IDataModelEvent> events;
        
        public UnitOfWorkEventBus(IDataModelEventBus bus)
        {
            this.events = new List<IDataModelEvent>();
            this.bus = bus;
        }
        
        public void Publish(IDataModelEvent evt)
        {
            this.events.Add(evt);
        }

        public void Dispose()
        {
            this.events.Clear();
        }

        public void Commit()
        {
            if (this.bus != null)
            {
                // todo: maybe deduplicate events, use a dictionary of id / type etc
                foreach (var evt in this.events)
                {
                    this.bus.Publish(evt);
                }
            }
        }
    }
}
