// --------------------------------------------------------------------------------------------------------------------
// <copyright file=".cs" company="sgmunn">
//   (c) sgmunn 2012  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Domain
{
    using System;
    using System.Runtime.Serialization;
    using MonoKit.Data;

    // todo: investigate having this with abstract properties so that serialization could be shifted to application domains
    // todo: kill off identity type from event base as we have it with Identity

//    [DataContract(Name="EventBase", Namespace=DomainNamespace.Namespace)]
    public abstract class EventBase : IAggregateEvent
    {
        public EventBase()
        {
            this.EventId = Guid.NewGuid();
            this.Timestamp = DateTime.UtcNow;
        }

        // todo: check serialization of Identity
//        [DataMember]
        public abstract IUniqueIdentity Identity { get; set; }

//        [DataMember]
        public abstract Guid EventId { get; set; }

//        [DataMember]
        public abstract int Version { get; set; }

//        [DataMember]
        public abstract DateTime Timestamp { get; set; }      
    }
}

