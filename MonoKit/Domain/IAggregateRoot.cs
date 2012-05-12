// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAggregateRoot.cs" company="sgmunn">
//   (c) sgmunn 2012  
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;

    public interface IAggregateRoot
    {
        Guid AggregateId { get; }
        
        int Version { get; }

        IEnumerable<IDomainEvent> UncommittedEvents { get; }

        void Commit();
    }
}

