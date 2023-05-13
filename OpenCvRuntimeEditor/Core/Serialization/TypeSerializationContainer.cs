namespace OpenCvRuntimeEditor.Core.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Newtonsoft.Json;
    using Utils;

    internal class TypeSerializationContainer
    {
        private readonly Dictionary<Type, int> _cachedTypeIndices = new Dictionary<Type, int>();
        public List<SerializedType> Types = new List<SerializedType>();

        public int GetSerializedTypeIndex(Type type)
        {
            if (_cachedTypeIndices.TryGetValue(type, out var index))
                return index;

            SerializedType serializedType;

            if (type.IsGenericParameter)
            {
                var declaringType = type.DeclaringType;
                var declaringTypeIndex = GetSerializedTypeIndex(declaringType);
                var genericParameterIndex = type.GenericParameterPosition;
                serializedType = new SerializedType(declaringTypeIndex, genericParameterIndex);
            }
            else if (type.IsConstructedGenericType)
            {
                var defenitionIndex = GetSerializedTypeIndex(type.GetGenericTypeDefinition());
                var genericParameterArgs = type.GetGenericArguments().Select(GetSerializedTypeIndex).ToArray();
                serializedType = new SerializedType(defenitionIndex, genericParameterArgs);
            }
            else
                serializedType = new SerializedType(type);

            index = Types.Count;
            _cachedTypeIndices.Add(type, index);
            Types.Add(serializedType);
            return index;
        }

        public Type GetTypeByIndex(int index)
        {
            if (index == -1)
            {
                Logger.LogError("Old saved file versions is no longer supported. I really sorry about that. Really hard to do back compatibility.");
                throw new InvalidOperationException("Old file versions are no longer supported");
            }

            Type type;
            var serializedType = Types[index];

            if (serializedType.CachedType != null)
                return serializedType.CachedType;

            if (serializedType.IsGenericParameter)
            {
                var declaringType = GetTypeByIndex(serializedType.GenericOwnerCacheIndex);
                var genericArguments = declaringType.GetGenericArguments();
                var position = serializedType.GenericParameterPosition;

                if (genericArguments.Length < position)
                    type = genericArguments[position];
                else
                {
                    if (declaringType != typeof(ErrorLoadingType))
                    {
                        Logger.LogError(
                            $"Generic type '{declaringType.Name}' don't have enough generic parameters to get generic parameter under position {position}");
                    }

                    type = typeof(ErrorLoadingType);
                }
            }
            else if (serializedType.IsConstructedGenericType)
            {
                var definitionType = GetTypeByIndex(serializedType.GenericOwnerCacheIndex);

                if (definitionType == typeof(ErrorLoadingType))
                    type = typeof(ErrorLoadingType);
                else
                {
                    var genericArguments = serializedType.ConstructedGenericTypeParameterArgumentIndices.Select(GetTypeByIndex).ToArray();
                    type = definitionType.MakeGenericType(genericArguments);

                    if (type == typeof(ErrorLoadingType) && genericArguments.All(x => x != typeof(ErrorLoadingType)))
                    {
                        Logger.LogError($"Cannot make generic from type {TypeRenamingUtils.RenameType(definitionType)} using arguments: " +
                                        $"{string.Join(", ", genericArguments.Select(x => TypeRenamingUtils.RenameType(x)))}");
                    }
                }
            }
            else
            {
                type = TypeLoadUtils.FindType(serializedType.ClassName, serializedType.AssemblyName);

                if (type == null)
                {
                    Logger.LogError(string.Format(ThrowHelpers.REFLECTION_TYPE_LOAD, serializedType.ClassName, serializedType.AssemblyName));
                    type = typeof(ErrorLoadingType);
                }
            }

            serializedType.CachedType = type;

            return type;
        }

        public SerializedMethod SerializeMethod(MethodInfo methodInfo)
        {
            var prams = methodInfo.GetParameters().Select(x => GetSerializedTypeIndex(x.ParameterType)).ToArray();
            var declaringTypeIndex = GetSerializedTypeIndex(methodInfo.DeclaringType);
            return new SerializedMethod(declaringTypeIndex, methodInfo.Name, prams);
        }

        public MethodInfo DeserializeMethod(SerializedMethod serializedMethod)
        {
            var declaringType = GetTypeByIndex(serializedMethod.DeclaringSerializedTypeIndex);
            var parameterTypes = serializedMethod.ParameterTypeIndices.Select(GetTypeByIndex).ToArray();
            var method = declaringType.GetMethod(serializedMethod.MethodName, parameterTypes);

            if (method == null && parameterTypes.All(x => x != typeof(ErrorLoadingType)))
                Logger.LogError($"Cannot reflect method {serializedMethod.MethodName}");

            return method;
        }

        public SerializedProperty SerializeProperty(PropertyInfo propertyInfo)
        {
            var declaringTypeIndex = GetSerializedTypeIndex(propertyInfo.DeclaringType);
            return new SerializedProperty(declaringTypeIndex, propertyInfo.Name);
        }

        public PropertyInfo DeserializeProperty(SerializedProperty serializedProperty)
        {
            var declaringType = GetTypeByIndex(serializedProperty.DeclaringSerializedTypeIndex);
            var propertyInfo = declaringType.GetProperty(serializedProperty.PropertyName);

            if (propertyInfo == null && declaringType != typeof(ErrorLoadingType))
                Logger.LogError($"Cannot reflect method {serializedProperty.PropertyName}");

            return propertyInfo;
        }
    }

    internal class SerializedType
    {
        public string AssemblyName;
        public string ClassName;
        public bool IsGenericParameter;
        public int GenericOwnerCacheIndex;
        public int GenericParameterPosition;
        public bool IsConstructedGenericType;
        public int[] ConstructedGenericTypeParameterArgumentIndices;

        [JsonConstructor]
        public SerializedType()
        {
        }

        [JsonIgnore]
        public Type CachedType { get; set; }

        public SerializedType(Type type)
        {
            AssemblyName = type.Assembly.GetName().Name;
            ClassName = type.FullName;
        }

        public SerializedType(int genericOwnerCacheIndex, int genericParameterPosition)
        {
            IsGenericParameter = true;
            GenericOwnerCacheIndex = genericOwnerCacheIndex;
            GenericParameterPosition = genericParameterPosition;
        }

        public SerializedType(int genericOwnerCacheIndex, int[] constructedGenericTypeParameterArgumentIndices)
        {
            IsConstructedGenericType = true;
            GenericOwnerCacheIndex = genericOwnerCacheIndex;
            ConstructedGenericTypeParameterArgumentIndices = constructedGenericTypeParameterArgumentIndices;
        }
    }

    internal class SerializedMethod
    {
        public int DeclaringSerializedTypeIndex;
        public string MethodName;
        public int[] ParameterTypeIndices;

        [JsonConstructor]
        public SerializedMethod()
        {
        }

        public SerializedMethod(int declaringSerializedTypeIndex, string methodName, int[] parameterTypeIndices)
        {
            DeclaringSerializedTypeIndex = declaringSerializedTypeIndex;
            MethodName = methodName;
            ParameterTypeIndices = parameterTypeIndices;
        }
    }

    internal class SerializedProperty
    {
        public int DeclaringSerializedTypeIndex;
        public string PropertyName;

        [JsonConstructor]
        public SerializedProperty()
        {
        }

        public SerializedProperty(int declaringSerializedTypeIndex, string propertyName)
        {
            DeclaringSerializedTypeIndex = declaringSerializedTypeIndex;
            PropertyName = propertyName;
        }
    }
}
