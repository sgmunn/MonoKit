// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandExecutor_T.cs" company="sgmunn">
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

namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using MonoKit.Data;

    public class CommandExecutor<T> : ICommandExecutor<T> where T : class, IAggregateRoot, new()
    {
        private readonly IRepository<T> repository;

        private readonly Dictionary<Guid, int> versions;

        public CommandExecutor(IRepository<T> repository)
        {
            this.repository = repository;
            this.versions = new Dictionary<Guid, int>();
        }

        public void Execute(IAggregateCommand command)
        {
            this.Execute(new [] { command }, 0);
        }

        public void Execute(IEnumerable<IAggregateCommand> commands)
        {
            this.Execute(commands, 0);
        }

        public void Execute(IAggregateCommand command, int expectedVersion)
        {
            this.Execute(new [] { command }, expectedVersion);
        }

        public void Execute(IEnumerable<IAggregateCommand> commands, int expectedVersion)
        {
            if (!commands.Any())
            {
                return;
            }

            var rootId = commands.First().AggregateId;

            if (commands.Any(x => x.AggregateId != rootId))
            {
                throw new InvalidOperationException("Can only execute commands for a single aggregate at a time");
            }

            var root = this.repository.GetById(rootId) ?? this.repository.New();

            if (expectedVersion != 0)
            {
                var rootVersion = root.Version;
                if (this.versions.ContainsKey(root.Identity))
                {
                    rootVersion = this.versions[root.Identity];
                }
    
                if (rootVersion != expectedVersion)
                {
                    throw new InvalidOperationException(string.Format("Not Expected Version {0}, {1}", expectedVersion, root.Version));
                }
            }

            foreach (var cmd in commands)
            {
                this.Execute(root, cmd);
            }

            this.repository.Save(root);

            if (expectedVersion != 0 && !this.versions.ContainsKey(root.Identity))
            {
                this.versions[root.Identity] = expectedVersion;
            }
        }

        private void Execute(IAggregateRoot aggregate, IAggregateCommand command)
        {
            try
            {
                if (!MethodExecutor.ExecuteMethodForSingleParam(aggregate, command))
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

