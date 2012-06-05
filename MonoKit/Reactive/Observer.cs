//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file=".cs" company="sgmunn">
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

using MonoKit.Reactive.Concurrency;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonoKit.Reactive.Subjects;

namespace MonoKit.Reactive
{
    using System;
    using MonoKit.Reactive.Disposables;

//    internal class HotObservable<T> : IObservable<T>
//    {
//        IDisposable scheduler_disposable;
//        Subject<T> subject;
//
//        public HotObservable (Action<IObserver<T>> work, IScheduler scheduler)
//        {
//            subject = new Subject<T> ();
//            scheduler_disposable = scheduler.Schedule (() => work (subject));
//        }
//
//        bool disposed;
//
//        public void Dispose ()
//        {
//            if (disposed)
//                return;
//            disposed = true;
//            scheduler_disposable.Dispose ();
//        }
//
//        public IDisposable Subscribe (IObserver<T> observer)
//        {
//            return subject.Subscribe (observer);
//        }
//    }

    public static class Observer
    {
        public static IObserver<T> Create<T>(Action<T> onNext)
        {
            return new AnonymousObserver<T>(onNext);
        }

        public static IObserver<T> Create<T>(Action<T> onNext, Action onCompleted)
        {
            return new AnonymousObserver<T>(onNext, onCompleted);
        }

        public static IObserver<T> Create<T>(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return new AnonymousObserver<T>(onNext, onError, onCompleted);
        }
    }
}
