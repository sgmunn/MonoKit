namespace MonoKit.Domain.Data
{
    using System;
    using System.Collections.Generic;
    using MonoKit.Data;
    
    public class Denormalizer<T> : MethodExecutor, IDenormalizer where T : new()
    {
        public readonly IRepository<T> repository;
        
        public Denormalizer(IRepository<T> repository)
        {
            this.repository = repository;
        }
        
        protected IRepository<T> Repository 
        {
            get
            {
                return this.repository;
            }
        }
        
        public void Handle(IList<IEvent> events)
        {
            // now, for each domain event, call a method that takes the event and call it
            foreach (var @event in events)
            {
                // todo: should we check for not handling the event or not.  denormalizers probably don't need to handle *everthing*
                this.ExecuteMethodForSingleParam(this, @event);
            }
        }
    }
}

