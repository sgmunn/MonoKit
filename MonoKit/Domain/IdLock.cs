//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="IdLock.cs" company="sgmunn">
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
    using System.Collections.Concurrent;
    using System.Threading;

    /// <summary>
    /// Provides a mechanism to lock id's.
    /// </summary>
    public sealed class IdLock : IDisposable
    {
        /// <summary>
        /// The static list of locks by id
        /// </summary>
        private static readonly ConcurrentDictionary<Guid, object> Locks = new ConcurrentDictionary<Guid, object>();

        /// <summary>
        /// The id this lock is for
        /// </summary>
        private readonly Guid id;
        
        /// <summary>
        /// The object to lock on
        /// </summary>
        private readonly object lockObject;

        /// <summary>
        /// A value indicating whether the lock has been disposed
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the IdLock class
        /// </summary>
        /// <param name="id">The id to lock on.</param>
        public IdLock(Guid id)
        {
            this.id = id;
            this.lockObject = Locks.GetOrAdd(id, x => new object());
            Monitor.Enter(this.lockObject);
        }

        /// <summary>
        /// Releases the lock
        /// </summary>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.isDisposed = true;

            object o;
            Locks.TryRemove(this.id, out o);
            Monitor.Exit(this.lockObject);
        }
    }
}

