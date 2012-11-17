
using System;

namespace MonoKit.Domain.UnitTests.Repositories
{
    public class GivenAnEventSourcedAggregateRepository : GivenAnInMemoryEventStoreRepository
    {
        public IAggregateRepository<TestAggregateRoot> Repository { get; private set; }

        public MockBus Bus { get; private set; }

        public override void SetUp()
        {
            base.SetUp();

            this.Bus = new MockBus();
            this.Repository = new EventSourcedAggregateRepository<TestAggregateRoot>(
                TestEventSerializer.Instance, 
                this.EventStore, 
                new MockAggregateManifestRepository(), 
                this.Bus);
        }
    }
}
