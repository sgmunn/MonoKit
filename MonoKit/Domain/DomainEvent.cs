namespace MonoKit.Domain
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Name="EventBase", Namespace=DomainNamespace.Namespace)]
    public abstract class DomainEvent : IDomainEvent
    {
        public DomainEvent()
        {
            this.EventId = Guid.NewGuid();
            this.Timestamp = DateTime.UtcNow;
        }

        [DataMember]
        public Guid EventId { get; set; }

        [DataMember]
        public Guid AggregateId { get; set; }

        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }      
    }
}

