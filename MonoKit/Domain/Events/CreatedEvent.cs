namespace MonoKit.Domain.Events
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Name="Created", Namespace=DomainNamespace.Namespace)]
    public class CreatedEvent : EventBase
    {
    }
}

