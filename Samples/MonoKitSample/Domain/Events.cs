//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="Events.cs" company="sgmunn">
//    (c) sgmunn 2012  
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//    documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//    to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//    the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//    THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//    CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//    IN THE SOFTWARE.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
//

namespace MonoKitSample
{
    using System;
    using MonoKit.Data;
    using MonoKit.Domain;
    using System.Runtime.Serialization;

    // todo: test the serialization of this class
    [DataContract(Name = "DomainEventBase", Namespace = "xyz")]
    public class DomainEventBase : EventBase
    {
        [DataMember]
        public override IUniqueIdentity Identity { get; set; }

        [DataMember]
        public override Guid EventId { get; set; }

        [DataMember]
        public override int Version { get; set; }

        [DataMember]
        public override DateTime Timestamp { get; set; }
    }
        
    [DataContract(Name="Event1", Namespace="http://sgmunn.com/2012/Sample/Domain")]
    public class TestEvent1 : DomainEventBase
    {
        [DataMember]
        public string Name { get; set; }
    }
        
    [DataContract(Name="Event2", Namespace="http://sgmunn.com/2012/Sample/Domain")]
    public class TestEvent2 : DomainEventBase
    {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public decimal Amount { get; set; }
    }
        
    [DataContract(Name="BalanceUpdated", Namespace="http://sgmunn.com/2012/Sample/Domain")]
    public class BalanceUpdatedEvent : DomainEventBase
    {
        [DataMember]
        public decimal Balance { get; set; }
    }
}
