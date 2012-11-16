//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="BindingScopeInjectedProperty.cs" company="sgmunn">
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

namespace MonoKit.DataBinding
{
    using System;

    public static class BindingScopeInjectedProperty
    {
        public static InjectedProperty BindingScopeProperty
        {
            get
            {
                return InjectedProperty.Register("BindingScope", typeof(IBindingScope), new InjectedPropertyMetadata(BindingScopeChanged));
            }
        }
        
        public static IBindingScope GetBindingScope(this IPropertyInjection owner)
        {
            return (IBindingScope)owner.InjectedProperties.GetInjectedProperty(BindingScopeInjectedProperty.BindingScopeProperty);
        }
        
        public static IBindingScope GetBindingScopeOrDefault(this IPropertyInjection owner)
        {
            var scope = owner.GetBindingScope();
            if (scope == null)
            {
                scope = new BindingScope();
                owner.SetBindingScope(scope);
            }

            return scope;
        }

        public static void SetBindingScope(this IPropertyInjection owner, IBindingScope value)
        {
            owner.InjectedProperties.SetInjectedProperty(BindingScopeInjectedProperty.BindingScopeProperty, value);
        }

        private static void BindingScopeChanged(object target, InjectedPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                ((IBindingScope)e.OldValue).ClearBindings();
            }
        }
    }
}
