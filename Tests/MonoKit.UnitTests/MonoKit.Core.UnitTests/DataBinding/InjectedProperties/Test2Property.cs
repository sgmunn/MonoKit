using System;
using MonoKit.DataBinding;

namespace MonoKit.Core.UnitTests.InjectedProperties
{
    public static class Test2Property
    {
        public static InjectedProperty Test
        {
            get
            {
                return InjectedProperty.Register("Test2", typeof(string), new InjectedPropertyMetadata("DefaultValue", TestPropertyChanged));
            }
        }
        
        public static string GetTest2Property(this IPropertyInjection owner)
        {
            return (string)owner.GetInjectedProperty(Test2Property.Test);
        }

        public static void SetTest2Property(this IPropertyInjection owner, string value)
        {
            owner.SetInjectedProperty(Test2Property.Test, value);
        }
        
        public static void TestPropertyChanged(object target, InjectedPropertyChangedEventArgs e)
        {
        }
    }
}
