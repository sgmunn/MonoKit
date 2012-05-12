// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository_T.cs" company="sgmunn">
//   (c) sgmunn 2012  
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Data
{
    using System;
    using System.Collections.Generic;

    public interface IRepository<T> : IDisposable
    {
        T New();

        T GetById(object id);

        IEnumerable<T> GetAll(); 

        void Save(T instance);

        void Delete(T instance);

        void DeleteId(object id);
    }
}