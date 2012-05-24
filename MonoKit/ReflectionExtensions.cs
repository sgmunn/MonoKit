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
        
        public static IEnumerable<T> GetAttributes<T>(this MemberInfo member, bool inherit)
        {
            return Attribute.GetCustomAttributes(member, inherit).OfType<T>();
        }


    }
}

