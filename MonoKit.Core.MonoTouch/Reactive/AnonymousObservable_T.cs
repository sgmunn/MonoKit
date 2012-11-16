// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnonymousObservable_T.cs" company="sgmunn">
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

    /// <remarks>
    /// Takes a function that returns a subscription token from an observer.  The function allows us to have side
    /// effects when we subscribe to an observable whilst being subscribed.
    /// </remarks>
    public class AnonymousObservable<T> : IObservable<T>
    {
        private readonly Func<IObserver<T>, IDisposable> onSubscribe;

        /// <param name='subscribe'>A function to subscribe an observer to</param>
        public AnonymousObservable(Func<IObserver<T>, IDisposable> onSubscribe)
        {
            this.onSubscribe = onSubscribe;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            // invoke the subscribe function and register the observer, 
            // we get given a token that we'll dispose
            IDisposable subscription = this.onSubscribe(observer);

            // returns a disposable that will dispose the subscription that the subscribe function
            // subscribed us to.
            //return Disposable.Create(() => subscription.Dispose()); 
            return subscription;
        }
    }
}

