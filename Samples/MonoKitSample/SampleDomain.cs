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

using System;
using MonoKit.Domain.Data;
using MonoKit.Data;
using MonoKit.Domain;
using MonoKit.Domain.Commands;
using System.Runtime.Serialization;
using MonoKit.Domain.Events;
using System.Collections.Generic;
using System.IO;
using MonoKit.Data.SQLite;
using MonoKit.Domain.Data.SQLite;

namespace MonoKitSample
{
    public class EventSourcedTestRoot : AggregateRoot, IEventSourced
    {
        public EventSourcedTestRoot() : base()
        {
        }

        public void Execute(TestCommand command)
        {
            Console.WriteLine("Execuite TestCommand");
            this.NewEvent(new TestEvent() { Description = command.Description, });
        }

        public void Apply(TestEvent domainEvent)
        {
            Console.WriteLine("apply TestEvent {0}", domainEvent.Description);
        }

        public void LoadFromEvents(System.Collections.Generic.IList<IEvent> events)
        {
            base.ApplyEvents(events);
        }
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

    public class TestSnapshot : ISnapshot
    {
        [MonoKit.Data.SQLite.PrimaryKey]
        public Guid Id { get; set; }
        public int Version { get; set; }
        public string Description { get; set; }
    }
    
    public class SnapshotTestRoot : AggregateRoot<TestSnapshot>
    {
    
        public void Execute(TestCommand command)
        {
            Console.WriteLine("Snapshot Execute TestCommand");
            this.NewEvent(new TestEvent() { Description = command.Description, });
        }

        public void Apply(TestEvent domainEvent)
        {
            Console.WriteLine("Snapshot apply TestEvent {0}", domainEvent.Description);
            this.InternalState.Description = domainEvent.Description;
        }
    
    }

    public class TestReadModel
    {
        
    }
    
    public class TestBuilder : ReadModelBuilder
    {
        public TestBuilder(IRepository<TestReadModel> repository)
        {
        }
        
        public void HandleMe(CreatedEvent @event)
        {
            Console.WriteLine("Builder Created Event {0}", @event.AggregateTypeId);
        }
        
        protected override void DoSaveReadModel(IReadModel readModel)
        {
        }

    }
}

