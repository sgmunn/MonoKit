namespace MonoKit.Reactive.Subjects
{
    using System;
    using System.Collections.Generic;
    using MonoKit.Reactive.Disposables;

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

        public void OnNext(T value)
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

