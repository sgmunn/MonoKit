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


namespace MonoKitSample
{
    using System;
    using MonoKit.Domain.Data;
    using MonoKit.Data;
    using MonoKit.Domain;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using System.IO;
    using MonoKit.Data.SQLite;
    using MonoKit.Domain.Data.SQLite;

    public class CreateCommand : CommandBase
    {
    }
        
    [DataContract(Name="Created", Namespace="http://sgmunn.com/2012/Sample/Domain")]
    public class CreatedEvent : EventBase
    {
    }

    public class TestCommand : CommandBase
    {
        public string Description { get; set; }
    }
        
    [DataContract(Name="Test", Namespace="http://sgmunn.com/2012/Sample/Domain")]
    public class TestEvent : EventBase
    {
        [DataMember]
        public string Description { get; set; }
    }





    public class EventSourcedTestRoot : AggregateRoot, IEventSourced
    {
        public EventSourcedTestRoot() : base()
        {
        }
   
        public void Execute(CreateCommand command)
        {
            Console.WriteLine("Execute CreateCommand");
            this.RaiseEvent(new CreatedEvent() );
        }

        public void Apply(CreatedEvent domainEvent)
        {
            Console.WriteLine("Apply CreateCommand {0}", domainEvent.Version);
        }

        public void Execute(TestCommand command)
        {
            Console.WriteLine("Execuite TestCommand");
            this.RaiseEvent(new TestEvent() { Description = command.Description, });
        }

        public void Apply(TestEvent domainEvent)
        {
            Console.WriteLine("Apply TestEvent {0} - {1}", domainEvent.Version, domainEvent.Description);
        }

        public void LoadFromEvents(IList<IAggregateEvent> events)
        {
            base.ApplyEvents(events);
        }
    }




    public class TestSnapshot : ISnapshot
    {
        [MonoKit.Data.SQLite.Ignore]
        public IUniqueIdentity Identity
        {
            get
            {
                return new Identity(this.Id);
            }

        }

        [MonoKit.Data.SQLite.PrimaryKey]
        public Guid Id { get; set; }


        public int Version { get; set; }
        public string Description { get; set; }
    }
    
    public class SnapshotTestRoot : AggregateRoot<TestSnapshot>
    {
   
        public void Execute(CreateCommand command)
        {
            Console.WriteLine("Snapshot Execute CreateCommand");
            this.RaiseEvent(new CreatedEvent() );
        }

        public void Apply(CreatedEvent domainEvent)
        {
            Console.WriteLine("Snapshot Apply CreateCommand {0}", domainEvent.Version);
        }

        public void Execute(TestCommand command)
        {
            Console.WriteLine("Snapshot Execuite TestCommand");
            this.RaiseEvent(new TestEvent() { Description = command.Description, });
        }

        public void Apply(TestEvent domainEvent)
        {
            Console.WriteLine("Snapshot Apply TestEvent {0} - {1}", domainEvent.Version, domainEvent.Description);
        }

        public override ISnapshot GetSnapshot()
        {
            var snapshot = this.InternalState;
            snapshot.Id = this.Identity.Id;
            snapshot.Version = this.Version;
            return snapshot;
        }
    }

    public class TestReadModel : IDataModel
    {
        #region IDataModel implementation
        public IUniqueIdentity Identity
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }
        #endregion
    }
    
    public class TestBuilder : ReadModelBuilder
    {
        public TestBuilder(IRepository<TestReadModel> repository)
        {
        }
        
        public void HandleMe(CreatedEvent @event)
        {
            Console.WriteLine("Builder Created Event {0}", @event.Identity);
        }
    }
}

