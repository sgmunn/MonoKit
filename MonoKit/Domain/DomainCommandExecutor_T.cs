namespace MonoKit.Domain
{
    using System;
    using MonoKit.Data;
    using MonoKit.Domain.Data;
    
    public class DomainCommandExecutor<T> : IDomainCommandExecutor<T> where T : class, IAggregateRoot, new()
    {
        private readonly IDomainContext context;
        
        private readonly IEventBus<T> bus;
        
        public DomainCommandExecutor(IDomainContext context)
        {
            this.context = context;
            this.bus = new EventBus<T>(context.EventBus);
        }
        
        public IAggregateRepository<T> GetOrResolveAggregateRepo()
        {
            // we don't really need to do a resolve, the built-in aggregate repository should be just fine once I support
            // snapshot state, 
            return new AggregateRepository<T>(this.context.Serializer, this.context.EventStore, this.bus);
        }
        
        public IUnitOfWork<T> NewUnitOfWork(IUnitOfWorkScope scope)
        {
            return new UnitOfWork<T>(scope, this.GetOrResolveAggregateRepo());
        }
        
        // overload to execute multiple commands 
        public void Execute(ICommand command)
        {
            var scope = this.context.GetScope();
            using (scope)
            {
                // uow will be owned by the scope, so we don't need to dispose explicitly
                var uow = this.NewUnitOfWork(scope);
            
                var cmd = new CommandExecutor<T>(uow);
                cmd.Execute(command, 0);
            
                scope.Commit();
            }
        }
        
        // overload to execute multiple commands
        public void Execute(IUnitOfWorkScope scope, ICommand command)
        {
            var uow = this.NewUnitOfWork(scope);
        
            using (uow)
            {
                var cmd = new CommandExecutor<T>(uow);
                cmd.Execute(command, 0);
            }
        }
        
        public void Execute(ICommand command, int expectedVersion)
        {
            this.Execute(command, 0);
        }
    }
}

