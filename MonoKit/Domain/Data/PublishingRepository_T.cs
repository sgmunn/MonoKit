//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="PublishingRepository_T.cs" company="sgmunn">
//    (c) sgmunn 2012  
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//    documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//    to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//    the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//    THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//    CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//    IN THE SOFTWARE.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
//

namespace MonoKit.Domain.Data
{
    using System;
    using MonoKit.Data;

    public class PublishingRepository<T> : RepositoryUnitOfWork<T> where T : IIdentity 
    {
        private readonly IEventBus bus; 

        public PublishingRepository(IUnitOfWorkScope scope, IRepository<T> repository, IEventBus bus) : base(scope, repository)
        {
            this.bus = bus;
        }

        public override void Save(T instance)
        {
            base.Save(instance);
            this.bus.Publish(new ReadModelChange(instance));
        }

        public override void Delete(T instance)
        {
            base.Delete(instance);
            this.bus.Publish(new ReadModelChange(instance.Identity));
        }

        public override void DeleteId(object id)
        {
            base.DeleteId(id);
            this.bus.Publish(new ReadModelChange((IUniqueIdentity)id));
        }
    }
}
