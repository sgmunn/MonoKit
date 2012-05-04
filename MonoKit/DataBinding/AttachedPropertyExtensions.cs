namespace MonoKit.DataBinding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides extension methods for handling AttachedProperties
    /// </summary>
    public static class AttachedPropertyExtensions
    {
        // todo: perform background check for stale values
  
        /// <summary>
        /// Value data store
        /// </summary>
        private static Dictionary<string, List<KeyValueWeakReference>> ValueDictionary = new Dictionary<string, List<KeyValueWeakReference>>();
        
        /// <summary>
        /// Sets the value of an AttachedProperty for an object 
        /// </summary>
        /// <param name='instance'>
        /// The instance for which the value is being set
        /// </param>
        /// <param name='property'>
        /// The AttachedProperty that defines the value being set
        /// </param>
        /// <param name='value'>
        /// The value of the property
        /// </param>
        public static void SetValue(this object instance, AttachedProperty property, object value)
        {
            var current = GetCurrent(instance, property, true);
            
            var oldValue = current.Value;
            current.Value = value;  
            
            if (oldValue != value && property.Metadata.ChangeCallback != null)
            {
                var args = new AttachedPropertyChangedEventArgs(property, value, oldValue);
                
                property.Metadata.ChangeCallback(instance, args);
            }
        }
        
        /// <summary>
        /// Gets the value of an AttachedProperty for an object 
        /// </summary>
        /// <param name='instance'>
        /// The instance for which the value is being retrieved
        /// </param>
        /// <param name='property'>
        /// The AttachedProperty that defines the value being retrieved
        /// </param>
        /// <returns>Returns an object, the default value or null</returns>
        public static object GetValue(this object instance, AttachedProperty property)
        {
            var current = GetCurrent(instance, property, false);
            if (current != null)
            {
                return current.Value;
            }
            
            return property.Metadata.DefaultValue;
        }
        
        /// <summary>
        /// Gets the current value of the property for the object
        /// </summary>
        /// <returns>
        /// Returns an object or null
        /// </returns>
        /// <param name='instance'>
        /// The object to look up in the values dictionary
        /// </param>
        /// <param name='property'>
        /// The property describing the value to look up.
        /// </param>
        /// <param name='addIfMissing'>Adds a reference if not found</param>
        private static KeyValueWeakReference GetCurrent(object instance, AttachedProperty property, bool addIfMissing)
        {
            KeyValueWeakReference result = null;
            var key = GetDicionaryKey(instance);
            
            if (ValueDictionary.ContainsKey(key))
            {
                var values = ValueDictionary[key];
                result = values.FirstOrDefault(x => x.Target == instance && x.Key == property.PropertyKey);
                
                if (result == null && addIfMissing)
                {
                    result = new KeyValueWeakReference(instance, property.PropertyKey);
                    values.Add(result);
                }
                
                return result;
            }
            
            if (addIfMissing)
            {
                result = new KeyValueWeakReference(instance, property.PropertyKey);
                ValueDictionary.Add(key, new List<KeyValueWeakReference>() { result });
            }
            
            return result;
        }
                
        /// <summary>
        /// Gets the key for the given object.
        /// </summary>
        /// <returns>
        /// Returns a string representing the key for given target
        /// </returns>
        private static string GetDicionaryKey(object target)
        {
            return string.Format("{0}-{1}", target.GetType(), target.GetHashCode());
        }

    }
}

