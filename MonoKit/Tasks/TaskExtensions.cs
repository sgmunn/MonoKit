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

namespace MonoKit.Tasks
{
    using System;
    using System.Threading.Tasks;
    using MonoKit.Reactive.Subjects;
    using MonoKit.Reactive;

    public static class TaskExtensions
    {
        public static IObservable<Unit> AsObservable<T>(this Task task)
        {
            var subject = new Subject<Unit>();

            task.ContinueWith
                (t => 
                 {
                    Console.WriteLine("Task Error {0} ", t.Exception);
                    subject.OnError(t.Exception); 
                }, TaskContinuationOptions.OnlyOnFaulted);

            task.ContinueWith
                (t => 
                 {
                    try
                    {

                    
                    Unit res;
                    subject.OnNext(res);
                    subject.OnCompleted(); 
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Task error on completion {0}", ex);
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

            return new AnonymousObservable<Unit>(o => {
                // subscribe the observer to our subject first,
                var subscription = subject.Subscribe(o);

                // start the task
                task.Start();

                // return the subscription token
                return subscription;
            }
            );
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

            return new AnonymousObservable<T>(o => {
                // subscribe the observer to our subject first,
                var subscription = subject.Subscribe(o);

                // start the task
                task.Start();

                // return the subscription token
                return subscription;
            }
            );
        }
    }
}

