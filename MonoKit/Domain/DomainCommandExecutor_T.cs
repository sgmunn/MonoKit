namespace MonoKit.Domain
{
    using System;
    using MonoKit.Data;
    using MonoKit.Domain.Data;
    
    public class DomainCommandExecutor<T> : IDomainCommandExecutor<T> where T : class, IAggregateRoot, new()
    {
        private readonly IDomainContext context;
        
        public DomainCommandExecutor(IDomainContext context)
        {
            this.context = context;
        }
        
        // overload to execute multiple commands 
        public void Execute(ICommand command)
        {
            var scope = this.context.GetScope();
            using (scope)
            {
                // uow will be owned by the scope, so we don't need to dispose explicitly
                var uow = new UnitOfWork<T>(scope, this.context.AggregateRepository<T>());
            
                var cmd = new CommandExecutor<T>(uow);
                cmd.Execute(command, 0);
            
                scope.Commit();
            }
        }
        
        // overload to execute multiple commands
        public void Execute(IUnitOfWorkScope scope, ICommand command)
        {
            // uow will be owned by the scope, so we don't need to dispose explicitly
            var uow = new UnitOfWork<T>(scope, this.context.AggregateRepository<T>());
        
            var cmd = new CommandExecutor<T>(uow);
            cmd.Execute(command, 0);
        }
        
        public void Execute(ICommand command, int expectedVersion)
        {
            this.Execute(command, 0);
        }
    }
}

