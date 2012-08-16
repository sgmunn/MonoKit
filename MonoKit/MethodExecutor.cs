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

namespace MonoKit
{
    using System;
    using System.Reflection;
    using System.Linq;

    // todo: make a static class and use caching
    public static class MethodExecutor
    {
        public static bool ExecuteMethodForSingleParam(object instance, object param)
        {
            return ExecuteMethodForParams(instance, param);
//            var method = this.GetCommandHandlerMethod(instance, param);
//
//            if (method != null)
//            {
//                method.Invoke(instance, new object[1] { param });
//                return true;
//            }
//            
//            return false;
        }

        public static bool ExecuteMethodForParams(object instance, params object[] args)
        {
            var method = GetCommandHandlerMethod(instance, args);

            if (method != null)
            {
                method.Invoke(instance, args);
                return true;
            }
            
            return false;
        }
        
        // todo: cache method info for aggregate and command combination
//        private MethodInfo GetCommandHandlerMethod(object instance, object param)
//        {
//            foreach (var method in instance.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
//            {
//                var methodParams = method.GetParameters();
//                if (methodParams.Count() == 1)
//                {
//                    var p1 = methodParams[0];
//                    if (p1 != null && p1.ParameterType == param.GetType())
//                    {
//                        return method;
//                    }
//                }
//            }
//            
//            return null;
//        }

        private static MethodInfo GetCommandHandlerMethod(object instance, params object[] args)
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