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
    using MonoKit.Data;

    public abstract class AggregateRoot : IAggregateRoot
    {
        private readonly List<IAggregateEvent> uncommittedEvents;

        public AggregateRoot()
        {
            this.uncommittedEvents = new List<IAggregateEvent>();
        }

        public IUniqueIdentity Identity { get; protected set; }

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
            if (this.Identity == null && events.Any())
            {
                this.Identity = events.First().AggregateId;
            }

            foreach (var evt in events)
            {
                this.ApplyEvent(evt);
                this.Version = evt.Version;
            }
        }

        protected void RaiseEvent(EventBase evt)
        {
            if (this.Identity == null)
            {
                this.Identity = evt.AggregateId;
            }

            this.Version++;

            evt.AggregateId = this.Identity;
            evt.IdentityType = this.Identity.GetType().Name;
            evt.Version = this.Version;
            evt.Timestamp = DateTime.UtcNow;

            this.ApplyEvent(evt);
            this.uncommittedEvents.Add(evt);
        }

        private void ApplyEvent(IAggregateEvent evt)
        {
            if (!MethodExecutor.ExecuteMethodForSingleParam(this, evt))
            {
                throw new MissingMethodException(string.Format("Aggregate {0} does not support a method that can be called with {1}", this, evt));
            }
        }
    }
}

