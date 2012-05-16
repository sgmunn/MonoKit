using System;
using MonoKit.Domain.Data;
using MonoKit.Data;
using MonoKit.Domain;
using MonoKit.Domain.Commands;
using System.Runtime.Serialization;
using MonoKit.Domain.Events;
using System.Collections.Generic;
using System.IO;

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
    
    public class TestDenormalizer : Denormalizer
    {
        public TestDenormalizer(IRepository<TestReadModel> repository)
        {
        }
        
        public void HandleMe(CreatedEvent @event)
        {
            Console.WriteLine("Denormalize Created Event {0}", @event.AggregateType);
        }
    }

    public  class SampleSerializer<T> : ISerializer
    {
        private  readonly DataContractSerializer singletonInstance;

        public SampleSerializer()
        {
            singletonInstance = new DataContractSerializer(
                typeof(T),
                new List<Type> {  typeof(EventBase), typeof(CreatedEvent), typeof(TestEvent)}
                );
        }

        public  object DeserializeFromString(string value)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(value);
                writer.Flush();
                stream.Position = 0;

                return (T)singletonInstance.ReadObject(stream);
            }
        }

        public  string SerializeToString(object value)
        {
            using (var stream = new MemoryStream())
            {
                singletonInstance.WriteObject(stream, value);
                stream.Flush();
                stream.Position = 0;
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }
    }

}

