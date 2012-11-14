
using System;
using NUnit.Framework;
using MonoKit.DataBinding;

namespace MonoKit.Core.UnitTests.Bindings
{
    [TestFixture]
    public class GivenABindingThatIsOneWayToSource : GivenABindingExpression
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.Binding.Mode = BindingMode.OneWayToSource;
        }
        
        [Test]
        public void WhenSettingTheTargetProperty_ThenTheSourceIsUpdated()
        {
            this.Target.PropertyA = Guid.NewGuid().ToString();
            Assert.AreEqual(this.Target.PropertyA, this.Source.Property1);
        }
        
        [Test]
        public void WhenSettingTheSourceProperty_ThenTheTargetIsNotUpdated()
        {
            this.Source.Property1 = Guid.NewGuid().ToString();
            Assert.AreEqual(null, this.Target.PropertyA);
        }
    }
}
