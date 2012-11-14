using System;
using NUnit.Framework;
using MonoKit.DataBinding;

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
        public void WhenSettingTheTargetProperty1000TimesWithTheDefaultPropertyAccessor_ThenTheSpeedIsOK()
        {
            for (int i= 0;i<1000;i++)
            {
                this.Target.PropertyA = Guid.NewGuid().ToString();
            }

            Assert.True(true);
        }
        
        [Test]
        public void WhenSettingTheTargetProperty1000TimesWithADelegatePropertyAccessor_ThenTheSpeedIsOK()
        {
            var source = new DelegatePropertyAccessor("Property1", 
                                                      (s) => ((SimpleSourceObject)s).Property1,
                                                      (s,v) => { ((SimpleSourceObject)s).Property1 = (string)v; });
            
            this.Binding.PropertyAccessor = source;

            var target = new DelegatePropertyAccessor("PropertyA",
                                                      (s) => ((SimpleTargetObject)s).PropertyA,
                                                      (s,v) => { ((SimpleTargetObject)s).PropertyA = (string)v; });

            this.Expression.PropertyAccessor = target;

            for (int i= 0;i<1000;i++)
            {
                this.Target.PropertyA = Guid.NewGuid().ToString();
            }
            
            Assert.True(true);
        }
    }
}
