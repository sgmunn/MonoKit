// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUnitOfWork.cs" company="sgmunn">
//   (c) sgmunn 2012  
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Data
{
    using System;

    public interface IUnitOfWork : IDisposable
    {
        void Commit();
    }
}