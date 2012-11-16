
using System;
using NUnit.Framework;
using MonoKit.Data;

namespace MonoKit.Core.UnitTests.Data
{
    [TestFixture]
    public class GivenADictionaryRepositoryWithItems : GivenADictionaryRepository
    {
        public IdTest Item1 { get; private set; }
        public IdTest Item2 { get; private set; }
        public IdTest Item3 { get; private set; }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.Item1 = this.Repository.New();
            this.Item2 = this.Repository.New();
            this.Item3 = this.Repository.New();

            this.Item1.Value = 1;
            this.Item2.Value = 2;
            this.Item3.Value = 3;

            this.Repository.Save(this.Item1);
            this.Repository.Save(this.Item2);
            this.Repository.Save(this.Item3);
        }
        
        [Test]
        public void WhenUpdatingAnItem_ThenTheItemSaveResultIsUpdated()
        {
            var item = new IdTest() { Identity = this.Item1.Identity, Value = 123, };
            var result = this.Repository.Save(item);
            Assert.AreEqual(SaveResult.Updated, result);
        }
        
        [Test]
        public void WhenUpdatingAnItem_ThenTheItemIsUpdated()
        {
            var item = new IdTest() { Identity = this.Item1.Identity, Value = 123, };
            this.Repository.Save(item);
            var updated = this.Repository.GetById(item.Identity);
            Assert.AreEqual(123, updated.Value);
        }
        
        [Test]
        public void WhenDeletingAnItem_ThenTheItemIsRemoved()
        {
            this.Repository.Delete(this.Item2);
            var count = this.Repository.GetAll().Count;
            Assert.AreEqual(2, count);
        }
        
        [Test]
        public void WhenDeletingAnItemById_ThenTheItemIsRemoved()
        {
            this.Repository.DeleteId(this.Item2.Identity);
            var count = this.Repository.GetAll().Count;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void WhenDeletingAnItem_ThenTheItemIsNoLongerInTheRepository()
        {
            this.Repository.Delete(this.Item2);
            var deleted = this.Repository.GetById(this.Item2.Identity);
            Assert.AreEqual(null, deleted);
        }
        
        [Test]
        public void WhenDeletingAllItems_ThenThereAreNoItems()
        {
            this.Repository.DeleteId(this.Item1.Identity);
            this.Repository.DeleteId(this.Item2.Identity);
            this.Repository.DeleteId(this.Item3.Identity);
            var count = this.Repository.GetAll().Count;
            Assert.AreEqual(0, count);
        }
    }
}
