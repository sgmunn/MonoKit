
using System;
using NUnit.Framework;

namespace MonoKit.Domain.UnitTests.Repositories
{
    public class GivenAnInMemoryEventStoreRepository
    {
        public IEventStoreRepository EventStore { get; private set; }

        public virtual void SetUp()
        {
            this.EventStore = new InMemoryEventStoreRepository<TestSerializedEvent>();
        }
    }
}
