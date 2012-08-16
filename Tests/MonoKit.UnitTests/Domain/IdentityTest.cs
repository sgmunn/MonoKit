
using MonoKit.Data;

namespace MonoKit.UnitTests.Domain
{
    using System;
    using NUnit.Framework;
    using MonoKit.Domain;

    
    public sealed class DomainId : Identity
    {
        public DomainId(Guid id)
            : base(id)
        {
        }

        public static DomainId NewId()
        {
            return new DomainId(Guid.NewGuid());
        }

        public static implicit operator DomainId(Guid id)
        {
            return new DomainId(id);
        }
    }


    [TestFixture]
    public class IdentityTest
    {
        private const string Guid1 = "008af708-d471-4fa3-b958-26ea1fb9c78d";
        private const string Guid2 = "9dd2c756-81de-4c04-8a95-559714d19ce9";

        [Test]
        public void EqualityBetweenIdentityAndGuid()
        {
            var identity = new Identity(new Guid(IdentityTest.Guid1));
            var guid = new Guid(IdentityTest.Guid1);

            Assert.AreEqual(identity, guid);
        }

        [Test]
        public void InequalityBetweenIdentityAndGuid()
        {
            var identity = new Identity(new Guid(IdentityTest.Guid1));
            var guid = new Guid(IdentityTest.Guid2);

            Assert.AreNotEqual(identity, guid);
        }

        [Test]
        public void CastToGuid()
        {
            var identity = new Identity(new Guid(IdentityTest.Guid1));
            Guid guid = identity;

            Assert.AreEqual(identity, guid);
        }

        [Test]
        public void EqualityBetweenIdentityAndNull()
        {
            var identity = new Identity(new Guid(IdentityTest.Guid1));

            Assert.AreNotEqual(identity, null);
        }

        [Test]
        public void EqualityBetweenIdentityAndObject()
        {
            var identity = new Identity(new Guid(IdentityTest.Guid1));

            Assert.AreNotEqual(identity, new Object());
        }

        [Test]
        public void CastOfSubclassToGuid()
        {
            var identity = DomainId.NewId();
            Guid guid = identity;

            Assert.AreEqual(identity, guid);
        }

        [Test]
        public void CastOfSubclassFromGuid()
        {
            var guid = Guid.NewGuid();
            Identity identity = (DomainId)guid;

            Assert.AreEqual(identity, guid);
        }
    }
}
