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
    }

        
    public class MinionContext : SQLiteDomainContext
    {
        public MinionContext(SQLiteConnection connection, IEventStoreRepository eventStore, IDomainEventBus eventBus) : base(connection, eventStore, eventBus)
        {
        }
    }



    public class MinionDataContract : ISnapshot
    {
          [PrimaryKey]
          public Guid Id { get; set; }
        
          public int Version { get; set; }
        
          public string Name { get; set; }  
        
          public decimal WeeklyAllowance { get; set; }
        
          public decimal Balance { get; set; }
    }

    public class Minion : AggregateRoot<MinionDataContract>
    {
        public void Execute(EarnPocketMoneyCommand command)
        {
            this.NewEvent(new PocketMoneyEarntEvent() { Date = command.Date, Amount = command.Amount });
        }

        public void Apply(PocketMoneyEarntEvent @event)
        {
            this.InternalState.Balance += @event.Amount;
        }
    }

    public class EarnPocketMoneyCommand : CommandBase
    {
        public DateTime Date { get; set; }
        
        public decimal Amount { get; set; }
    } 

    public class PocketMoneyEarntEvent : EventBase
    {
        public DateTime Date { get; set; }
        
        public decimal Amount { get; set; }
    }

    public class PocketMoneyTransactionDataContract
    {
        public PocketMoneyTransactionDataContract()
        {
            this.Id = Guid.NewGuid();
        }

        [PrimaryKey]
        public Guid Id { get; set; }
        
        [Indexed]
        public Guid MinionId { get; set; }
        
        public DateTime Date { get; set; }
        
        public decimal Amount { get; set; }
    }

    
    public class TransactionReadModelBuilder : ReadModelBuilder
    {
        private IRepository<PocketMoneyTransactionDataContract> repository;

        public TransactionReadModelBuilder(IRepository<PocketMoneyTransactionDataContract> repository)
        {
            Console.WriteLine("read model created");
            this.repository = repository;
        }
        
        public void Handle(PocketMoneyEarntEvent @event)
        {
            Console.WriteLine("read model updated");
            var transaction = this.repository.New();
            transaction.MinionId = @event.AggregateId;
            transaction.Amount = @event.Amount;
            transaction.Date = @event.Date;

            this.repository.Save(transaction);
        }
    }

}

