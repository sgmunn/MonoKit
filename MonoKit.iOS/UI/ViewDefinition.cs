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

namespace MonoKit.UI
{
    using System;
    using System.Collections.Generic;
    using MonoTouch.UIKit;
    
    public class UIViewDefinition<TView, TData> : ViewDefinition<TView> where TView : UIView
    {
        public UIViewDefinition() : base()
        {
            this.ViewType = typeof(TView);
            this.DataType = typeof(TData);
        }
        
        public UIViewDefinition(Action<UIView, object> bindingAction) : base()
        {
            this.ViewType = typeof(TView);
            this.DataType = typeof(TData);
            this.BindToSourceAction = bindingAction;
        }
        
        public UIViewDefinition(params Type[] behaviours) : base(behaviours)
        {
            this.ViewType = typeof(TView);
            this.DataType = typeof(TData);
        }
        
        public UIViewDefinition(Action<UIView, object> bindingAction, params Type[] behaviours) : base(behaviours)
        {
            this.ViewType = typeof(TView);
            this.DataType = typeof(TData);
            this.BindToSourceAction = bindingAction;
        }
    }
}

