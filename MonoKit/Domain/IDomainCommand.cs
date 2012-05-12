// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDomainCommand.cs" company="sgmunn">
//   (c) sgmunn 2012  
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Domain
{
    using System;

    public interface IDomainCommand
    {
        Guid CommandId { get; }
        
        Guid AggregateId { get; }
    }
}