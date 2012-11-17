
using System;
using NUnit.Framework;

namespace MonoKit.Core.UnitTests.InjectedProperties
{
    [TestFixture]
    public class GivenAnInjectablePropertyStoreWithAValueSet : GivenAnInjectableObject
    {
        private object someValue;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.someValue = new object();
            this.Injectable.SetTestProperty(this.someValue);
            TestProperty.OnChangedCalled = false;
        }
        
        [Test]
        public void WhenGettingTheValueForAPropertyThatHasBeenSet_ThenTheValueIsTheValueThatWasSet()
        {
            var value = this.Injectable.GetTestProperty();
            Assert.AreEqual(this.someValue, value);
        }
        
        [Test]
        public void WhenChangingTheValue_ThenTheOnChangedEventIsCalled()
        {
            this.Injectable.SetTestProperty(new object());
            Assert.AreEqual(true, TestProperty.OnChangedCalled);
        }
        
        [Test]
        public void WhenChangingTheValueToTheSameValue_ThenTheOnChangedEventIsNotCalled()
        {
            this.Injectable.SetTestProperty(this.someValue);
            Assert.AreEqual(false, TestProperty.OnChangedCalled);
        }
    }
}
