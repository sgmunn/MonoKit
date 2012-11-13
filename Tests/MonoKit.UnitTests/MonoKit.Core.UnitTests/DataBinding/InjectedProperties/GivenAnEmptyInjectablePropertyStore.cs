
using System;
using NUnit.Framework;

namespace MonoKit.Core.UnitTests.InjectedProperties
{
    [TestFixture]
    public class GivenAnEmptyInjectablePropertyStore : GivenAnInjectablePropertyStore
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
        }

        [Test]
        public void WhenGettingTheValueForAPropertyThatHasNotBeenSet_ThenTheValueIsNull()
        {
            var value = this.PropertyStore.GetTestProperty();
            Assert.AreEqual(null, value);
        }

        [Test]
        public void WhenGettingTheValueForAPropertyWithADefaultValueThatHasNotBeenSet_ThenTheValueIsTheDefaultValue()
        {
            var value = this.PropertyStore.GetTest2Property();
            Assert.AreEqual(Test2Property.Test.Metadata.DefaultValue, value);
        }
    }
}
