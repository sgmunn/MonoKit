
using System;
using NUnit.Framework;

namespace MonoKit.Core.UnitTests.Reactive
{
    [TestFixture]
    public class Given
    {
        [Test]
        public void Pass()
        {
            Assert.True(true);
        }

        [Test]
        public void Fail()
        {
            Assert.False(true);
        }

        [Test]
        [Ignore ("another time")]
        public void Ignore()
        {
            Assert.True(false);
        }
    }
}
