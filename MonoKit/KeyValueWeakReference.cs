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

