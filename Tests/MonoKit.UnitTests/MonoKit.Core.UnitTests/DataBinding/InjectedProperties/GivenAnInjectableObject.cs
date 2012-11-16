
using System;
using MonoKit.DataBinding;

namespace MonoKit.Core.UnitTests.InjectedProperties
{
    public abstract class GivenAnInjectableObject
    {
        public virtual void Setup()
        {
            this.Injectable = new InjectableObject();
        }

        public IPropertyInjection Injectable{ get; private set; }
    }

    public class InjectableObject : IPropertyInjection
    {
        public InjectableObject()
        {
            this.InjectedProperties = new InjectedPropertyStore(this);
        }

        public IInjectedPropertyStore InjectedProperties
        {
            get; 
            private set;
        } 
    }
}
