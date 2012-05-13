// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommand.cs" company="sgmunn">
//   (c) sgmunn 2012  
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Domain
{
    using System;

    public interface ICommand
    {
        Guid CommandId { get; }
        
        Guid AggregateId { get; }
    }
}