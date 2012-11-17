
using System;
using NUnit.Framework;
using MonoKit.Data;

namespace MonoKit.Core.UnitTests.Data
{
    public class GivenADictionaryRepository
    {
        public IRepository<IdTest> Repository { get; private set; }

        public virtual void SetUp()
        {
            this.Repository = new IdDictionaryRepository<IdTest>();
        }
    }

    public class IdTest : IId
    {
        public IdTest()
        {
            this.Identity = Guid.NewGuid();
        }

        public Guid Identity { get; set; }

        public int Value { get; set; }
    }
}
