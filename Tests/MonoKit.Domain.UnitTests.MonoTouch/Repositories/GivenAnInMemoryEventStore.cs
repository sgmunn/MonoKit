
using System;
using NUnit.Framework;

namespace MonoKit.Domain.UnitTests.Repositories
{
    [TestFixture]
    public class GivenAnInMemoryEventStore : GivenAnInMemoryEventStoreRepository
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void WhenAddingEventsForAnAggregate_ThenAllEventsAreReturned()
        {
            var id = Guid.NewGuid();
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id, Version = 1, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id, Version = 2, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id, Version = 3, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id, Version = 4, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id, Version = 5, });

            var events = this.EventStore.GetAllAggregateEvents(id);
            Assert.AreEqual(5, events.Count);
        }

        [Test]
        public void WhenAddingEventsForAnAggregate_ThenTheEventsAreReturnedInOrder()
        {
            var id = Guid.NewGuid();
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id, Version = 1, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id, Version = 2, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id, Version = 3, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id, Version = 4, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id, Version = 5, });
            
            var events = this.EventStore.GetAllAggregateEvents(id);
            Assert.AreEqual(1, events[0].Version);
            Assert.AreEqual(2, events[1].Version);
            Assert.AreEqual(3, events[2].Version);
            Assert.AreEqual(4, events[3].Version);
            Assert.AreEqual(5, events[4].Version);
        }

        [Test]
        public void WhenAddingEventsForAnAggregate_ThenTheEventsHaveTheCorrectId()
        {
            var id = Guid.NewGuid();
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id, Version = 1, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id, Version = 2, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id, Version = 3, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id, Version = 4, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id, Version = 5, });
            
            var events = this.EventStore.GetAllAggregateEvents(id);
            Assert.AreEqual(id, events[0].AggregateId);
            Assert.AreEqual(id, events[1].AggregateId);
            Assert.AreEqual(id, events[2].AggregateId);
            Assert.AreEqual(id, events[3].AggregateId);
            Assert.AreEqual(id, events[4].AggregateId);
        }

        [Test]
        public void WhenAddingEventsForMultipleAggregates_ThenTheEventsHaveTheCorrectId()
        {
            var id1 = Guid.NewGuid();
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id1, Version = 1, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id1, Version = 2, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id1, Version = 3, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id1, Version = 4, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id1, Version = 5, });
            
            var id2 = Guid.NewGuid();
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id2, Version = 1, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id2, Version = 2, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id2, Version = 3, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id2, Version = 4, });
            this.EventStore.Save(new TestSerializedEvent() {AggregateId = id2, Version = 5, });

            var events1 = this.EventStore.GetAllAggregateEvents(id1);
            Assert.AreEqual(id1, events1[0].AggregateId);
            Assert.AreEqual(id1, events1[1].AggregateId);
            Assert.AreEqual(id1, events1[2].AggregateId);
            Assert.AreEqual(id1, events1[3].AggregateId);
            Assert.AreEqual(id1, events1[4].AggregateId);

            var events2 = this.EventStore.GetAllAggregateEvents(id2);
            Assert.AreEqual(id2, events2[0].AggregateId);
            Assert.AreEqual(id2, events2[1].AggregateId);
            Assert.AreEqual(id2, events2[2].AggregateId);
            Assert.AreEqual(id2, events2[3].AggregateId);
            Assert.AreEqual(id2, events2[4].AggregateId);
        }
    }
}
