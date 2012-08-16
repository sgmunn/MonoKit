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
using System.Runtime.Serialization;
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
            this.RaiseEvent(new PocketMoneyEarntEvent() { Date = command.Date, Amount = command.Amount });
        }

        public void Apply(PocketMoneyEarntEvent @event)
        {
            this.InternalState.Balance += @event.Amount;
        }

        public override ISnapshot GetSnapshot()
        {
            var snapshot = this.InternalState;
            snapshot.Id = this.Identity.Id;
            snapshot.Version = this.Version;
            return snapshot;
        }
    }
    
    public class MinionDataContract : ISnapshot
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

    public class PocketMoneyTransactionDataContract : IReadModel
    {
        public PocketMoneyTransactionDataContract()
        {
            this.Id = new Identity(Guid.NewGuid());
        }

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
            transaction.MinionId = @event.AggregateId.Id;
            transaction.Amount = @event.Amount;
            transaction.Date = @event.Date;

            this.repository.Save(transaction);
        }
    }


}
