namespace MonoKit.Reactive.Disposables
{
    using System;

    public sealed class NullDisposable : IDisposable
    {
        public static readonly NullDisposable Instance = new NullDisposable();

        private NullDisposable()
        {
        }

        public void Dispose()
        {
        }
    }
}
