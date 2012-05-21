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
    public class MinionContext : SQLiteDomainContext
    {
        public MinionContext(SQLiteConnection connection, IEventStoreRepository eventStore, IDomainEventBus eventBus) : base(connection, eventStore, eventBus)
        {
        }
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
    
    public class MinionDataContract : ISnapshot
    {
          [PrimaryKey]
          public Guid Id { get; set; }
        
          public int Version { get; set; }
        
          public string Name { get; set; }  
        
          public decimal WeeklyAllowance { get; set; }
        
          public decimal Balance { get; set; }
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
