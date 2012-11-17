
using System;
using NUnit.Framework;
using MonoKit.Data;

namespace MonoKit.Core.UnitTests.Data
{
    [TestFixture]
    public class GivenAnEmptyUowRepository : GivenAUnitOfWorkRepository
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }
        
        [Test]
        public void WhenGettingAllItems_ThenNoItemsAreReturned()
        {
            var items = this.UnitOfWork.GetAll();
            Assert.AreEqual(0, items.Count);
        }
        
        [Test]
        public void WhenCreatingANewItem_ThenAnItemIsReturned()
        {
            var item = this.UnitOfWork.New();
            Assert.AreNotEqual(null, item);
        }
        
        [Test]
        public void WhenAddingAnItem_ThenOneItemIsAdded()
        {
            this.UnitOfWork.Save(this.UnitOfWork.New());
            var count = this.UnitOfWork.GetAll().Count;
            Assert.AreEqual(1, count);
        }
        
        [Test]
        public void WhenAddingAnItem_ThenTheUnderlyingRepositoryIsNotUpdated()
        {
            this.UnitOfWork.Save(this.UnitOfWork.New());
            var count = this.Repository.GetAll().Count;
            Assert.AreEqual(0, count);
        }
        
        [Test]
        public void WhenAddingAnItemAndCommitting_ThenTheUnderlyingRepositoryIsUpdated()
        {
            this.UnitOfWork.Save(this.UnitOfWork.New());
            this.UnitOfWork.Commit();

            var count = this.Repository.GetAll().Count;
            Assert.AreEqual(1, count);
        }

        [Test]
        public void WhenAddingAnItem_ThenTheSaveResultIsAdded()
        {
            var saveResult = this.UnitOfWork.Save(this.UnitOfWork.New());
            Assert.AreEqual(SaveResult.Added, saveResult);
        }

        [Test]
        public void WhenAddingAnItem_ThenTheItemIsReturned()
        {
            var item = this.UnitOfWork.New();
            this.UnitOfWork.Save(item);
            var result = this.UnitOfWork.GetById(item.Identity);
            Assert.AreEqual(item, result);
        }

        [Test]
        public void WhenAddingAnItem_ThenTheUnderlyingRepositoryDoesNotContainTheItem()
        {
            var item = this.UnitOfWork.New();
            this.UnitOfWork.Save(item);
            var result = this.Repository.GetById(item.Identity);
            Assert.AreEqual(null, result);
        }

        [Test]
        public void WhenAddingAnItemAndCommitting_ThenTheUnderlyingRepositoryDoesContainTheItem()
        {
            var item = this.UnitOfWork.New();
            this.UnitOfWork.Save(item);
            this.UnitOfWork.Commit();

            var result = this.Repository.GetById(item.Identity);
            Assert.AreEqual(item, result);
        }

        [Test]
        public void WhenAdding10Items_Then10AreReturned()
        {
            for (int i = 0;i<10;i++)
            {
                this.UnitOfWork.Save(this.UnitOfWork.New());
            }
            
            var count = this.UnitOfWork.GetAll().Count;
            Assert.AreEqual(10, count);
        }
    }
}
