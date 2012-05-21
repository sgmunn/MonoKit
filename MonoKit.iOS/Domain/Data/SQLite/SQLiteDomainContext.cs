namespace MonoKit.Domain.Data.SQLite
{
    using System;
    using MonoKit.Domain;
    using MonoKit.Domain.Data;
    using MonoKit.Data;
    using MonoKit.Data.SQLite;

    public class SQLiteDomainContext : DomainContext
    {
        public SQLiteDomainContext(SQLiteConnection connection, IEventStoreRepository eventStore, IDomainEventBus eventBus) : base(eventStore, eventBus)
        {
            this.Connection = connection;
        }

        public SQLiteConnection Connection { get; private set; }

        public override IUnitOfWorkScope BeginUnitOfWork()
        {
            return new SQLiteUnitOfWorkScope(this.Connection);
        }
    }
}
