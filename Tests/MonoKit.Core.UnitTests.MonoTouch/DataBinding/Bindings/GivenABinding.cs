
using System;
using MonoKit.DataBinding;

namespace MonoKit.Core.UnitTests.Bindings
{
    public abstract class GivenABinding
    {
        public Binding Binding { get; protected set; }

        public virtual void SetUp()
        {
            this.Binding = new Binding("Property1");
        }
    }
}
