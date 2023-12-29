using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static SOLIDCheckingLibrary.Utility;

namespace SOLIDCheckingLibrary
{
    public static class SingleResponsibility
    {
        public static (bool, string) CheckClassMethodsForManyParameters(Type type, int countThreshold = 8)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
                Where(m=>m.DeclaringType == type).
                Where(m=>!IsDefaultMethod(m)).
                ToList();

            bool followsPrinciple = true;
            string checkLog = "";

            foreach (var method in methods)
            {
                if (method.GetParameters().Length > countThreshold)
                {
                    followsPrinciple = false;
                    checkLog += $"Method {method.ToString()} has too many arguments: {method.GetParameters().Length}.\n";
                }
            }
            if (followsPrinciple) checkLog = $"The class methods' arguments count doesn't excede threshold: {countThreshold}.";
            else checkLog = $"Following methods exceded threshold of number of arguments - {countThreshold}:\n" + checkLog;

            return (followsPrinciple, checkLog);
        }
        public static (bool, string) CheckClassForManyMethods(Type type, int countThreshold = 12)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
                Where(m => m.DeclaringType == type).
                Where(m => !IsDefaultMethod(m)).
                ToList();

            bool followsPrinciple = (methods.Count <= countThreshold);
            return (followsPrinciple, $"The class has {methods.Count}, threshold is {countThreshold}!");
        }
        public static (bool, string) CheckClassForSingleResponsibility(Type type, int thresholdOfMethods = 12, int thresholdOfMethodParameters = 8)
        {
            var resultMethods = CheckClassForManyMethods(type, thresholdOfMethods);
            var resultParameters = CheckClassMethodsForManyParameters(type, thresholdOfMethodParameters);

            return (resultMethods.Item1 & resultParameters.Item1, resultMethods.Item2 + "\n" + resultParameters.Item2);
        }
    }
}
