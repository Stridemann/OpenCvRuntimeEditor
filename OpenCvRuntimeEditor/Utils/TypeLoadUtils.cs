namespace OpenCvRuntimeEditor.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Core;
    using Core.Serialization;
    using JetBrains.Annotations;

    public static class TypeLoadUtils
    {
        private static readonly Dictionary<string, Assembly> _cachedAssemblies = new Dictionary<string, Assembly>();
        private static readonly Dictionary<string, Type> _cachedTypes = new Dictionary<string, Type>();

        static TypeLoadUtils()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    _cachedAssemblies.Add(assembly.GetName().Name, assembly);
                }
                catch (ArgumentException)//key is already exist exception
                {
                    Logger.LogError($"TypeLoadUtils: Assembly cache: assembly with the same type is already exist: {assembly.GetName().Name}");
                }
            }
        }

        [NotNull]
        public static Type FindType(string typeName, string assemblyName)
        {
            var cachedTypeName = typeName + assemblyName;

            if (_cachedTypes.TryGetValue(cachedTypeName, out var type))
            {
                return type;
            }

            if (_cachedAssemblies.TryGetValue(assemblyName, out var assembly))
            {
                type = assembly.GetType(typeName);
                if (type == null)
                {
                    type = typeof(ErrorLoadingType);
                    Logger.LogError(ThrowHelpers.REFLECTION_TYPE_LOAD, typeName, assemblyName);
                }
                _cachedTypes.Add(cachedTypeName, type);//type can be null here, but it's ok
                return type;
            }
            return null;
        }
    }
}
