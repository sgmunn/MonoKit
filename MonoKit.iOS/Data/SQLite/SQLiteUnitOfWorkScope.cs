namespace MonoKit.Data.SQLite
{
    using System;
    using System.Collections.Generic;
    
    public class SQLiteUnitOfWorkScope : IUnitOfWorkScope
    {
        private readonly SQLiteConnection connection;
        
        private readonly List<IUnitOfWork> scopedWork;

        private bool startedTransaction;
        
        public SQLiteUnitOfWorkScope(SQLiteConnection connection)
        {
            this.connection = connection;
            this.scopedWork = new List<IUnitOfWork>();

            if (!this.connection.IsInTransaction)
            {
                this.startedTransaction = true;
                this.connection.BeginTransaction();
            }
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

            if (this.startedTransaction)
            {
                this.connection.Commit();
                this.startedTransaction = false;
            }
        }

        public void Dispose()
        {
            foreach (var uow in this.scopedWork)
            {
                uow.Dispose();
            }
            
            if (this.startedTransaction)
            {
                this.connection.Rollback();
            }
        }
    }
}

