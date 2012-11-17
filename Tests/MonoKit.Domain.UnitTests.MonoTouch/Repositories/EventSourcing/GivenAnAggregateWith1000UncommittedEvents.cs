using System;
using NUnit.Framework;

namespace MonoKit.Domain.UnitTests.Repositories.EventSourcing
{
    [TestFixture]
    public class GivenAnAggregateWith1000UncommittedEvents : GivenAnEventSourcedAggregateRepository
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
        }
        
        [Test]
        public void WhenSavingTheAggregate_ThenThePerformanceIsOK()
        {
            this.Repository.Save(this.Aggregate);
            Assert.True(true);
        }
    }
}

