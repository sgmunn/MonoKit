using System;
using MonoKit.DataBinding;

namespace MonoKit.Core.UnitTests.InjectedProperties
{
    public static class TestProperty
    {
        public static bool OnChangedCalled;

        public static InjectedProperty Test
        {
            get
            {
                return InjectedProperty.Register("Test", typeof(object), new InjectedPropertyMetadata(TestPropertyChanged));
            }
        }
        
        public static object GetTestProperty(this IPropertyInjection owner)
        {
            return owner.InjectedProperties.GetInjectedProperty(TestProperty.Test);
        }
        
        public static void SetTestProperty(this IPropertyInjection owner, object value)
        {
            owner.InjectedProperties.SetInjectedProperty(TestProperty.Test, value);
        }
        
        public static void TestPropertyChanged(object target, InjectedPropertyChangedEventArgs e)
        {
            OnChangedCalled = true;
        }
    }
}
