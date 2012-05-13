namespace MonoKit.Domain
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Name="CommandBase", Namespace=DomainNamespace.Namespace)]
    public abstract class CommandBase : ICommand
    {
        public CommandBase()
        {
            this.CommandId = Guid.NewGuid();
        }
        
        [DataMember]
        public Guid CommandId { get; set; }
        
        [DataMember]
        public Guid AggregateId { get; set; }
    }
}

