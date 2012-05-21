// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregateRoot.cs" company="sgmunn">
//   (c) sgmunn 2012  
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

