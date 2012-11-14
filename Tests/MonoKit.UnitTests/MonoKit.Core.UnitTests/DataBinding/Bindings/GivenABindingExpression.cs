using System;
using MonoKit.DataBinding;

namespace MonoKit.Core.UnitTests.Bindings
{
    public abstract class GivenABindingExpression : GivenASourceAndTarget
    {
        public BindingExpression Expression { get; private set; }

        public override void SetUp()
        {
            base.SetUp();

            this.Expression = new BindingExpression(this.Target, "PropertyA", this.Source, this.Binding);
        }
    }
}
