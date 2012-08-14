// --------------------------------------------------------------------------------------------------------------------
// <copyright file=".cs" company="sgmunn">
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
    using MonoKit.Data;

    public class CommandExecutor<T> : ICommandExecutor<T> where T : class, IAggregateRoot, new()
    {
        private readonly IRepository<T> repository;

        private readonly Dictionary<Identity, int> versions;

        private readonly IEventBus bus;

        public CommandExecutor(IRepository<T> repository, IEventBus bus)
        {
            this.repository = repository;
            this.bus = bus;
            this.versions = new Dictionary<Identity, int>();
        }

        public void Execute(ICommand command, int expectedVersion)
        {
            var root = this.repository.GetById(command.AggregateId) ?? this.repository.New();

            if (expectedVersion != 0)
            {
                var rootVersion = root.Version;
                if (this.versions.ContainsKey(root.AggregateId))
                {
                    rootVersion = this.versions[root.AggregateId];
                }
    
                if (rootVersion != expectedVersion)
                {
                    throw new InvalidOperationException(string.Format("Not Expected Version {0}, {1}", expectedVersion, root.Version));
                }
            }

            this.Execute(root, command);
   
            this.repository.Save(root);

            // we can assume that this is what is going to be saved, as the version will ensure consistency
            // thus, if the aggregate supports snapshot we can publish it
            // a bit hacky, will think on it.
            if (bus != null && root as ISnapshotSupport != null)
            {
                var snapshot = ((ISnapshotSupport)root).GetSnapshot() as ISnapshot;

                // todo: correct readmodel change for snapshot
                //bus.Publish(snapshot);
                bus.Publish(new ReadModelChangeEvent(snapshot, ReadModelChange.Changed));
            }

            if (expectedVersion != 0 && !this.versions.ContainsKey(root.AggregateId))
            {
                this.versions[root.AggregateId] = expectedVersion;
            }
        }

        private void Execute(IAggregateRoot aggregate, ICommand command)
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

