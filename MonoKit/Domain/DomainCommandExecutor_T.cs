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
    using MonoKit.Data;
    using MonoKit.Domain.Data;
    
    public class DomainCommandExecutor<T> : IDomainCommandExecutor<T> where T : class, IAggregateRoot, new()
    {
        private readonly IDomainContext context;
        
        public DomainCommandExecutor(IDomainContext context)
        {
            this.context = context;
        }
        
        // overload to execute multiple commands 
        public void Execute(ICommand command)
        {
            var bus = new UnitOfWorkEventBus(this.context.EventBus);
            using (bus)
            {
                var scope = this.context.BeginUnitOfWork();
                using (scope)
                {
                    // uow will be owned by the scope, so we don't need to dispose explicitly
                    var uow = new UnitOfWork<T>(scope, this.context.AggregateRepository<T>(bus));
                
                    var cmd = new CommandExecutor<T>(uow);
                    cmd.Execute(command, 0);
                
                    scope.Commit();
                }

                bus.Commit();
            }
        }
        
        // overload to execute multiple commands, actually this isn't so good, the bus isn't outside the scope
        // so we need post commit actions in the scope
        public void Execute(IUnitOfWorkScope scope, ICommand command)
        {
            // todo: post commit actions in the scope
            throw new NotImplementedException();

            var bus = new UnitOfWorkEventBus(this.context.EventBus);
            // uow will be owned by the scope, so we don't need to dispose explicitly
            var uow = new UnitOfWork<T>(scope, this.context.AggregateRepository<T>(bus));
        
            var cmd = new CommandExecutor<T>(uow);
            cmd.Execute(command, 0);
        }
        
        public void Execute(ICommand command, int expectedVersion)
        {
            this.Execute(command, 0);
        }
    }
}

