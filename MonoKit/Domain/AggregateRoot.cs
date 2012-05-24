// --------------------------------------------------------------------------------------------------------------------
// <copyright file=".cs" company="sgmunn">
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
    using MonoKit.Domain.Commands;
    using MonoKit.Domain.Events;

    public abstract class AggregateRoot : IAggregateRoot
    {
        // todo: 
        private static MethodExecutor Executor = new MethodExecutor();

        private readonly List<IEvent> uncommittedEvents;

        public AggregateRoot()
        {
            this.uncommittedEvents = new List<IEvent>();
        }

        public Guid AggregateId { get; protected set; }

        public int Version { get; protected set; }

        public bool Created { get; private set; }

        public IEnumerable<IEvent> UncommittedEvents
        {
            get
            {
                return this.uncommittedEvents;
            }
        }
        
        public virtual void Execute(CreateCommand command)
        {
            if (this.Created)
            {
                throw new InvalidOperationException(string.Format("The Aggregate {0} has already been created.", this.GetType().Name));
            }

            this.AggregateId = command.AggregateId;
            this.NewEvent(new CreatedEvent());
        }
        
        public void Apply(CreatedEvent @event)
        {
            this.AggregateId = @event.AggregateId;
        }

        public void Commit()
        {
            this.uncommittedEvents.Clear();
        }

        protected virtual string GetAggregateTypeId()
        {
            return this.GetType().Name;
        }

        protected void ApplyEvents(IList<IEvent> events)
        {
            foreach (var @event in events)
            {
                this.ApplyEvent(@event);
                this.Version = @event.Version;
            }
        }

        protected void NewEvent(EventBase @event)
        {
            this.Version++;

            @event.AggregateId = this.AggregateId;
            @event.AggregateTypeId = this.GetAggregateTypeId();
            @event.Version = this.Version;
            @event.Timestamp = DateTime.UtcNow;

            this.ApplyEvent(@event);
            this.uncommittedEvents.Add(@event);
        }

        private void ApplyEvent(IEvent @event)
        {
            Console.WriteLine("{0} - Apply {1} event {2}",  @event.Version,  @event.GetType().Name,  @event.Timestamp);

            if (!Executor.ExecuteMethodForSingleParam(this, @event))
            {
                throw new MissingMethodException(string.Format("Aggregate {0} does not support a method that can be called with {1}", this, @event));
            }
        }
    }

    public abstract class AggregateRoot<TState> : AggregateRoot, ISnapshotSupport where TState : class, ISnapshot, new()
    {
        public AggregateRoot()
        {
            this.InternalState = new TState();
        }

        protected TState InternalState { get; private set; }
                
        public override void Execute(CreateCommand command)
        {
            base.Execute(command);
        }


        public void LoadFromSnapshot(ISnapshot snapshot)
        {
            this.InternalState = snapshot as TState;
            this.Version = snapshot.Version;
            this.AggregateId = snapshot.Id;
        }

        public ISnapshot GetSnapshot()
        {
            var snapshot = this.InternalState;
            snapshot.Id = this.AggregateId;
            snapshot.Version = this.Version;

            return snapshot;
        }
    }
}

