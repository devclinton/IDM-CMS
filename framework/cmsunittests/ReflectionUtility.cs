/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;
using System.Reflection;
using compartments.emod.utils;
using compartments.solvers;

namespace cmsunittests
{
    class ReflectionUtility
    {
        public static MethodInfo GetHiddenMethod(string methodName, object sourceObject)
        {
            return FindMethod(methodName, sourceObject.GetType());
        }

        public static MethodInfo FindMethod(string methodName, Type sourceType)
        {
            // Recurse (FindField()) through base types if the field is not found on the current type.
            return sourceType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance) ??
                   FindMethod(methodName, sourceType.BaseType);
        }

        public static T GetHiddenField<T>(string fieldName, object sourceObject)
        {
            var fieldInfo = FindField(fieldName, sourceObject.GetType());
            return (T)fieldInfo.GetValue(sourceObject);
        }

        public static void SetHiddenField(string fieldName, object sourceObject, object newValue)
        {
            FindAndSetField(fieldName, sourceObject.GetType(), sourceObject, newValue);
        }

        public static void FindAndSetField(string fieldName, Type sourceType, object sourceObject, object newValue)
        {
            var fieldInfo = sourceType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null) fieldInfo.SetValue(sourceObject, newValue);
        }

        public static FieldInfo FindField(string fieldName, Type sourceType)
        {
            // Recurse (FindField()) through base types if the field is not found on the current type.
            return sourceType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance) ??
                   FindField(fieldName, sourceType.BaseType);
        }

        public static string GetSolverRegimeName(TauLeaping solver)
        {
            var regimeType = typeof(TauLeaping).GetNestedType("Regime", BindingFlags.NonPublic);
            var regime = GetHiddenField<int>("_regime", solver);
            var regimeName = regimeType.GetEnumName(regime);

            return regimeName;
        }

        public static void RunResetRngFactory()
        {
            var methodInfo = typeof(RNGFactory).GetMethod("Reset", BindingFlags.NonPublic | BindingFlags.Static);
            methodInfo.Invoke(null, null);
        }
    }
}
