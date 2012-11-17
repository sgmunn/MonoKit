// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadModelBuildingEventBus_T.cs" company="sgmunn">
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
    using MonoKit.Data;

    // todo: change from T to pass in the registerd builders
    public class ReadModelBuildingEventBus<T> : INotificationEventBus 
        where T : IAggregateRoot, new()
    {
        private readonly IDomainContext context;

        private readonly INotificationEventBus bus;
        
        public ReadModelBuildingEventBus(IDomainContext context, INotificationEventBus bus)
        {
            this.context = context;
            this.bus = bus;
        }
        
        public void Publish(INotificationEvent evt)
        {
            if (this.bus != null)
            {
                this.bus.Publish(evt);
            }

            var builders = this.context.GetReadModelBuilders<T>(this.bus);

            var updatedReadModels = new List<IDataChangeEvent>();

            // todo: this coud be done async, but because we should only have one thread to the db at any one time it's not really worth it.
            // just don't take too long in any one builder and don't make assumptions on the order of builders being executed.
            foreach (var builder in builders)
            {
                updatedReadModels.AddRange(builder.Handle(evt));
            }

            if (this.bus != null)
            {
                foreach (var readModel in updatedReadModels)
                {
                    this.bus.Publish(new NotificationEvent(readModel.DataType, readModel.DataId, readModel));
                }
            }
        }

        public IDisposable Subscribe(IObserver<INotificationEvent> subscriber)
        {
            throw new NotSupportedException();
        }
    }
}

