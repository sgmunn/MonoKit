
using System;

namespace MonoKit.Domain.UnitTests.Repositories
{
    public class MockAggregateManifestRepository : IAggregateManifestRepository
    {
        public void UpdateManifest(Guid aggregateId, int currentVersion, int newVersion)
        {
        }
    }
}
