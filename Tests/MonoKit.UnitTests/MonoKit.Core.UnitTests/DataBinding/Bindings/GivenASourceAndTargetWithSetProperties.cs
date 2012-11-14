using System;
using NUnit.Framework;
using MonoKit.DataBinding;

namespace MonoKit.Core.UnitTests.Bindings
{
    [TestFixture]
    public class GivenASourceAndTargetWithSetProperties : GivenASourceAndTarget
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.Source.Property1 = Guid.NewGuid().ToString();
            this.Target.PropertyA = Guid.NewGuid().ToString();
        }

        [Test]
        public void WhenBindingTheSourceAndTarget_ThenTheTargetPropertyIsUpdated()
        {
            new BindingExpression(this.Target, "PropertyA", this.Source, this.Binding);
            Assert.AreEqual(this.Source.Property1, this.Target.PropertyA);
        }
    }
}
