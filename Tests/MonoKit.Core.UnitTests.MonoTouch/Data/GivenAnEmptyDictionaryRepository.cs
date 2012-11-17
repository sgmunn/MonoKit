
using System;
using NUnit.Framework;
using MonoKit.Data;

namespace MonoKit.Core.UnitTests.Data
{
    [TestFixture]
    public class GivenAnEmptyDictionaryRepository : GivenADictionaryRepository
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void WhenGettingAllItems_ThenNoItemsAreReturned()
        {
            var items = this.Repository.GetAll();
            Assert.AreEqual(0, items.Count);
        }

        [Test]
        public void WhenCreatingANewItem_ThenAnItemIsReturned()
        {
            var item = this.Repository.New();
            Assert.AreNotEqual(null, item);
        }

        [Test]
        public void WhenAddingAnItem_ThenOneItemIsAdded()
        {
            this.Repository.Save(this.Repository.New());
            var count = this.Repository.GetAll().Count;
            Assert.AreEqual(1, count);
        }

        [Test]
        public void WhenAddingAnItem_ThenTheSaveResultIsAdded()
        {
            var saveResult = this.Repository.Save(this.Repository.New());
            Assert.AreEqual(SaveResult.Added, saveResult);
        }

        [Test]
        public void WhenAddingAnItem_ThenTheItemIsReturned()
        {
            var item = this.Repository.New();
            this.Repository.Save(item);
            var result = this.Repository.GetById(item.Identity);
            Assert.AreEqual(item, result);
        }

        [Test]
        public void WhenAdding10Items_Then10AreReturned()
        {
            for (int i = 0;i<10;i++)
            {
                this.Repository.Save(this.Repository.New());
            }

            var count = this.Repository.GetAll().Count;
            Assert.AreEqual(10, count);
        }
    }
}
