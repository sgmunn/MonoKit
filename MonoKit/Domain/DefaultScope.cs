namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;
    using MonoKit.Data;

    public class DefaultScope : IUnitOfWorkScope
    {
        private readonly List<IUnitOfWork> scopedWork;
 
        public DefaultScope()
        {
            this.scopedWork = new List<IUnitOfWork>();
        }

        public void Add(IUnitOfWork uow)
        {
            this.scopedWork.Add(uow);
        }

        public void Commit()
        {
            foreach (var uow in this.scopedWork)
            {
                uow.Commit();
            }
        }

        public void Dispose()
        {
            foreach (var uow in this.scopedWork)
            {
                uow.Dispose();
            }
        }
    }
}

