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
    using System.Runtime.Serialization;
    using MonoKit.Data;
    using MonoKit.Domain;

    [DataContract(Name="EventBase", Namespace="urn:SampleDomain")]
    public class EventBase : IAggregateEvent
    {
        public EventBase()
        {
        }

        public IUniqueIdentity Identity { get; set; }

        [DataMember]
        public Guid IdentityId 
        { 
            get
            {
                return this.Identity.Id;
            }

            set
            {
                this.Identity = new Identity(value);
            }
        }

        [DataMember]
        public Guid EventId { get; set; }

        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }      
    }

    [DataContract(Name="Event1", Namespace="urn:SampleDomain")]
    public class TestEvent1 : EventBase
    {
        [DataMember]
        public string Name { get; set; }
    }
        
    [DataContract(Name="Event2", Namespace="urn:SampleDomain")]
    public class TestEvent2 : EventBase
    {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public decimal Amount { get; set; }
    }
        
    [DataContract(Name="BalanceUpdated", Namespace="urn:SampleDomain")]
    public class BalanceUpdatedEvent : EventBase
    {
        [DataMember]
        public decimal Balance { get; set; }
    }
}
