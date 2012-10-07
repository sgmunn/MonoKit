// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlUnitOfWorkScope.cs" company="sgmunn">
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

namespace MonoKit.Data.SQLite
{
    using System;
    using System.Collections.Generic;
    
    public class SqlUnitOfWorkScope : IUnitOfWorkScope
    {
        private readonly SQLiteConnection connection;
        
        private readonly List<IUnitOfWork> scopedWork;

        public SqlUnitOfWorkScope(SQLiteConnection connection)
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
            lock(this.connection)
            {
                this.connection.BeginTransaction();
                try
                {
                    Console.WriteLine("SqlUnitOfWork.Commit -- added uow's");
                    foreach (var uow in this.scopedWork)
                    {
                        uow.Commit();
                    }

                    Console.WriteLine("SqlUnitOfWork.Commit -- db connection");
                    this.connection.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SqlUnitOfWork Exception \n{0}", ex);
                    this.connection.Rollback();
                    throw;
                }
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

