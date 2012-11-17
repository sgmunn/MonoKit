
using System;
using NUnit.Framework;

namespace MonoKit.Domain.UnitTests.Repositories.EventSourcing
{
    [TestFixture]
    public class GivenAnEmptyRepository : GivenAnEventSourcedAggregateRepository
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }
        
        [Test]
        public void WhenRetreivingAnAggregate_ThenNullIsReturned()
        {
            var id = Guid.NewGuid();
            var result = this.Repository.GetById(id);

            Assert.AreEqual(null, result);
        }
    }
}
