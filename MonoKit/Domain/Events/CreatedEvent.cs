namespace MonoKit.Domain.Events
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Name="Created", Namespace="http://sgmunn.com/MonoKit/Domain")]
    public class CreatedEvent : EventBase
    {
        [DataMember]
        public string Test { get; set; }
    }
}

