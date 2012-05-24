namespace MonoKit.Reactive.Disposables
{
    using System;

    public static class Disposable
    {
        public static IDisposable Empty
        {
            get
            {
                return NullDisposable.Instance;
            }
        }
        
        public static IDisposable Create(Action onDispose)
        {
            return new AnonymousDisposable(onDispose);
        }
    }
}

