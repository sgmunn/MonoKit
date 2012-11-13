using System;
using NUnit.Framework;

namespace MonoKit.Core.UnitTests.InjectedProperties
{
    [TestFixture]
    public class GivenAnInjectablePropertyStoreWithADisposableValue : GivenAnInjectablePropertyStore
    {
        private bool disposed;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.disposed = false;
            this.PropertyStore.SetTestProperty(new DisposableObject(() => { this.disposed = true; }));
        }

        [Test]
        public void WhenRemovingTheValue_ThenTheValueIsDisposed()
        {
            this.PropertyStore.RemoveInjectedProperty(TestProperty.Test);
            Assert.AreEqual(true, this.disposed);
        }

        [Test]
        public void WhenRemovingAllValues_ThenTheValueIsDisposed()
        {
            this.PropertyStore.RemoveAllInjectedProperties();
            Assert.AreEqual(true, this.disposed);
        }
    }
}
