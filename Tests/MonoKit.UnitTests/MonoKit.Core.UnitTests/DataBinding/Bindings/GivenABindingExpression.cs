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

//            var assistant = new BindingAssistant((s) => ((SimpleTargetObject)s).PropertyA,
//                                                 (s,v) => { ((SimpleTargetObject)s).PropertyA = (string)v; });
//
//            this.Expression = new BindingExpression(this.Target, "PropertyA", assistant, this.Source, this.Binding);

            this.Expression = new BindingExpression(this.Target, "PropertyA", this.Source, this.Binding);
        }
    }
}
