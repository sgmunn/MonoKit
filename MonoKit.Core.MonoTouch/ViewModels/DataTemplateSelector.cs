//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="DataTemplateSelector.cs" company="sgmunn">
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

    public class DataTemplateSelector : IDataTemplateSelector
    {
        private Type viewModelType;
        private Func<object, bool> selector;
        private Func<string, object> creator; 
        private Action<object> initializer;
        private Action<object, object> binder;
        private Func<object, float> height;

        public DataTemplateSelector(string reuseIdentifier)
        {
            this.ReuseIdentifier = reuseIdentifier;
        }
        
        public string ReuseIdentifier
        {
            get;
            private set;
        }

        public Type ViewType
        {
            get;
            private set;
        }

        public DataTemplateSelector WhenSelecting<TViewModel>(Func<TViewModel, bool> selector)
        {
            this.viewModelType = typeof(TViewModel);
            this.selector = (v) => selector((TViewModel)v);

            return this;
        }

        public DataTemplateSelector Creates<TView>(Func<string, TView> creator)
        {
            if (creator == null)
            {
                throw new ArgumentNullException("creator");
            }

            this.ViewType = typeof(TView);
            this.creator = creator;
            
            return this;
        }

        public DataTemplateSelector WhenInitializing<TView>(Action<TView> initializer)
        {
            if (initializer == null)
            {
                throw new ArgumentNullException("initializer");
            }
            
            this.initializer = (x) => initializer((TView)x);
            
            return this;
        }
        
        public DataTemplateSelector WhenBinding<TViewModel, TView>(Action<TViewModel, TView> binder)
        {
            if (binder == null)
            {
                throw new ArgumentNullException("binder");
            }
            
            if (this.viewModelType == null)
            {
                this.viewModelType = typeof(TViewModel);
            }
            
            this.binder = (vm, view) => binder((TViewModel)vm, (TView)view);
            
            return this;
        }

        public DataTemplateSelector HavingHeight<TViewModel>(Func<TViewModel, float> height)
        {
            this.height = (vm) => height((TViewModel)vm);
            return this;
        }
        
        public DataTemplateSelector HavingHeight(float height)
        {
            this.height = (vm) => height;
            return this;
        }

        public TemplateMatch AppliesToViewModel(object viewModel)
        {
            if (viewModel == null)
            {
                return TemplateMatch.None;
            }

            if (this.viewModelType == viewModel.GetType())
            {
                return TemplateMatch.Exact;
            }

            if (this.viewModelType.IsAssignableFrom(viewModel.GetType()))
            {
                if (this.selector != null && this.selector(viewModel))
                {
                    return TemplateMatch.Exact;
                }

                return TemplateMatch.Assignable;
            }
            
            return TemplateMatch.None;    
        }
        
        public object CreateView()
        {
            if (this.creator == null)
            {
                throw new InvalidOperationException("No Creator function specified");
            }

            return this.creator(this.ReuseIdentifier);
        }
        
        public void InitializeView(object view)
        {
            if (this.initializer != null)
            {
                this.initializer(view);
            }
        }
        
        public void BindViewModel(object viewModel, object view)
        {
            if (this.binder != null)
            {
                this.binder(viewModel, view);
            }
        }
        
        public float CalculateHeight(object viewModel)
        {
            if (this.height != null)
            {
                return this.height(viewModel);
            }
            
            return -1;
        }
    }
}
