
using System;

namespace MonoKit.Domain.UnitTests.Repositories
{
    public class GivenASnapshotSourcedAggregateRepository : GivenAnInMemorySnapshotRepository
    {
        public IAggregateRepository<TestAggregateRoot> Repository { get; private set; }

        public MockBus Bus { get; private set; }
        
        public override void SetUp()
        {
            base.SetUp();
            
            this.Bus = new MockBus();
            this.Repository = new SnapshotAggregateRepository<TestAggregateRoot>(
                this.SnapshotStore,
                new MockAggregateManifestRepository(), 
                this.Bus);
        }
    }
}
