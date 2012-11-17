using System;

namespace MonoKit.Core.UnitTests.Bindings
{
    public abstract class GivenASourceAndTarget : GivenABinding
    {
        public SimpleTargetObject Target { get; private set; }
        public SimpleSourceObject Source { get; private set; }
        
        public override void SetUp()
        {
            base.SetUp();
            this.Source = new SimpleSourceObject();
            this.Target = new SimpleTargetObject();
        }
    }
}
