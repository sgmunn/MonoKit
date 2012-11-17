using System;
using NUnit.Framework;
using MonoKit.Data;

namespace MonoKit.Domain.UnitTests.Repositories.EventSourcing
{
    [TestFixture]
    public class GivenAExistingAggregateWithUncommittedEvents : GivenAnEventSourcedAggregateRepository
    {
        public TestAggregateRoot Aggregate { get; private set; }
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            var id = Guid.NewGuid();
            this.Aggregate = new TestAggregateRoot();
            this.Aggregate.Execute(new TestCommand1{ AggregateId = id, Value1 = "1", Value2 = 1, });
            this.Repository.Save(this.Aggregate);

            this.Aggregate.Execute(new TestCommand1{ AggregateId = id, Value1 = "2", Value2 = 2, });
        }
        
        [Test]
        public void WhenSavingTheAggregate_ThenTheSaveResultIsAdded()
        {
            var result = this.Repository.Save(this.Aggregate);
            Assert.AreEqual(SaveResult.Updated, result);
        }
    }
}
