// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUnitOfWork_T.cs" company="sgmunn">
//   (c) sgmunn 2012  
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Data
{
    using System;

    public interface IUnitOfWork<T> : IRepository<T>, IUnitOfWork
    {
    }
}