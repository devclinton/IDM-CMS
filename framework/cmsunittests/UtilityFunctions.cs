/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System.Reflection;
using distlib.hidden;

namespace cmsunittests
{
    class UtilityFunctions
    {
        public static T GetField<T>(object obj, string fieldName)
        {
            FieldInfo fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)fieldInfo.GetValue(obj);
        }

        public static void CallMethod(object obj, string methodName, object[] parameters = null)
        {
            MethodInfo methodInfo = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(obj, parameters);
        }
    }
}
