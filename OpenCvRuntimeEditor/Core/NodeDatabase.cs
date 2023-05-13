namespace OpenCvRuntimeEditor.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Emgu.CV;
    using Emgu.CV.Structure;
    using JetBrains.Annotations;
    using NodeSpawn;
    using ProcessingStrategies;
    using ProcessingStrategies.Runtime;
    using Serialization;
    using Utils;
    using ViewModels.PinDataVisualProcessors;

    public static class NodeDatabase
    {
        private const string REFLECTION_TYPES_CONFIG_PATH = "NodesReflectionSources.txt";
        private const BindingFlags BINDING_FLAGS = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Instance;
        private static readonly Dictionary<Type, Type> _registeredPinDataViewModels = new Dictionary<Type, Type>();
        public static readonly Dictionary<ConverterNodeType, NodeCreationConfig> Converters = new Dictionary<ConverterNodeType, NodeCreationConfig>();
        internal static ObservableCollection<NodeCreationConfig> RegisteredNodes { get; } = new ObservableCollection<NodeCreationConfig>();

        public static void Load()
        {
            RegisterDefaultPinDataViewModels();
            RegisteredNodes.Clear();
            AddDefaultNodes();
            LoadTypesFromConfig();
        }

        #region Default nodes

        private static void AddDefaultNodes()
        {
            RegisteredNodes.Add(new NodeCreationConfig("Load from file", new DefaultNodeSpawnStrategy(new[]
            {
                new NodePinSpawnConfig("Path", typeof(string), string.Empty, true, true)
            }, new[]
            {
                new NodePinSpawnConfig("Mat", typeof(Mat))
            }), typeof(LoadFromFileProcessingStrategy)) {IsPreviewOpened = true});

            RegisteredNodes.Add(new NodeCreationConfig("Save to file", new DefaultNodeSpawnStrategy(new[]
            {
                new NodePinSpawnConfig("Input", typeof(IInputArray)),
                new NodePinSpawnConfig("Path", typeof(string))
            }, null), typeof(SaveToFileProcessingStrategy)) {IsPreviewOpened = true});

            RegisteredNodes.Add(new NodeCreationConfig("Display Result", new DefaultNodeSpawnStrategy(new[]
            {
                new NodePinSpawnConfig("Image", typeof(IInputArray))
            }, null), typeof(DisplayImageProcessingStrategy)) {IsPreviewOpened = true});

            var toImageConverter = new NodeCreationConfig("ToImage", new DefaultNodeSpawnStrategy(new[]
            {
                new NodePinSpawnConfig("Mat", typeof(Mat)),
                new NodePinSpawnConfig("Color", typeof(IColor), typeof(Gray)),
                new NodePinSpawnConfig("Depth", typeof(ValueType), typeof(byte)),
                new NodePinSpawnConfig("TryShareData", typeof(bool), false)
            }, new[]
            {
                new NodePinSpawnConfig("Image", typeof(Image<Gray, byte>))
            }), typeof(ToImageProcessingStrategy));

            RegisteredNodes.Add(toImageConverter);
            Converters.Add(ConverterNodeType.MatToImage, toImageConverter);

            var toMatConverter = new NodeCreationConfig("ToMat", new DefaultNodeSpawnStrategy(new[]
            {
                new NodePinSpawnConfig("src1", typeof(Image<,>))
            }, new[]
            {
                new NodePinSpawnConfig("Result", typeof(Mat))
            }), typeof(ToMatProcessingStrategy));

            RegisteredNodes.Add(toMatConverter);
            Converters.Add(ConverterNodeType.ImageToMat, toMatConverter);

            RegisteredNodes.Add(new NodeCreationConfig("MCvScalar", new DefaultNodeSpawnStrategy(null, new[]
            {
                new NodePinSpawnConfig("MCvScalar", typeof(MCvScalar))
            }), typeof(NewMCvScalarProcessingStrategy)));
        }

        #endregion

        private static void LoadTypesFromConfig()
        {
            if (!File.Exists(REFLECTION_TYPES_CONFIG_PATH))
            {
                File.WriteAllLines(REFLECTION_TYPES_CONFIG_PATH, new []
                {
                    "Emgu.CV.CvInvoke|Emgu.CV",
                    "Emgu.CV.Mat|Emgu.CV",
                    "Emgu.CV.Structure.RangeF|Emgu.CV",
                    "Emgu.CV.Image`2|Emgu.CV",
                    "System.Math|System.Runtime",
                });
            }
            var lines = File.ReadAllLines(REFLECTION_TYPES_CONFIG_PATH);

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith("\\"))
                    continue;

                var splitLine = line.Split('|');

                if (splitLine.Length != 2)
                {
                    Logger.LogError($"Reflection type load config: Cannot parse line '{line}', expecting | separator");
                    continue;
                }

                var typeName = splitLine[0];
                var assemblyName = splitLine[1];
                var type = TypeLoadUtils.FindType(typeName, assemblyName);
                
                if (type == typeof(ErrorLoadingType))
                {
                    Logger.LogError(ThrowHelpers.REFLECTION_TYPE_LOAD, typeName, assemblyName);

                    continue;
                }

                LoadFromType(type, BINDING_FLAGS);
            }
        }

        private static void LoadFromType(Type type, BindingFlags flags)
        {
            var constructors = type.GetConstructors(flags);
            constructors = constructors.OrderBy(x => x.DeclaringType.Name).ThenBy(x => x.GetParameters().Length).ToArray();

            foreach (var constructorInfo in constructors)
            {
                GenerateNodeFromConstructorInfo(constructorInfo);
            }

            var methods = type.GetMethods(flags);
            methods = methods.OrderBy(x => x.ReflectedType.Name).ThenBy(x => x.Name).ThenBy(x => x.GetParameters().Length).ToArray();

            foreach (var methodInfo in methods)
            {
                GenerateNodeFromMethodInfo(methodInfo);
            }

            var properties = type.GetProperties(flags);
            properties = properties.OrderBy(x => x.ReflectedType.Name).ThenBy(x => x.Name).ToArray();

            foreach (var propertyInfo in properties)
            {
                GenerateNodeFromPropertyInfo(propertyInfo);
            }
        }

        private static void GenerateNodeFromMethodInfo(MethodInfo methodInfo)
        {
            var methodName = methodInfo.Name;

            if (methodInfo.IsSpecialName)
            {
                if (methodName.StartsWith("get_") || methodName.StartsWith("set_"))
                    return;

                if (methodName.StartsWith("op_"))
                    methodName = TypeRenamingUtils.RenameOperationMethod(methodName);
            }

            var displayName = methodName;

            var parameters = methodInfo.GetParameters();

            var argsStr = parameters.Select(x => TypeRenamingUtils.RenameType(x.ParameterType)).ToArray();

            if (argsStr.Length > 0)
                displayName = $"{displayName} ({string.Join(", ", argsStr)})";

            displayName += $" {TypeRenamingUtils.RenameType(methodInfo.ReturnType)}";

            var inPinsConfig = new List<NodePinSpawnConfig>();
            var outPinsConfig = new List<NodePinSpawnConfig>();

            foreach (var parameterInfo in parameters)
            {
                var isOut = parameterInfo.ParameterType == typeof(IOutputArray);

                object defaultValue;

                if (parameterInfo.HasDefaultValue)
                    defaultValue = parameterInfo.DefaultValue;
                else
                    defaultValue = ReflectionUtils.GetDefaultValue(parameterInfo.ParameterType);

                inPinsConfig.Add(new NodePinSpawnConfig(
                    parameterInfo.Name,
                    parameterInfo.ParameterType,
                    defaultValue,
                    parameterInfo.HasDefaultValue || isOut,
                    isOut));
            }

            if (!methodInfo.IsStatic)
            {
                inPinsConfig.Insert(0, new NodePinSpawnConfig(
                    TypeRenamingUtils.RenameType(methodInfo.DeclaringType),
                    methodInfo.DeclaringType));
            }

            if (methodInfo.ReturnType != typeof(void))
                outPinsConfig.Add(new NodePinSpawnConfig(TypeRenamingUtils.RenameType(methodInfo.ReturnType), methodInfo.ReturnType));
            else
            {
                var possibleOut = inPinsConfig.FirstOrDefault(x => typeof(IInputArray).IsAssignableFrom(x.PinType));

                if (possibleOut != null)
                {
                    outPinsConfig.Add(new NodePinSpawnConfig(possibleOut.PinName, possibleOut.PinType));
                }
            }

            var nodeSpawnStrategy = new DefaultNodeSpawnStrategy(inPinsConfig.ToArray(), outPinsConfig.ToArray());

            var newConfig = new NodeCreationConfig(methodName, nodeSpawnStrategy, typeof(RuntimeMethodProcessingStrategy), displayName)
            {
                StrategyMethod = methodInfo,
                ReflectionSource = $"{TypeRenamingUtils.RenameType(methodInfo.ReflectedType)}."
            };

            RegisteredNodes.Add(newConfig);
        }

        private static void GenerateNodeFromPropertyInfo(PropertyInfo propertyInfo)
        {
            if (propertyInfo.Name.StartsWith("get_"))
                return;

            var getMethod = propertyInfo.GetGetMethod();

            if (getMethod == null)
                return;

            var propertyName = propertyInfo.Name.Replace("get_", string.Empty);
            var displayName = propertyName;

            displayName += $" {TypeRenamingUtils.RenameType(propertyInfo.PropertyType)}";

            var inPinsConfig = new List<NodePinSpawnConfig>();
            var outPinsConfig = new List<NodePinSpawnConfig>();

            if (!getMethod.IsStatic)
            {
                inPinsConfig.Insert(0, new NodePinSpawnConfig(
                    TypeRenamingUtils.RenameType(propertyInfo.DeclaringType),
                    propertyInfo.DeclaringType));
            }

            outPinsConfig.Add(new NodePinSpawnConfig(TypeRenamingUtils.RenameType(propertyInfo.PropertyType), propertyInfo.PropertyType));

            var nodeSpawnStrategy = new DefaultNodeSpawnStrategy(inPinsConfig.ToArray(), outPinsConfig.ToArray());

            var newConfig = new NodeCreationConfig(propertyName, nodeSpawnStrategy, typeof(RuntimePropertyProcessingStrategy), displayName)
            {
                StrategyProperty = propertyInfo,
                ReflectionSource = $"{TypeRenamingUtils.RenameType(propertyInfo.ReflectedType)}."
            };

            RegisteredNodes.Add(newConfig);
        }

        private static void GenerateNodeFromConstructorInfo(ConstructorInfo constructorInfo)
        {
            var parameters = constructorInfo.GetParameters();

            var argsStr = parameters.Select(x => TypeRenamingUtils.RenameType(x.ParameterType)).ToArray();

            var ctorTypeName = TypeRenamingUtils.RenameType(constructorInfo.DeclaringType);
            var displayName = ctorTypeName;

            if (argsStr.Length > 0)
                displayName = $"{displayName} ({string.Join(", ", argsStr)})";

            var inPinsConfig = new List<NodePinSpawnConfig>();
            var outPinsConfig = new List<NodePinSpawnConfig>();

            foreach (var parameterInfo in parameters)
            {
                var isOut = parameterInfo.ParameterType == typeof(IOutputArray);

                object defaultValue;

                if (parameterInfo.HasDefaultValue)
                    defaultValue = parameterInfo.DefaultValue;
                else
                    defaultValue = ReflectionUtils.GetDefaultValue(parameterInfo.ParameterType);

                inPinsConfig.Add(new NodePinSpawnConfig(
                    parameterInfo.Name,
                    parameterInfo.ParameterType,
                    defaultValue,
                    parameterInfo.HasDefaultValue || isOut,
                    isOut));
            }

            outPinsConfig.Add(new NodePinSpawnConfig(ctorTypeName, constructorInfo.DeclaringType));

            var nodeSpawnStrategy = new DefaultNodeSpawnStrategy(inPinsConfig.ToArray(), outPinsConfig.ToArray());

            var newConfig = new NodeCreationConfig(ctorTypeName, nodeSpawnStrategy, typeof(RuntimeConstructorProcessingStrategy), displayName)
            {
                StrategyCtorType = constructorInfo.DeclaringType,
                ReflectionSource = $"{TypeRenamingUtils.RenameType(constructorInfo.ReflectedType)}."
            };

            RegisteredNodes.Add(newConfig);
        }

        #region Default pin data

        public static readonly Type[] NumericalTypeViewModels =
        {
            typeof(long),
            typeof(ulong),
            typeof(double),
            typeof(int),
            typeof(uint),
            typeof(float),
            typeof(short),
            typeof(ushort)
        };

        private static void RegisterDefaultPinDataViewModels()
        {
            foreach (var numericalTypeViewModel in NumericalTypeViewModels)
            {
                var generic = typeof(NumericalPinVisualDataProcessor<>).MakeGenericType(numericalTypeViewModel);
                _registeredPinDataViewModels.Add(numericalTypeViewModel, generic);
            }

            _registeredPinDataViewModels.Add(typeof(Enum), typeof(EnumPinVisualDataProcessor));
            _registeredPinDataViewModels.Add(typeof(Size), typeof(SizePinVisualDataProcessor));
            _registeredPinDataViewModels.Add(typeof(Point), typeof(PointPinVisualDataProcessor));
            _registeredPinDataViewModels.Add(typeof(string), typeof(SelectFilePathVisualDataProcessor));
            _registeredPinDataViewModels.Add(typeof(bool), typeof(BoolPinVisualDataProcessor));
            _registeredPinDataViewModels.Add(typeof(IColor), typeof(TypeSelectorVisualDataProcessor));
            _registeredPinDataViewModels.Add(typeof(ValueType), typeof(TypeSelectorVisualDataProcessor));
        }

        [CanBeNull]
        public static Type GetRegisteredPinDataViewModelForType([NotNull] Type type)
        {
            if (type.IsEnum)
                type = typeof(Enum);

            _registeredPinDataViewModels.TryGetValue(type, out var model);
            return model;
        }

        #endregion
    }
}
