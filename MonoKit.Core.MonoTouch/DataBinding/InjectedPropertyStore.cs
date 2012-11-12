// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectedPropertyStore.cs" company="sgmunn">
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

namespace MonoKit.DataBinding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class InjectedPropertyStore : IPropertyInjection
    {
        private readonly Dictionary<InjectedProperty, object> injectedProperties;

        public InjectedPropertyStore()
        {
            this.injectedProperties = new Dictionary<InjectedProperty, object>();
        }

        public void AddInjectedProperty(InjectedProperty property, object value)
        {
            this.injectedProperties[property] = value;
        }

        public object GetInjectedProperty(InjectedProperty property)
        {
            object value = null;
            if (this.injectedProperties.TryGetValue(property, out value))
            {
                return value;
            }

            return property.Metadata.DefaultValue;
        }

        public void RemoveInjectedProperty(InjectedProperty property)
        {
            object value = null;
            if (this.injectedProperties.TryGetValue(property, out value))
            {
                var disposable = value as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }

                this.injectedProperties.Remove(property);
            }
        }

        public void RemoveAllInjectedProperties()
        {
            foreach (var disposable in this.injectedProperties.Values.OfType<IDisposable>())
            {
                disposable.Dispose();
            }

            this.injectedProperties.Clear();
        }
    }
}
