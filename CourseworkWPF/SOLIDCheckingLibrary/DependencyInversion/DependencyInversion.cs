using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using static SOLIDCheckingLibrary.Utility;
using System.Text.RegularExpressions;

namespace SOLIDCheckingLibrary
{
    public static class DependencyInversion
    {
        public enum CheckingSettings
        {
            IgnoreBasicCollectionTypes = 0b00001,
            IgnoreInheritedFields = 0b00010,
            IgnoreModels = 0b00100
        }
        private static bool IsBasicCollectionType(Type type)
        {
            var allBasicTypes = ListOfAllBasicTypes.allBasicCollectionTypes;
            return allBasicTypes.FirstOrDefault(t => t.Name == type.Name) != null;
        }
        private static bool IsModel(Type type)
        {
            return Regex.IsMatch(type.Name, "model", RegexOptions.IgnoreCase);
        }
        private static bool TypeIsClass(Type type)
        {
            return type.IsClass && !type.IsAbstract && type != typeof(string);
        }
        private static bool CheckingTypeBasedOnSettings(Type type, CheckingSettings settings)
        {
            if ((settings & CheckingSettings.IgnoreBasicCollectionTypes) > 0)
                if (IsBasicCollectionType(type))
                    return true;

            if ((settings & CheckingSettings.IgnoreModels) > 0)
                if (IsModel(type))
                    return true;

            return false;
        }
        private static bool DoesTypeFollowPrinciple(Type type, Type firstInitialClassType, CheckingSettings settings)
        {
            if (type == firstInitialClassType) return true;
            try
            {
                var genericType = type.GetGenericTypeDefinition();
                var genericTypeArgument = type.GetGenericArguments();

                if (genericType != typeof(Task<>))
                {
                    if (TypeIsClass(genericType) && !CheckingTypeBasedOnSettings(type, settings)) return false;
                }

                foreach (var argument in genericTypeArgument)
                {
                    if (!DoesTypeFollowPrinciple(argument, firstInitialClassType, settings))
                        return false;
                }

                return true;
            }
            catch (InvalidOperationException)
            {
                return !TypeIsClass(type) || CheckingTypeBasedOnSettings(type, settings);
            }
        }
        public static (bool, string) ClassConstructorsFollowPrinciple(Type type, CheckingSettings settings)
        {
            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            string checkLog = "";
            bool followsPrinciple = true;

            foreach (var constructor in constructors)
            {
                bool ctorFollowPrinciple = true;
                string checkLogForCtor = "";
                int currentArgument = 1;
                foreach (var parameter in constructor.GetParameters())
                {
                    if (!DoesTypeFollowPrinciple(parameter.ParameterType, type, settings))
                    {
                        ctorFollowPrinciple = false;
                        checkLogForCtor += $"Argument {currentArgument} - {parameter.ParameterType.Name} {parameter.Name}, " +
                            $"is a class or if generic, contains class parameters!'\n";
                    }
                    currentArgument++;
                }
                followsPrinciple &= ctorFollowPrinciple;
                if (!ctorFollowPrinciple)
                {
                    checkLogForCtor = $"Constructor {constructor} doesn't follow the principle!\n" + checkLogForCtor;
                    checkLog += checkLogForCtor;
                }
            }
            if (followsPrinciple) checkLog = $"Class {type.Name} constructors  follows the principle!";
            else checkLog = $"Class {type.Name} constructors failed to follow the principle!\n" + checkLog;

            return (followsPrinciple, checkLog);
        }
        public static (bool, string) ClassMethodsFollowPrinciple(Type type, CheckingSettings settings)
        {
            var allMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).
                Where(m => !IsDefaultMethod(m));

            if ((settings & CheckingSettings.IgnoreInheritedFields) != 0) allMethods = allMethods.Where(m => m.DeclaringType == type).ToArray();

            string checkLog = "";
            bool followsPrinciple = true;

            foreach (var method in allMethods)
            {
                bool methodFollowsPrinciple = true;
                string checkLogForMethod = "";

                if (!DoesTypeFollowPrinciple(method.ReturnType, type, settings))
                {
                    methodFollowsPrinciple = false;
                    checkLogForMethod += $"Return type of method {method.ReturnType.Name} is a class or if generic, contains class parameters!\n";
                }

                int currentArgument = 1;
                foreach (var parameter in method.GetParameters())
                {
                    if (!DoesTypeFollowPrinciple(parameter.ParameterType, type, settings))
                    {
                        methodFollowsPrinciple = false;
                        checkLogForMethod += $"Argument {currentArgument} - {parameter.ParameterType.Name} {parameter.Name}, is a class or if generic, contains class parameters!'\n";
                    }
                    ++currentArgument;
                }

                if (!methodFollowsPrinciple)
                {
                    checkLogForMethod = $"Method {method} doesn't follow the principle!\n" + checkLogForMethod;
                    checkLog += checkLogForMethod;
                }
                followsPrinciple &= methodFollowsPrinciple;
            }

            if (followsPrinciple) checkLog = $"Methods of class {type.Name} follows the principle!";
            else checkLog = $"Methods of class {type.Name} failed to follow the principle!\n" + checkLog;

            return (followsPrinciple, checkLog);
        }
        public static (bool, string) ClassFieldsFollowPrinciple(Type type, CheckingSettings settings)
        {
            var allFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if ((settings & CheckingSettings.IgnoreInheritedFields) != 0) allFields = allFields.Where(f => f.DeclaringType == type).ToArray();

            string checkLog = "";
            bool followsPrinciple = true;

            foreach (var field in allFields)
            {
                if (!DoesTypeFollowPrinciple(field.FieldType, type, settings))
                {
                    followsPrinciple = false;
                    checkLog += $"Field  - {field.FieldType} {field.Name} -  " +
                        $"doesn't follow the principle, is a class or if generic, contains class parameters!\n";
                }
            }

            if (followsPrinciple) checkLog = $"Fields of class {type.Name} follows the principle!";
            else checkLog = $"Class fields {type.Name} failed to follow the principle!\n" + checkLog;

            return (followsPrinciple, checkLog);
        }
        public static (bool, string) ClassPropertiesFollowPrinciple(Type type, CheckingSettings settings)
        {
            var allProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if ((settings & CheckingSettings.IgnoreInheritedFields) != 0) allProperties = allProperties.Where(p => p.DeclaringType == type).ToArray();
            string checkLog = "";
            bool followsPrinciple = true;

            foreach (var property in allProperties)
            {
                if (!DoesTypeFollowPrinciple(property.PropertyType, type, settings))
                {
                    followsPrinciple = false;
                    checkLog += $"Property  - {property.PropertyType} {property.Name} -  " +
                        $"doesn't follow the principle, is a class or if generic, contains class parameters!\n";
                }
            }

            if (followsPrinciple) checkLog = $"Properties of class {type.Name} follows the principle!";
            else checkLog = $"Class properties {type.Name} failed to follow the principle!\n" + checkLog;

            return (followsPrinciple, checkLog);
        }
    }
}
