
using System;
using NUnit.Framework;
using MonoKit.DataBinding;

namespace MonoKit.Core.UnitTests.InjectedProperties
{
    [TestFixture]
    public class GivenAnInjectablePropertyStore
    {
        private InjectedPropertyStore propertyStore;

        [SetUp]
        public void Setup()
        {
            this.propertyStore = new InjectedPropertyStore();
        }

        [Test]
        public void WhenGettingTheValueForAPropertyThatHasNotBeenSet_ThenTheValueIsNull()
        {
            var value = propertyStore.GetTestProperty();
            Assert.True(value == null);
        }

        [Test]
        public void Fail()
        {
           // Assert.False(true);
        }

        [Test]
       // [Ignore ("another time")]
        public void Ignore()
        {
          //  Assert.True(false);
        }

    }

    public static class TestProperty
    {
        public static InjectedProperty Test
        {
            get
            {
                return InjectedProperty.Register("Test", typeof(object), new InjectedPropertyMetadata(TestPropertyChanged));
            }
        }
        
        public static object GetTestProperty(this IPropertyInjection owner)
        {
            return owner.GetInjectedProperty(TestProperty.Test);
        }
        
        public static void SetTestProperty(this IPropertyInjection owner, object value)
        {
            owner.AddInjectedProperty(TestProperty.Test, value);
        }
        
        public static void TestPropertyChanged(object target, InjectedPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                // bind "target" to new value
                // target.Bind(e.NewValue, --template--)
                
                // how do we get the binding information for "target" in an easy manner
                // in xaml this would be defined in the view.  perhaps we do the same
                // by either using attributes on target or having the view register
                // perhaps the view can register define a way of binding
            }
        }
    }
    
}
