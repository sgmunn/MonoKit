// -----------------------------------------------------------------------
// <copyright file="ICommandExecutor_T.cs" company="sgmunn">
//   (c) sgmunn 2012
// </copyright>
// -----------------------------------------------------------------------

namespace MonoKit.Domain
{
    using System;

    public interface ICommandExecutor<T> where T : IAggregateRoot, new()
    {
        // todo: can we drop the expected version
        void Execute(ICommand command, int expectedVersion);
    }
}
