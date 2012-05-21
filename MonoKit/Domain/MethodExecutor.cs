// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodExecutor.cs" company="sgmunn">
//   (c) sgmunn 2012  
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Domain
{
    using System;
    using System.Reflection;
    using System.Linq;

    // todo: make a static class and use caching
    public class MethodExecutor
    {
        public bool ExecuteMethodForSingleParam(object instance, object param)
        {
            return this.ExecuteMethodForParams(instance, param);
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

        public bool ExecuteMethodForParams(object instance, params object[] args)
        {
            var method = this.GetCommandHandlerMethod(instance, args);

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

        private MethodInfo GetCommandHandlerMethod(object instance, params object[] args)
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