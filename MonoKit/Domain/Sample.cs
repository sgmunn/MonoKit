using System;
using MonoKit.Domain.Data;
using MonoKit.Data;

namespace MonoKit.Domain
{
    using System.Runtime.Serialization;

    public class CreateCommand : CommandBase
    {

    }

    public class TestCommand2 : CommandBase
    {

    }
    
    public class TestState : ISnapshot
    {
        public int Version { get; set; }
    }
    
    public class TestStateRoot : AggregateRoot<TestState>
    {
        
    }

    public class TestRoot : AggregateRoot, IEventSourced
    {
        public TestRoot()
            : base()
        {

        }

        public void Test()
        {

        }

        // [CommandHandler]
        public void Test(CreateCommand command)
        {
            // bit of a double up
            this.AggregateId = command.AggregateId;
            Console.WriteLine("Test");
            this.NewEvent(new CreatedEvent() { Test = "Test", });
            this.NewEvent(new NextEvent() );
        }

        public void Test(TestCommand2 command)
        {
            Console.WriteLine("Test2");
            this.NewEvent(new TestEvent2());
        }

        public void Apply(CreatedEvent domainEvent)
        {
            this.AggregateId = domainEvent.AggregateId;
            Console.WriteLine("Created event {0}", domainEvent.Version);
        }

        public void Apply(NextEvent domainEvent)
        {
            Console.WriteLine("Next event {0}", domainEvent.Version);
        }

        public void Apply(TestEvent2 domainEvent)
        {
            Console.WriteLine("Test event {0}", domainEvent.Version);
        }

        public void LoadFromEvents(System.Collections.Generic.IList<IEvent> events)
        {
            base.ApplyEvents(events);
        }
    }

    [DataContract(Name="Created", Namespace="http://sgmunn.com/MonoKit/Domain")]
    public class CreatedEvent : EventBase
    {
        [DataMember]
        public string Test { get; set; }
    }

    [DataContract(Name="Next", Namespace="http://sgmunn.com/MonoKit/Domain")]
    public class NextEvent : EventBase
    {
    }

    [DataContract(Name="Test", Namespace="http://sgmunn.com/MonoKit/Domain")]
    public class TestEvent2 : EventBase
    {
    }
    
    public class TestReadModel
    {
        
    }
    
    public class TestDenormalizer : Denormalizer
    {
        public TestDenormalizer(IRepository<TestReadModel> repository)
        {
        }
        
        public void Handle(CreatedEvent @event)
        {
            Console.WriteLine("Denormalize Created Event");
        }
    }
}

