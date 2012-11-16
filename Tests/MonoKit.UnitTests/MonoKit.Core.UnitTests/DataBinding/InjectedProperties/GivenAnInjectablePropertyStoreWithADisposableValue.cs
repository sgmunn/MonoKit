using System;
using NUnit.Framework;

namespace MonoKit.Core.UnitTests.InjectedProperties
{
    [TestFixture]
    public class GivenAnInjectablePropertyStoreWithADisposableValue : GivenAnInjectableObject
    {
        private bool disposed;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.disposed = false;
            this.Injectable.SetTestProperty(new DisposableObject(() => { this.disposed = true; }));
        }

        [Test]
        public void WhenRemovingTheValue_ThenTheValueIsDisposed()
        {
            this.Injectable.InjectedProperties.RemoveInjectedProperty(TestProperty.Test);
            Assert.AreEqual(true, this.disposed);
        }

        [Test]
        public void WhenRemovingAllValues_ThenTheValueIsDisposed()
        {
            this.Injectable.InjectedProperties.RemoveAllInjectedProperties();
            Assert.AreEqual(true, this.disposed);
        }
    }
}
