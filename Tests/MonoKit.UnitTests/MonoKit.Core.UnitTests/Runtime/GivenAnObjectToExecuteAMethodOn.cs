using System;
using NUnit.Framework;
using MonoKit.Runtime;

namespace MonoKit.Runtime.UnitTests.Bindings
{
    [TestFixture]
    public class GivenAnObjectToExecuteAMethodOn
    {
        [Test]
        public void WhenExecutingAMethod_ThenTheMethodIsCalled()
        {
            var obj = new ExecutableObject();

            MethodExecutor.ExecuteMethod(obj, 123);

            Assert.AreEqual(123, obj.TheValue);
        }
    }
}
