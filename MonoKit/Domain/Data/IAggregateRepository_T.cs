namespace MonoKit.Domain.Data
{
    using System;
    using MonoKit.Data;

    public interface IAggregateRepository<T> : IRepository<T> where T : IAggregateRoot, new()
    {
    }
}

