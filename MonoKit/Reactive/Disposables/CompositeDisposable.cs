//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="CompositeDisposable.cs" company="sgmunn">
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

namespace MonoKit.Reactive.Disposables
{
    using System;
    using System.Collections.Generic;

    public class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> disposables;

        public CompositeDisposable()
        {
            this.disposables = new List<IDisposable>();
        }

        public void Dispose()
        {
            this.Clear();
        }

        public void Add(IDisposable disposable)
        {
            this.disposables.Add(disposable);
        }

        public void Clear()
        {
            foreach (var disposable in this.disposables)
            {
                disposable.Dispose();
            }

            this.disposables.Clear();
        }
    }
}

