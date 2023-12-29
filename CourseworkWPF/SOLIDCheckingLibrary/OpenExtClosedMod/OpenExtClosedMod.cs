using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static SOLIDCheckingLibrary.Utility;

namespace SOLIDCheckingLibrary
{
    public static class OpenExtClosedMod
    {
        public static (bool, string) IsClassExtensible(Type type, float acceptableExtensibility = 0.25f, bool ignoreInheritedClasses = true)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).
                Where(m => !IsDefaultMethod(m));

            if (ignoreInheritedClasses)
                methods = methods.Where(m => m.DeclaringType == type);

            var overridableMethods = GetVirtualAbstractInterfaceMethods(type, ignoreInheritedClasses);

            float coef = 0;
            if (methods.Count() != 0)
                coef = (float)overridableMethods.Count() / (float)methods.Count();

            if (Math.Round(coef, 2) >= acceptableExtensibility)
                return (true, $"Class is extendable enough, {Math.Round(coef, 2) * 100f} percent of methods can be overriden.");

            return (false, $"Class is NOT extendable enough, only {Math.Round(coef, 2) * 100f} percent of methods can be overriden.");
        }

    }
}
