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
