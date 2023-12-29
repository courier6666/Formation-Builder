using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SOLIDCheckingLibrary
{
    public static class InterfaceSegregation
    {
        private static object? CreateInstanceOfClass(Type classType)
        {
            var firstConstructor = classType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
                First();
            var arguments = firstConstructor.GetParameters().
                Select(p => p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType) : null).
                ToArray();

            return firstConstructor.Invoke(arguments);
        }

        public static bool MethodIsImplemented(MethodInfo methodInfo, Type initialClass)
        {
            var arguments = methodInfo.GetParameters().
                Select(p => p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType) : null).
                ToArray();

            try
            {
                var instance = CreateInstanceOfClass(initialClass);
                methodInfo.Invoke(instance, arguments);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException is NotImplementedException)
                    return false;
            }
            catch (Exception ex)
            {
                return true;
            }

            return true;
        }
        public static (bool, string) ClassMethodsFollowPrinciple(Type type)
        {
            var allMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            bool followsPrinciple = true;
            string checkLog = "";

            foreach (var method in allMethods)
            {
                if (!MethodIsImplemented(method, type))
                {
                    followsPrinciple = false;
                    checkLog += $"Method {method.ToString()} is not implemented!\n";
                }
            }
            if (followsPrinciple) checkLog = $"Methods of class {type.Name} follow the principle";
            else checkLog = $"Methods of class {type.Name} failed to follow the principle!\n" + checkLog;

            return (followsPrinciple, checkLog);
        }
        public static (bool, string) ClassFollowsPrinciple(Type type)
        {
            bool followsPrinciple = true;
            string checkLog = "";
            var methodsCheck = ClassMethodsFollowPrinciple(type);
            if (!methodsCheck.Item1)
            {
                checkLog += methodsCheck.Item2 + "\n";
                followsPrinciple = false;
            }
            if (followsPrinciple) checkLog = $"Class {type.Name} follows the principle of interface segregation";
            else checkLog = $"Class {type.Name} doesn't follows the principle of interface segregation:\n" + checkLog;
            return (followsPrinciple, checkLog);
        }
    }
}
