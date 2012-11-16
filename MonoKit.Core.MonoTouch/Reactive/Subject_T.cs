// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subject_T.cs" company="sgmunn">
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

namespace MonoKit.Reactive
{
    using System;
    using System.Collections.Generic;

    public class Subject<T> : IObserver<T>, IObservable<T>, IDisposable
    {
        private readonly List<IObserver<T>> observers;
        
        private object lockObject = new object();

        private bool stopped;

        private bool disposed;
        
        public Subject()
        {
            this.observers = new List<IObserver<T>>();
        }
        
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (this.stopped)
            {
                observer.OnCompleted();
            }

            // create an anonymous disposable that will remove the observer when it is disposed
            var subscription = Disposable.Create(() => this.RemoveObserver(observer));
            lock (this.lockObject)
            {
                this.observers.Add(observer);
            }

            return subscription;
        }

        public virtual void OnNext(T value)
        {
            this.CheckDisposed();
            if (this.stopped)
            {
                // or throw?
                return;
            }

            foreach (var observer in this.GetCurrentObservers())
            {
                observer.OnNext(value);
            }
        }

        public void OnCompleted()
        {
            this.CheckDisposed();
            this.stopped = true;
            foreach (var observer in this.GetCurrentObservers())
            {
                observer.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            this.CheckDisposed();
            foreach (var observer in this.GetCurrentObservers())
            {
                observer.OnError(error);
            }
        }

        public void Dispose()
        {
            this.disposed = true;
        }

        private void CheckDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Subject");
            }
        }

        private void RemoveObserver(IObserver<T> observer)
        {
            lock (this.lockObject)
            {
                this.observers.Remove(observer);
            }  
        }
        
        private List<IObserver<T>> GetCurrentObservers()
        {
            var observersToUpdate = new List<IObserver<T>>();
            lock (this.lockObject)
            {
                observersToUpdate.AddRange(this.observers);
            }
            
            return observersToUpdate;
        }
    }
}

