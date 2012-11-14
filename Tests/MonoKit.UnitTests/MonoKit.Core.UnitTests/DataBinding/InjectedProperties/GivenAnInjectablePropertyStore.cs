
using System;
using MonoKit.DataBinding;

namespace MonoKit.Core.UnitTests.InjectedProperties
{
    public abstract class GivenAnInjectablePropertyStore
    {
        public virtual void Setup()
        {
            this.PropertyStore = new InjectedPropertyStore(this);
        }

        public InjectedPropertyStore PropertyStore { get; private set; }
    }
}
