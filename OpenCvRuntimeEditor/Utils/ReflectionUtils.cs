namespace OpenCvRuntimeEditor.Utils
{
    using System;

    public static class ReflectionUtils
    {
        public static object GetDefaultValue(Type t)
        {
            if (t.ContainsGenericParameters)
                return null;

            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }
    }
}
