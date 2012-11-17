
using System;
using System.Collections.Generic;

namespace MonoKit.Domain.UnitTests.Repositories
{
    public class MockBus : INotificationEventBus
    {
        public MockBus()
        {
            this.PublishedEvents = new List<INotificationEvent>();
        }

        public List<INotificationEvent> PublishedEvents { get; private set; }

        public void Publish(INotificationEvent evt)
        {
            this.PublishedEvents.Add(evt);
        }   

        public IDisposable Subscribe(IObserver<INotificationEvent> observer)
        {
            return null;
        }       
    }
}
