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
    
    public class MethodExecutor
    {
        public bool ExecuteMethodForSingleParam(object instance, object param)
        {
            var method = this.GetCommandHandlerMethod(instance, param);

            if (method != null)
            {
                method.Invoke(instance, new object[1] { param });
                return true;
            }
            
            return false;
        }
        
        // todo: cache method info for aggregate and command combination
        private MethodInfo GetCommandHandlerMethod(object instance, object param)
        {
            foreach (var method in instance.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                var methodParams = method.GetParameters();
                if (methodParams.Count() == 1)
                {
                    var p1 = methodParams[0];
                    if (p1 != null && p1.ParameterType == param.GetType())
                    {
                        return method;
                    }
                }
            }
            
            return null;
        }
    }
}