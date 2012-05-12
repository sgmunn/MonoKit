// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandExecutor_T.cs" company="sgmunn">
//   (c) sgmunn 2012  
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using MonoKit.Data;

    public class CommandExecutor<T> : MethodExecutor, ICommandExecutor<T> where T : class, IAggregateRoot, new()
    {
        private readonly IUnitOfWork<T> unitOfWork;

        private readonly Dictionary<Guid, int> versions;

        public CommandExecutor(IUnitOfWorkScope scope, IRepository<T> repository)
        {
            this.unitOfWork = new UnitOfWork<T>(repository);
            this.versions = new Dictionary<Guid, int>();

            scope.Add(this.unitOfWork);
        }

        public void Execute(IDomainCommand command, int expectedVersion)
        {
            var root = this.unitOfWork.GetById(command.AggregateId) ?? this.unitOfWork.New();

            // we need to compare the expected version with the "original" expected version
            var rootVersion = root.Version;
            if (this.versions.ContainsKey(root.AggregateId))
            {
                rootVersion = this.versions[root.AggregateId];
            }

            if (rootVersion != expectedVersion)
            {
                throw new InvalidOperationException(string.Format("Not Expected Version {0}, {1}", expectedVersion, root.Version));
            }

            this.Execute(root, command);

            this.unitOfWork.Save(root);

            if (!this.versions.ContainsKey(root.AggregateId))
            {
                this.versions[root.AggregateId] = expectedVersion;
            }
        }

        private void Execute(IAggregateRoot aggregate, IDomainCommand command)
        {
            try
            {
                if (!this.ExecuteMethodForSingleParam(aggregate, command))
                {
                    throw new MissingMethodException(string.Format("Aggregate {0} does not support a method that can be called with {1}", aggregate, command));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error executing command\n{0}", ex);
                throw;
            }
        }
    }
}

