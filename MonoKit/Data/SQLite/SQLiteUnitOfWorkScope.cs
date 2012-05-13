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
            this.connection.BeginTransaction();
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
            
            this.connection.Commit();
        }

        public void Dispose()
        {
            foreach (var uow in this.scopedWork)
            {
                uow.Dispose();
            }
            
            this.connection.Rollback();
        }
    }
}

