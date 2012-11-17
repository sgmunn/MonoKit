// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregateRoot.cs" company="sgmunn">
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
    using MonoKit.Runtime;

    public abstract class AggregateRoot : IAggregateRoot
    {
        private readonly List<IAggregateEvent> uncommittedEvents;

        public AggregateRoot()
        {
            this.uncommittedEvents = new List<IAggregateEvent>();
        }

        public Guid Identity { get; set; }

        public int Version { get; protected set; }

        public IEnumerable<IAggregateEvent> UncommittedEvents
        {
            get
            {
                return this.uncommittedEvents;
            }
        }

        public void Commit()
        {
            this.uncommittedEvents.Clear();
        }

        protected void ApplyEvents(IList<IAggregateEvent> events)
        {
            if (this.Identity == Guid.Empty && events.Any())
            {
                this.Identity = events.First().AggregateId;
            }

            foreach (var evt in events)
            {
                this.ApplyEvent(evt);
                this.Version = evt.Version;
            }
        }

        protected void RaiseEvent(Guid aggregateId, IAggregateEvent evt)
        {
            if (aggregateId == Guid.Empty)
            {
                throw new ArgumentNullException("aggregateId", "Cannot raise an event without specifying the correct aggregate id");
            }

            if (this.Identity == Guid.Empty)
            {
                this.Identity = aggregateId;
            }

            if (this.Identity != aggregateId)
            {
                throw new InvalidOperationException("Cannot raise an event for a different aggregate root id");
            }

            this.Version++;

            evt.Identity = Guid.NewGuid();
            evt.AggregateId = aggregateId;
            evt.Version = this.Version;
            evt.Timestamp = DateTime.UtcNow;

            this.ApplyEvent(evt);
            this.uncommittedEvents.Add(evt);
        }

        private void ApplyEvent(IAggregateEvent evt)
        {
            if (!MethodExecutor.ExecuteMethod(this, evt))
            {
                throw new MissingMethodException(string.Format("Aggregate {0} does not support a method that can be called with {1}", this, evt));
            }
        }
    }
}

