namespace MonoKit
{
    using System;

    public class EventProxy<TTarget, TEventArgs> : IDisposable where TTarget : class where TEventArgs : EventArgs
    {
        private WeakReference target;

        public EventProxy(object target)
        {
            this.target = new WeakReference(target);
        }

        ~EventProxy()
        {
            Console.WriteLine("~EventProxy");
        }
        
        public void Dispose()
        {
            this.target.Target = null;
            this.Handle = null;
        }

        public void HandleEvent(object sender, TEventArgs args)
        {
            var t = this.target.Target as TTarget;

            if (this.Handle != null && t != null)
            {
                this.Handle(t, sender, args);
            }

        }

        public Action<TTarget, object, TEventArgs> Handle { get; set; }
    }
}

