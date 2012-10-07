//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ReadModel.cs" company="sgmunn">
//    (c) sgmunn 2012  
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//    documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//    to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//    the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//    THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//    CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//    IN THE SOFTWARE.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

namespace MonoKitSample.Domain
{
    using System;
    using MonoKit.Data;
    using MonoKit.Data.SQLite;
    using MonoKit.Domain;

    public class TestReadModel : IId
    {
        [PrimaryKey]
        public Guid Identity { get; set;}
    }
    
    public class TestBuilder : ReadModelBuilder<TestReadModel>
    {
        public TestBuilder(IRepository<TestReadModel> repository) : base(repository)
        {
        }
        
        public void HandleMe(TestEvent1 @event)
        {
            Console.WriteLine("Builder TestEvent1 {0}", @event.Identity);
        }
        
        public void HandleMe(TestEvent2 @event)
        {
            Console.WriteLine("Builder TestEvent2 {0}", @event.Identity);
        }
    }

    public class TransactionDataContract : IId
    {
        public TransactionDataContract()
        {
            this.Identity = Guid.NewGuid();
        }

        [PrimaryKey]
        public Guid Identity { get; set; }

        [Indexed]
        public Guid TestId { get; set; }
        
        public string Description { get; set; }
        
        public decimal Amount { get; set; }

//        public class TransactionId : Identity
//        {
//            public TransactionId(Guid id) : base(id)
//            {
//            }
//        }

        public override string ToString()
        {
            return string.Format("readmodel [{0}] {1}", this.Description, this.Amount);
        }
    }
    
    public class TransactionReadModelBuilder : ReadModelBuilder<TransactionDataContract>
    {
        public TransactionReadModelBuilder(IRepository<TransactionDataContract> repository) : base(repository)
        {
            Console.WriteLine("read model builder created");
        }
        
        public void Handle(TestEvent2 evt)
        {
            Console.WriteLine("read model updated");
            var transaction = this.Repository.New();

            transaction.TestId = evt.Identity;
            transaction.Amount = evt.Amount;
            transaction.Description = evt.Description;

            this.Repository.Save(transaction);
        }
    }
}

