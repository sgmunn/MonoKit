// --------------------------------------------------------------------------------------------------------------------
// <copyright file=".cs" company="sgmunn">
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
 
    /// <summary>
    /// Implements an observer of T and performs actions when each of the OnNext, OnCompleted, OnError are called
    /// </summary>
    public class AnonymousObserver<T> : IObserver<T>
    {
        private readonly Action<T> onNext;

        private readonly Action<Exception> onError;

        private readonly Action onCompleted;

        public AnonymousObserver(Action<T> onNext)
        {
            this.onNext = onNext;
        }
        
        public AnonymousObserver(Action<T> onNext, Action onCompleted)
        {
            this.onNext = onNext;
            this.onCompleted = onCompleted;
        }
        
        public AnonymousObserver(Action<T> onNext, Action<Exception> onError)
        {
            this.onNext = onNext;
            this.onError = onError;
        }
        
        public AnonymousObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            this.onNext = onNext;
            this.onError = onError;
            this.onCompleted = onCompleted;
        }
        
        public AnonymousObserver(Action onCompleted)
        {
            this.onCompleted = onCompleted;
        }

        public void OnCompleted ()
        {
            if (this.onCompleted != null)
            {
                this.onCompleted();
            }
        }

        public void OnError (Exception error)
        {
            if (this.onError != null)
            {
                this.onError(error);
            }
        }

        public void OnNext (T value)
        {
            if (this.onNext != null)
            {
                this.onNext(value);
            }
        }
    }
}

