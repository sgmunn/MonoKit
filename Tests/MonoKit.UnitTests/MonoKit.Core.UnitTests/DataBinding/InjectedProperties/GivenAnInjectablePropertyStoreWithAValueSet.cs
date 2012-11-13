
using System;
using NUnit.Framework;

namespace MonoKit.Core.UnitTests.InjectedProperties
{
    [TestFixture]
    public class GivenAnInjectablePropertyStoreWithAValueSet : GivenAnInjectablePropertyStore
    {
        private object someValue;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.someValue = new object();
            this.PropertyStore.SetTestProperty(this.someValue);
            TestProperty.OnChangedCalled = false;
        }
        
        [Test]
        public void WhenGettingTheValueForAPropertyThatHasBeenSet_ThenTheValueIsTheValueThatWasSet()
        {
            var value = this.PropertyStore.GetTestProperty();
            Assert.AreEqual(this.someValue, value);
        }
        
        [Test]
        public void WhenChangingTheValue_ThenTheOnChangedEventIsCalled()
        {
            this.PropertyStore.SetTestProperty(new object());
            Assert.AreEqual(true, TestProperty.OnChangedCalled);
        }
        
        [Test]
        public void WhenChangingTheValueToTheSameValue_ThenTheOnChangedEventIsNotCalled()
        {
            this.PropertyStore.SetTestProperty(this.someValue);
            Assert.AreEqual(false, TestProperty.OnChangedCalled);
        }
    }
}
