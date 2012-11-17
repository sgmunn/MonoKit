
using System;
using NUnit.Framework;
using MonoKit.Data;

namespace MonoKit.Domain.UnitTests.Repositories.EventSourcing
{
    [TestFixture]
    public class GivenANewAggregateWithUncommittedEvents : GivenAnEventSourcedAggregateRepository
    {
        public TestAggregateRoot Aggregate { get; private set; }
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var id = Guid.NewGuid();
            this.Aggregate = new TestAggregateRoot();
            this.Aggregate.Execute(new TestCommand1{ AggregateId = id, Value1 = "1", Value2 = 1, });
        }

        [Test]
        public void WhenSavingTheAggregate_ThenTheSaveResultIsAdded()
        {
            var result = this.Repository.Save(this.Aggregate);
            Assert.AreEqual(SaveResult.Added, result);
        }
    }
}
