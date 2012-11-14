
using System;
using MonoKit.DataBinding;

namespace MonoKit.Core.UnitTests.Bindings
{
    public abstract class GivenABinding
    {
        public Binding Binding { get; private set; }

        public virtual void SetUp()
        {
//            var assistant = new BindingAssistant((s) => ((SimpleSourceObject)s).Property1,
//                                                 (s,v) => { ((SimpleSourceObject)s).Property1 = (string)v; });
//
//            this.Binding = new Binding("Property1", assistant); 

            this.Binding = new Binding("Property1");
        }
    }
}
