namespace MonoKit.Domain
{
    using System;
    using MonoKit.Data;

    public interface IEventBus<T> where T : IAggregateRoot
    {
        // this needs the aggregate type with it as well
        // how about IEventBus of T ??
        void Publish(IDomainEvent domainEvent);
    }
    
    public class EventBus<T> : IEventBus<T> where T : IAggregateRoot
    {
        public void Publish(IDomainEvent domainEvent)
        {
            
        }
        
        
    }
    
    public class UnitOfWorkEventBus<T> : IEventBus<T>, IUnitOfWork where T : IAggregateRoot
    {
        public void Publish(IDomainEvent domainEvent)
        {
            
        }
        
        public void Dispose()
        {
            
        }
        
        public void Commit()
        {
            
        }
    }
    
    // unit of work event bus, add to scope and commit does the actual publish ?
    // if so, then how to hook up
}

