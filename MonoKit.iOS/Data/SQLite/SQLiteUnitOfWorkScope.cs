namespace MonoKit.Data.SQLite
{
    using System;
    using System.Collections.Generic;
    
    public class SQLiteUnitOfWorkScope : IUnitOfWorkScope
    {
        private readonly SQLiteConnection connection;
        
        private readonly List<IUnitOfWork> scopedWork;

        public SQLiteUnitOfWorkScope(SQLiteConnection connection)
        {
            this.connection = connection;
            this.scopedWork = new List<IUnitOfWork>();
        }

        public void Add(IUnitOfWork uow)
        {
            this.scopedWork.Add(uow);
        }

        public void Commit()
        {
            this.connection.BeginTransaction();
            try
            {
                foreach (var uow in this.scopedWork)
                {
                    uow.Commit();
                }

                this.connection.Commit();
            }
            catch
            {
                this.connection.Rollback();
                throw;
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

