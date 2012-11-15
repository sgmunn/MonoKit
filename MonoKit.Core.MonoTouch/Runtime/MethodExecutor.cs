// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodExecutor.cs" company="sgmunn">
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

namespace MonoKit.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Linq;

    /// <summary>
    /// Provides a way to execute an unnamed method that accepts a single parameter of a specific type
    /// This is used primarily to support executing commands against an aggregate and applying events
    /// </summary>
    public static class MethodExecutor
    {
        private static ConcurrentDictionary<Type, ConcurrentDictionary<Type, MethodInfo>> methodCache = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, MethodInfo>>();

        public static bool ExecuteMethod(object instance, object param)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            var methodInfo = GetFromCache(instance, param);
            if (methodInfo == null)
            {
                methodInfo = GetMethodForParams(instance, param);
                AddToCache(instance, param, methodInfo);
            }

            if (methodInfo != null)
            {
                methodInfo.Invoke(instance, new[] { param });
                return true;
            }

            return false;
        }

        private static bool ExecuteMethodForParams(object instance, params object[] args)
        {
            var method = GetMethodForParams(instance, args);

            if (method != null)
            {
                method.Invoke(instance, args);
                return true;
            }
            
            return false;
        }

        private static MethodInfo GetFromCache(object instance, object param)
        {
            ConcurrentDictionary<Type, MethodInfo> instanceCache;

            if (methodCache.TryGetValue(instance.GetType(), out instanceCache))
            {
                MethodInfo methodInfo;
                if (instanceCache.TryGetValue(param.GetType(), out methodInfo))
                {
                    return methodInfo;
                }
            }

            return null;
        }

        private static void AddToCache(object instance, object param, MethodInfo methodInfo)
        {
            ConcurrentDictionary<Type, MethodInfo> instanceCache;
            
            if (methodCache.TryGetValue(instance.GetType(), out instanceCache))
            {
                instanceCache.TryAdd(param.GetType(), methodInfo);
            }
            else
            {
                instanceCache = new ConcurrentDictionary<Type, MethodInfo>();
                instanceCache.TryAdd(param.GetType(), methodInfo);
                methodCache.TryAdd(instance.GetType(), instanceCache);
            }
        }

        private static MethodInfo GetMethodForParams(object instance, params object[] args)
        {
            foreach (var method in instance.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                var methodParams = method.GetParameters();
                if (methodParams.Count() == args.Count())
                {
                    bool matches = true;
                    for (int i = 0; i < args.Count(); i++)
                    {
                        var p1 = methodParams [i];

                        if (p1 == null || p1.ParameterType != args [i].GetType())
                        {
                            matches = false;
                        }
                    }

                    if (matches)
                    {
                        return method;
                    }
                }
            }
            
            return null;
        }
    }
}