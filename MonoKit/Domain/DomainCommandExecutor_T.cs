// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainCommandExecutor_T.cs" company="sgmunn">
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
    using System.Linq;
    using MonoKit.Data;

    public class DomainCommandExecutor<T> : ICommandExecutor<T> where T : class, IAggregateRoot, new()
    {
        private readonly IDomainContext context;
        
        public DomainCommandExecutor(IDomainContext context)
        {
            this.context = context;
        }
        
        public void Execute(ICommand command)
        {
            this.Execute(new[] { command }, 0);
        }
        
        public void Execute(IEnumerable<ICommand> commands)
        {
            this.Execute(commands, 0);
        }
        
        public void Execute(ICommand command, int expectedVersion)
        {
            this.Execute(new[] { command }, expectedVersion);
        }

        public void Execute(IEnumerable<ICommand> commands, int expectedVersion)
        {
            var bus = new UnitOfWorkEventBus(this.context.EventBus);
            using (bus)
            {
                var scope = this.context.BeginUnitOfWork();
                using (scope)
                {
                    var uow = new UnitOfWorkRepository<T>(this.context.GetAggregateRepository<T>(bus));
                    // uow will be owned by the scope, so we don't need to dispose explicitly
                    scope.Add(uow);
                
                    var cmd = new CommandExecutor<T>(uow);

                    foreach (var command in commands.ToList())
                    {
                        cmd.Execute(command, 0);
                    }
                
                    Console.WriteLine("DomainCommandExecutor - commit scope");
                    scope.Commit();
                }

                Console.WriteLine("DomainCommandExecutor - commit bus");
                bus.Commit();
            }
        }
    }
}

