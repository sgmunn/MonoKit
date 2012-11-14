using System;
using NUnit.Framework;

namespace MonoKit.Core.UnitTests.Bindings
{
    [TestFixture]
    public class BindingPerformance : GivenABindingExpression
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }
        
        [Test]
        public void WhenSettingTheTargetProperty1000Times_ThenTheSourceIsUpdated()
        {
            for (int i= 0;i<1000;i++)
            {
                this.Target.PropertyA = Guid.NewGuid().ToString();
            }
            Assert.AreEqual(this.Target.PropertyA, this.Source.Property1);
        }
    }
}
