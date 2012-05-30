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

    public abstract class ReadModelBuilder : MethodExecutor, IReadModelBuilder
    {
        private readonly List<IReadModel> updatedReadModels;

        protected ReadModelBuilder()
        {
            this.updatedReadModels = new List<IReadModel>();
        }

        public IEnumerable<IReadModel> Handle(IList<IEvent> events)
        {
            this.updatedReadModels.Clear();

            // now, for each domain event, call a method that takes the event and call it
            foreach (var @event in events)
            {
                // todo: should we check for not handling the event or not.  read model builders probably don't need to 
                // handle *everthing*
                this.ExecuteMethodForParams(this, @event);
            }

            return this.updatedReadModels;
        }

        /// <summary>
        /// Provides a reminder to inheritors that we need to register which read models we update so that we can
        /// publish them on the event bus.  A bit of an annoyance but we'll see how well this works first.
        /// </summary>
        protected abstract void DoSaveReadModel(IReadModel readModel);

        protected void SaveReadModel(IReadModel readModel)
        {
            this.DoSaveReadModel(readModel);
            this.updatedReadModels.Add(readModel);
        }
    }
}

