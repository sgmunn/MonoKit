namespace MonoKit.Domain.Data
{
    using System;
    using System.Collections.Generic;

    public abstract class ReadModelBuilder : MethodExecutor, IReadModelBuilder
    {
        public void Handle(IList<IEvent> events)
        {
            // now, for each domain event, call a method that takes the event and call it
            foreach (var @event in events)
            {
                // todo: should we check for not handling the event or not.  read model builders probably don't need to handle *everthing*
                this.ExecuteMethodForParams(this, @event);
            }
        }
    }
}

