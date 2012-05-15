namespace MonoKit.Domain.Data.SQLite
{
    using System;
    using MonoKit.Domain.Data;
    using MonoKit.Data.SQLite;

    public class SQLStoredEvent : IEventStoreContract
    {
        [PrimaryKey]
        public Guid EventId { get; set; }
  
        [Indexed]
        public Guid AggregateId { get; set; }
  
        public int Version { get; set; }
  
        public string Event { get; set; }
    }
}
