using System;
using NUnit.Framework;
using MonoKit.DataBinding;

namespace MonoKit.Core.UnitTests.Bindings
{
    [TestFixture]
    public class GivenAValueConverter : GivenABindingExpression
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.Binding = new Binding("Property2");
            this.Binding.Converter = BooleanToStringConverter.Instance;
            this.Expression = new WeakBindingExpression(this.Target, "PropertyA", this.Source, this.Binding);
        }
        
        [Test]
        public void WhenSettingTheTargetProperty_ThenTheTargetIsConvertedBack()
        {
            this.Target.PropertyA = "1";
            Assert.True(this.Source.Property2);
        }
        
        [Test]
        public void WhenSettingTheSourceProperty_ThenTheSourceIsConverted()
        {
            this.Source.Property2 = true;
            Assert.AreEqual("True", this.Target.PropertyA);
        }
    }
}
