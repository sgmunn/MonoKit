// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncRepository_T.cs" company="sgmunn">
//   (c) sgmunn 2012  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Data
{
    using System;
    using System.Collections.Generic;
//    using MonoKit.Tasks;
//
//    public class SyncRepository<T> : IRepository<T> where T: new()
//    {
//        private readonly IRepository<T> repository;
//        
//        public SyncRepository(IRepository<T> repository)
//        {
//            this.repository = repository;
//        }
//        
//        public void Dispose()
//        {
//            this.repository.Dispose();
//        }
//
//        public T New()
//        {
//            return SynchronousTask.GetSync(() => { 
//                return this.repository.New(); 
//            });
//        }
//
//        public T GetById(Guid id)
//        {
//            return SynchronousTask.GetSync(() => { 
//                return this.repository.GetById(id); 
//            });
//        }
//
//        public IList<T> GetAll()
//        {
//            return SynchronousTask.GetSync(() => { 
//                return this.repository.GetAll(); 
//            });
//        }
//
//        public SaveResult Save(T instance)
//        {
//            return SynchronousTask.GetSync(() => { 
//                return this.repository.Save(instance); 
//            });
//        }
//
//        public void Delete(T instance)
//        {
//            SynchronousTask.DoSync(() => { 
//                this.repository.Delete(instance); 
//            });
//        }
//
//        public void DeleteId(Guid id)
//        {
//            SynchronousTask.DoSync(() => { 
//                this.repository.DeleteId(id); 
//            });
//        }
//    }
}

