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

namespace MonoKit.Domain.Data
{
    using System;
    using System.Collections.Generic;
    using MonoKit.Data;

    public abstract class ReadModelBuilder : IReadModelBuilder
    {
        private readonly List<IReadModelChange> updatedReadModels;

        protected ReadModelBuilder()
        {
            this.updatedReadModels = new List<IReadModelChange>();
        }

        public IEnumerable<IReadModelChange> Handle(IList<IDataEvent> events)
        {
            this.updatedReadModels.Clear();

            // now, for each domain event, call a method that takes the event and call it
            foreach (var evt in events)
            {
                // todo: should we check for not handling the event or not.  read model builders probably don't need to 
                // handle *everthing*
                MethodExecutor.ExecuteMethodForParams(this, evt);
            }

            return this.updatedReadModels;
        }

        protected void NotifyReadModelChange(IReadModel readModel, bool deleted)
        {
            this.updatedReadModels.Add(new ReadModelChange(readModel, deleted));
        }

        protected void NotifyReadModelChange(IUniqueIdentity id, bool deleted)
        {
            this.updatedReadModels.Add(new ReadModelChange(id, deleted));
        }
    }
}

