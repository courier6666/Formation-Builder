using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SOLIDCheckingLibrary
{
    internal static class Utility
    {
        public static bool IsDefaultMethod(MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType == typeof(object)
                && (methodInfo.Name == "Equals" ||
                methodInfo.Name == "GetHashCode" ||
                methodInfo.Name == "ToString" ||
                methodInfo.Name == "GetType" ||
                methodInfo.Name == "MemberwiseClone" ||
                methodInfo.Name == "Finalize");
        }
        public static bool ArgumentsAreSame(MethodInfo method1, MethodInfo method2)
        {
            if (method1.GetParameters().Length != method2.GetParameters().Length) return false;

            var argumentsMethod1 = method1.GetParameters();
            var argumentsMethod2 = method2.GetParameters();
            for (int i = 0; i < argumentsMethod1.Length; ++i)
            {
                if (argumentsMethod1[i].ParameterType != argumentsMethod2[i].ParameterType) return false;
            }
            return true;
        }
        public static MethodInfo[] GetVirtualAbstractInterfaceMethods(Type type, bool ignoreInheritedClasses = true)
        {
            var interfaces = type.GetInterfaces();
            var interfaceMethods = interfaces.
                SelectMany(i => i.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
                Where(m => !IsDefaultMethod(m));

            if (ignoreInheritedClasses)
                methods = methods.Where(m => m.DeclaringType == type);

            var methodsInterfacesImplemented = methods.
                Where(m => !IsDefaultMethod(m)).
                Where(m => m.IsVirtual).
                Where(m => m.DeclaringType == type).
                Where(m => interfaceMethods.Any(im => m.ReturnType == im.ReturnType
                && ArgumentsAreSame(m, im)
                && im.Name == m.Name));

            var virtualMethods = methods.Where(m => !methodsInterfacesImplemented.Any(im => im == m)).
                Where(m => m.IsVirtual && !m.IsAbstract);

            var abstractMethods = methods.Where(m => m.IsAbstract);

            return methodsInterfacesImplemented.Concat(virtualMethods).Concat(abstractMethods).ToArray();
        }
    }
}
