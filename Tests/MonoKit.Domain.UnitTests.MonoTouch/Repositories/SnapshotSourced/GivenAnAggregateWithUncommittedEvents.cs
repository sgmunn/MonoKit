using System;
using NUnit.Framework;
using System.Linq;

namespace MonoKit.Domain.UnitTests.Repositories.SnapshotSourced
{
    [TestFixture]
    public class GivenAnAggregateWithUncommittedEvents : GivenASnapshotSourcedAggregateRepository
    {
        public TestAggregateRoot Aggregate { get; private set; }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var id = Guid.NewGuid();
            this.Aggregate = new TestAggregateRoot();
            this.Aggregate.Execute(new TestCommand1{ AggregateId = id, Value1 = "1", Value2 = 1, });
            this.Aggregate.Execute(new TestCommand1{ AggregateId = id, Value1 = "2", Value2 = 2, });
            this.Aggregate.Execute(new TestCommand1{ AggregateId = id, Value1 = "3", Value2 = 3, });
        }

        [Test]
        public void WhenSavingTheAggregate_ThenTheAggregateHasNoUncommittedEvents()
        {
            this.Repository.Save(this.Aggregate);
            Assert.AreEqual(0, this.Aggregate.UncommittedEvents.ToList().Count);
        }

        [Test]
        public void WhenSavingTheAggregate_ThenTheSnapshotStoreContainsTheSnapshot()
        {
            this.Repository.Save(this.Aggregate);

            var snapshot = this.SnapshotStore.GetById(this.Aggregate.Identity);
            Assert.AreNotEqual(null, snapshot);
            Assert.AreEqual(3, snapshot.Version);
        }

        [Test]
        public void WhenTheAggregateHasBeenSaved_ThenTheAggregateCanBeRetrievedAndHasTheCorrectVersion()
        {
            this.Repository.Save(this.Aggregate);

            var result = this.Repository.GetById(this.Aggregate.Identity);
            Assert.AreNotEqual(null, result);

            Assert.AreEqual(3, result.Version);
            Assert.AreEqual("3", result.Value1);
            Assert.AreEqual(3, result.Value2);
        }

        [Test]
        [ExpectedException(typeof(ConcurrencyException))]
        public void WhenSavingTheAggregateAndAnotherProcessHasSavedOtherEvents_ThenAConcurrencyExceptionIsThrown()
        {
            // other process
            this.SnapshotStore.Save(new TestSnapshot() {Identity = this.Aggregate.Identity, Version = 4, });

            // this process
            this.Repository.Save(this.Aggregate);
        }

        [Test]
        public void WhenSavingTheAggregate_ThenTheUncommittedEventsArePublished()
        {
            this.Repository.Save(this.Aggregate);

            Assert.AreEqual(3, this.Bus.PublishedEvents.Count);
        }

        [Test]
        public void WhenSavingTheAggregate_ThenTheUncommittedEventsArePublishedInOrder()
        {
            this.Repository.Save(this.Aggregate);
            
            Assert.AreEqual(1, ((TestEvent1)this.Bus.PublishedEvents[0].Event).Version);
            Assert.AreEqual(2, ((TestEvent1)this.Bus.PublishedEvents[1].Event).Version);
            Assert.AreEqual(3, ((TestEvent1)this.Bus.PublishedEvents[2].Event).Version);
        }
    }
}

