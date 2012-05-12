// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUnitOfWorkScope.cs" company="sgmunn">
//   (c) sgmunn 2012  
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Data
{
    using System;

    public interface IUnitOfWorkScope : IUnitOfWork
    {
        void Add(IUnitOfWork uow);
    }
}