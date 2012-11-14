
using System;
using NUnit.Framework;

namespace MonoKit.Core.UnitTests.Bindings
{
    [TestFixture]
    public class GivenADisposedExpression : GivenABindingExpression
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.Expression.Dispose();
        }

        [Test]
        public void WhenSettingTheTargetProperty_ThenTheSourceIsNotUpdated()
        {
            this.Target.PropertyA = Guid.NewGuid().ToString();
            Assert.AreEqual(null, this.Source.Property1);
        }
        
        [Test]
        public void WhenSettingTheSourceProperty_ThenTheTargetIsNotUpdated()
        {
            this.Source.Property1 = Guid.NewGuid().ToString();
            Assert.AreEqual(null, this.Target.PropertyA);
        }
    }
}
