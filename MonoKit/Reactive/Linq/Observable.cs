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

namespace MonoKit.Reactive.Linq
{
    using System;
    using System.Threading.Tasks;
    using MonoKit.Reactive;
    using MonoKit.Reactive.Concurrency;
    using MonoKit.Reactive.Subjects;
    using MonoKit.Reactive.Disposables;

    public static class Observable
    {
        public static IObservable<T> Empty<T>()
        {
            return new AnonymousObservable<T>(
                (observer) => 
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                });
        }
        
        public static IObservable<T>Create<T>(Func<IObserver<T>, Action> onSubscribe)
        {
            return Create<T>(observer => Disposable.Create(onSubscribe(observer)));
        }
        
        public static IObservable<T>Create<T>(Func<IObserver<T>, IDisposable> onSubscribe)
        {
            return new AnonymousObservable<T>(onSubscribe);
        }
                
        public static IObservable<T> ObserveOn<T>(this IObservable<T> source, IScheduler scheduler)
        {
            return new AnonymousObservable<T>
                (observer =>
                 {
                    var x = new ScheduledObserver<T>(scheduler, observer);

                    return source.Subscribe(x);
                });
        }

        public static IObservable<T> Return<T>(T value)
        {
            return new AnonymousObservable<T>(
                (observer) => 
                {
                    observer.OnNext(value);
                    observer.OnCompleted();
                    return Disposable.Empty;
                });
        }

        public static IObservable<T> Throw<T>(Exception ex)
        {
            return new AnonymousObservable<T>(
                (observer) => 
                {
                    observer.OnError(ex);
                    return Disposable.Empty;
                });
        }

        public static IObservable<T> Start<T>(Func<T> function)
        {
            var task = new Task<T>(function);
            return task.AsObservable();
        }

        public static IObservable<Unit> Start(Action action)
        {
            var task = new Task<Unit>
                (() => 
                 {
                    action();
                    return Unit.Default;
                });

            return task.AsObservable();
        }

        public static IObservable<T> AsObservable<T>(this Task<T> task)
        {
            var subject = new Subject<T>();

            task.ContinueWith
                (t => 
                 {
                    subject.OnError(t.Exception); 
                }, TaskContinuationOptions.OnlyOnFaulted);

            task.ContinueWith
                (t => 
                 {
                    subject.OnNext(t.Result);
                    subject.OnCompleted(); 
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

            return new AnonymousObservable<T>
                (o => {
                    // subscribe the observer to our subject first,
                    var subscription = subject.Subscribe(o);

                    // start the task
                    task.Start();

                    // return the subscription token
                    return subscription;
                });
        }

        public static IObservable<T> Do<T>(this IObservable<T> source, Action<T> onNextAction)
        {
            Func<IObserver<T>, IDisposable> subscribeFunction = observer => source.Subscribe(
                (x =>
                {
                    try
                    {
                        onNextAction(x);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    return;
                    }

                    observer.OnNext(x);
                }),
                observer.OnError,
                observer.OnCompleted);

            return new AnonymousObservable<T>(subscribeFunction);
        }

        public static IObservable<T> Where<T>(this IObservable<T> source, Func<T, bool> whereClause)
        {
            return new AnonymousObservable<T>(
                observer => source.Subscribe(
                    new AnonymousObserver<T>(
                    x =>
                    {
                        bool result;
                        try
                        {
                            result = whereClause(x);
                        }
                        catch (Exception exception)
                        {
                            observer.OnError(exception);
                            return;
                        }

                        if (result)
                        {
                            observer.OnNext(x);
                        }
                    },
                    observer.OnError,
                    observer.OnCompleted))
                    );


        }


        public static IObservable<TResult> Select<T, TResult>(this IObservable<T> source, Func<T, TResult> selector)
        {
            return new AnonymousObservable<TResult>(
                observer => source.Subscribe(
                    new AnonymousObserver<T>(
                    x =>
                    {
                        TResult result;

                        try
                        {
                            result = selector(x);
                        }
                        catch (Exception exception)
                        {
                            observer.OnError(exception);
                            return;
                        }

                        observer.OnNext(result);
                    },
                    observer.OnError,
                    observer.OnCompleted)));


        }


        public static IObservable<T> Take<T>(this IObservable<T> source, int count)
        {
            return new AnonymousObservable<T>(
                observer => source.Subscribe(
                    new AnonymousObserver<T>(
                                x =>
                                    {
                                        if (count > 0)
                                        {
                                            count--;
                                            observer.OnNext(x);
                                            if (count < 1)
                                            {
                                                observer.OnCompleted();
                                            }
                                        }
                                    },
                                observer.OnError,
                                observer.OnCompleted)));
        }


        public static IObservable<T> Skip<T>(this IObservable<T> source, int count)
        {
            return new AnonymousObservable<T>(
                observer => source.Subscribe(
                    new AnonymousObserver<T>(
                                x =>
                                    {
                                        if (count <= 0)
                                        {
                                            observer.OnNext(x);
                                        }
                                        else
                                        {
                                            count--;
                                        }
                                    },
                                observer.OnError,
                                observer.OnCompleted)));
        }
    }
}

