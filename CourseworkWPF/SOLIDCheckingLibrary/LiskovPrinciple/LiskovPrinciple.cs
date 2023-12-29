using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static SOLIDCheckingLibrary.Utility;

namespace SOLIDCheckingLibrary
{
    public static class LiskovPrinciple
    {


        /// <summary>
        /// ONLY use when parent class is the LAST argument!
        /// DON'T include that parent class in methodParam, it will be create manualy in this method
        /// and passed to reviewing function
        /// </summary>
        /// <param name="classInvokingMethod"></param>
        /// <param name="method"></param>
        /// <param name="methodParam"></param>
        /// <param name="childClass"></param>
        /// <param name="parentClass"></param>
        /// <param name="constructorParam"></param>
        /// <returns></returns>
        public static (bool, string) CheckLSPForMethodTwoClasses(Type classInvokingMethod, MethodInfo method, object[]? methodParam, Type childClass, Type parentClass, object[]? constructorParam)
        {
            var _childClass = Activator.CreateInstance(childClass, constructorParam);
            var _parentClass = Activator.CreateInstance(parentClass, constructorParam);

            if (methodParam == null)
                methodParam = new object[0];

            List<object> paramListChild = new List<object>(methodParam)
            {
                _childClass
            };
            List<object> paramListParent = new List<object>(methodParam)
            {
                _parentClass
            };

            bool result = true;
            string checkLog = "";

            try
            {
                result = method.Invoke(classInvokingMethod, paramListChild.ToArray()) ==
                    method.Invoke(classInvokingMethod, paramListParent.ToArray());
                if (!result) checkLog = "Result of method is not the same!";
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException is NotImplementedException)
                {
                    checkLog = "One of the classes is not implemented!";
                    result = false;
                }
            }
            catch (Exception ex)
            {

            }

            return (result, result ? $"Classes {childClass.Name} and {parentClass.Name} follow Liskov Principle!" :
                $"Classes {childClass.Name} and {parentClass.Name} DON'T follow Liskov Principle!\n" + checkLog);
        }
        public static (bool, string) CheckMethodsOfParentIsOverriden(Type childClass, Type parentClass)
        {
            var childMethods = childClass.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
                Where(m => !IsDefaultMethod(m)).ToList();
            var parentMethods = parentClass.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
                Where(m => !IsDefaultMethod(m)).ToList();

            bool followsPrinciple = true;
            string checkLog = "";

            foreach (var method in parentMethods)
            {
                var methodsInChild = childMethods.Where(m => method.Name == m.Name &&
                method.ReturnType == m.ReturnType &&
                ArgumentsAreSame(method, m)).ToList();

                // if there are more than two method found, it means that child class doesn't override method prvoided by parent
                // So we would get wrong result, it would violdate Liskov Principle
                if (method.IsVirtual && (methodsInChild.Any() && methodsInChild.Count >= 2))
                {
                    followsPrinciple = false;
                    checkLog += $"Class {childClass.Name} doesn't override method {method.ToString()} provided by class {parentClass.Name}.\n";
                }
            }

            if (!followsPrinciple) checkLog = $"Class {childClass.Name}, that is inheriting parent class {parentClass.Name}, doesn't follow the principle!";
            else checkLog = "\n" + checkLog;

            return (followsPrinciple, checkLog);
        }
    }
}
