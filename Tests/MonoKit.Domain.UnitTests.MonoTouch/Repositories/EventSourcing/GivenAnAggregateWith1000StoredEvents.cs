
using System;
using NUnit.Framework;

namespace MonoKit.Domain.UnitTests.Repositories.EventSourcing
{
    [TestFixture]
    public class GivenAnAggregateWith1000StoredEvents : GivenAnEventSourcedAggregateRepository
    {
        public TestAggregateRoot Aggregate { get; private set; }
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            var id = Guid.NewGuid();
            this.Aggregate = new TestAggregateRoot();

            for (int i = 0; i < 1000; i++)
            {
                this.Aggregate.Execute(new TestCommand1{ AggregateId = id, Value1 = "1", Value2 = i, });
            }

            this.Repository.Save(this.Aggregate);
        }
        
        [Test]
        public void WhenLoadingTheAggregate_ThenThePerformanceIsOK()
        {
            var result = this.Repository.GetById(this.Aggregate.Identity);
            Assert.AreNotEqual(null, result);
            Assert.AreNotEqual(0, result.Version);
        }
    }
}
