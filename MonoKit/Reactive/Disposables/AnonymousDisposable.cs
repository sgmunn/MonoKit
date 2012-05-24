namespace MonoKit.Reactive.Disposables
{
    using System;
    using System.Threading;
    
    public class AnonymousDisposable : IDisposable
    {
        private int disposed;
        
        private Action onDispose;

        public AnonymousDisposable(Action onDispose)
        {
            this.onDispose = onDispose;
        }
        
        ~AnonymousDisposable()
        {
            // Worth noting that the onDispose action could be called on the Finalizer thread, which is not the main UI thread
            //this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private void Dispose(bool disposing)
        {
            // allow only one thread to invoke the dispose
            if (Interlocked.Exchange(ref this.disposed, 1) == 0)
            {
                if (this.onDispose != null)
                {
                    this.onDispose();
                    // clear this so that whatever objects the action is referencing can be collected
                    this.onDispose = null;
                }
            }
        }
    }

}

