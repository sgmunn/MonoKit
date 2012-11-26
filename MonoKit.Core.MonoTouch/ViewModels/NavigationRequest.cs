//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="NavigationRequest.cs" company="sgmunn">
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

namespace MonoKit.ViewModels
{
    using System;
    using System.Collections.Generic;

    public sealed class NavigationRequest : INavigationRequest
    {
        public NavigationRequest()
        {
            this.Params = new Dictionary<object, object>();
        }

        public NavigationRequest(string url)
        {
            this.Url = url;
            this.Params = new Dictionary<object, object>();
        }

        public NavigationRequest(IViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.Params = new Dictionary<object, object>();
        }

        public string Url 
        { 
            get; 
            private set; 
        }

        public IViewModel ViewModel 
        { 
            get; 
            private set; 
        }

        public IDictionary<object, object> Params
        { 
            get; 
            private set; 
        }
    }
}
