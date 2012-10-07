//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="AggregateRoot_TState.cs" company="sgmunn">
//    (c) sgmunn 2012  
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//    documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//    to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//    the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//    THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//    CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//    IN THE SOFTWARE.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
//
namespace MonoKit.Domain
{
    using System;

    public abstract class AggregateRoot<TState> : AggregateRoot, ISnapshotSupport where TState : class, ISnapshot, new()
    {
        public AggregateRoot()
        {
            this.InternalState = new TState();
        }

        protected TState InternalState { get; private set; }

        public virtual void LoadFromSnapshot(ISnapshot snapshot)
        {
            this.InternalState = snapshot as TState;
            this.Version = snapshot.Version;
            this.Identity = snapshot.Identity;
        }

        public abstract ISnapshot GetSnapshot();
    }
}

