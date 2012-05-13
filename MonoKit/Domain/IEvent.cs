// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEvent.cs" company="sgmunn">
//   (c) sgmunn 2012  
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Domain
{
    using System;

    public interface IEvent
    {
        Guid AggregateId { get; }

        Guid EventId { get; }
        
        // todo: we can use the command id to match events to commands, or when we store unsent commands
        //Guid CommandId { get; }

        int Version { get; }
        
        DateTime Timestamp { get; }
    }
}

