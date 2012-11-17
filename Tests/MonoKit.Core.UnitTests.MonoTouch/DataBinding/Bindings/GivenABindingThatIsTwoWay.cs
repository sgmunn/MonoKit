using System;
using NUnit.Framework;

namespace MonoKit.Core.UnitTests.Bindings
{
    [TestFixture]
    public class GivenABindingThatIsTwoWay : GivenABindingExpression
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void WhenSettingTheTargetProperty_ThenTheSourceIsUpdated()
        {
            this.Target.PropertyA = Guid.NewGuid().ToString();
            Assert.AreEqual(this.Target.PropertyA, this.Source.Property1);
        }

        [Test]
        public void WhenSettingTheSourceProperty_ThenTheTargetIsUpdated()
        {
            this.Source.Property1 = Guid.NewGuid().ToString();
            Assert.AreEqual(this.Source.Property1, this.Target.PropertyA);
        }
    }
}
