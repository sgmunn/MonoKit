namespace MonoKit.Domain
{
    using System;
    using MonoKit.Data;

    public interface IDomainCommandExecutor<T> : ICommandExecutor<T> where T: class, IAggregateRoot, new()
    {
        void Execute(ICommand command);
        //todo: void Execute(IEnumerable<ICommand> commands);
        void Execute(IUnitOfWorkScope scope, ICommand command);
        //todo: void Execute(IUnitOfWorkScope scope, IEnumerable<ICommand> commands);
    }
}

