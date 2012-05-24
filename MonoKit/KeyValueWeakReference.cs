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

namespace MonoKit
{
    using System;
    using System.Collections.Generic;
    
    internal class KeyedWeakReference<T1, T2> where T1 : class where T2 : class
    {
        private WeakReference target;
        
        internal KeyedWeakReference(T1 target)
        {
            this.target = new WeakReference(target);
        }
        
        internal KeyedWeakReference(T1 target, T2 key)
        {
            this.target = new WeakReference(target);
            this.Key = key;
        }

        public bool IsStale
        {
            get
            {
                if (this.target != null && this.target.IsAlive)
                {
                    return false;
                }
                
                return true;
            }
        }
        
        public T1 Target
        {
            get
            {
                if (this.target != null)
                {
                    return this.target.Target as T1;
                }
                
                return null;
            }
        }
        
        public T2 Key
        {
            get;
            private set;
        }
    }
    
    internal class KeyValueWeakReference<T1, T2, T3> : KeyedWeakReference<T1, T2> where T1 : class where T2 : class where T3 : class
    {      
        private WeakReference valueReference;
        
        internal KeyValueWeakReference(T1 target) : base(target)
        {
        }
        
        internal KeyValueWeakReference(T1 target, T2 key) : base(target, key)
        {
        }
        
        internal KeyValueWeakReference(T1 target, T2 key, T3 value) : base(target, key)
        {
            this.Value = value;
        }
        
        public T3 Value
        {
            get
            {
                if (this.valueReference != null)
                {
                    return this.valueReference.Target as T3;
                }
                
                return null;
            }
            
            set
            {
                if (value == null)
                {
                    this.valueReference = null;
                    return;
                }
                
                this.valueReference = new WeakReference(value);
            }
        }
    }
    
    internal class KeyValueWeakReference : KeyValueWeakReference<object, string, object>
    {         
        internal KeyValueWeakReference(object target) : base(target)
        {
        }
        
        internal KeyValueWeakReference(object target, string key) : base(target, key)
        {
        }
        
        internal KeyValueWeakReference(object target, string key, object value) : base(target, key, value)
        {
        }
    }
}

