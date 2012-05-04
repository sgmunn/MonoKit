namespace MonoKit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Reflection extensions.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets all Types that have the given attribute defined
        /// </summary>        
        public static IQueryable<Type> GetTypesWith<TAttribute>(bool inherit) where TAttribute: System.Attribute
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies().AsQueryable()
                from t in a.GetTypes()
                    where t.IsDefined(typeof(TAttribute),inherit)
                    select t;
        }
        
        /// <summary>
        /// Gets the attributes for a given Type as a Queryable
        /// </summary>
        /// <returns>
        /// A Queryable of attributes.
        /// </returns>
        public static IQueryable<Attribute> GetAttributes(Type objectType)
        {
            return System.Attribute.GetCustomAttributes(objectType).AsQueryable();
        }
        
        public static PropertyInfo GetPropertyInfo(this object instance, string propertyName)
        {
            // todo: cache property info
            return instance.GetType().GetProperty(propertyName);
        }


    }
}

