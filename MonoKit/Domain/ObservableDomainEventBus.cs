// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableDomainEventBus.cs" company="sgmunn">
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
    using MonoKit.Reactive.Subjects;
    using MonoKit.Data;

    public class ObservableDomainEventBus : IDomainEventBus
    {
        private readonly Subject<IDataEvent> eventPublisher;
        private readonly Subject<IReadModelChange> readModelPublisher;

        public ObservableDomainEventBus()
        {
            this.eventPublisher = new Subject<IDataEvent>();
            this.readModelPublisher = new Subject<IReadModelChange>();
        }

        public void Publish(IDataEvent @event)
        {
            this.eventPublisher.OnNext(@event);
        }

        public void Publish(IReadModelChange readModelChange)
        {
            this.readModelPublisher.OnNext(readModelChange);
        }

        public IDisposable Subscribe(IObserver<IDataEvent> observer)
        {
            return this.eventPublisher.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<IReadModelChange> observer)
        {
            return this.readModelPublisher.Subscribe(observer);
        }
    }
}

