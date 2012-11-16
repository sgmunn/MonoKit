using System;
using MonoKit.DataBinding;

namespace MonoKit.Core.UnitTests.Bindings
{
    public abstract class GivenABindingExpression : GivenASourceAndTarget
    {
        public IBindingExpression Expression { get; protected set; }

        public override void SetUp()
        {
            base.SetUp();

            this.Expression = this.GetExpression();
        }

        protected virtual IBindingExpression GetExpression()
        {
            return new WeakBindingExpression(this.Target, "PropertyA", this.Source, this.Binding);
        }
    }
}
