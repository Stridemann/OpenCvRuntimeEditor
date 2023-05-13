namespace OpenCvRuntimeEditor.Core
{
    using System;
    using System.Reflection;
    using System.Windows;
    using Emgu.CV;
    using NodeSpawn;
    using ProcessingStrategies;
    using ViewModels;
    using ViewModels.PinDataVisualProcessors.Base;

    public static class NodeFactory
    {
        internal static NodeViewModel CreateNode(NodeCreationConfig config, int id)
        {
            var vewModel = config.SpawnStraregy.Spawn(config);
            vewModel.Id = id;
            vewModel.PipelineData.StrategyMethod = config.StrategyMethod;
            vewModel.PipelineData.StrategyProperty = config.StrategyProperty;
            vewModel.PipelineData.StrategyConstructorType = config.StrategyCtorType;
            vewModel.VariableRefId = config.VariableRefId;

            var processingStrategy = (IProcessingStrategy) Activator.CreateInstance(config.ProcessingStrategyType);
            vewModel.ProcessingStrategy = processingStrategy;

            foreach (var pin in vewModel.InPins)
            {
                var pinDataViewModelType = NodeDatabase.GetRegisteredPinDataViewModelForType(pin.PinType);

                if (pinDataViewModelType != null)
                {
                    var defaultConstructor = pinDataViewModelType.GetConstructor(new[] {typeof(PinData)});

                    if (defaultConstructor == null)
                    {
                        pin.ErrorInfo.AddError("Can't find match constructor for PinVisualDataProcessor, expecting .ctor(PinData data)",
                            ErrorInfoType.Core);

                        pin.IsOptional = false;
                    }
                    else
                    {
                        var pinDataViewModelInstance = (BasePinVisualDataProcessor) Activator.CreateInstance(pinDataViewModelType, pin.PinData);
                        pin.IsOptional = true;
                        pin.VisualDataProcessor = pinDataViewModelInstance;
                    }
                }

                if (pin.PinType == typeof(IOutputArray))
                {
                    vewModel.OutPins.Add(new NodePinViewModel(pin.Name, pin.PinType, PinDirection.Out, vewModel));
                    pin.IsVisible = false;
                }
            }

            return vewModel;
        }
    }

    public class NodeCreationConfig : BaseViewModel
    {
        private Visibility _isSelectedInNodesList = Visibility.Collapsed;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCreationConfig" /> class.
        /// </summary>
        /// <param name="nodeName">Name of the node.</param>
        /// <param name="spawnStraregy">The spawn straregy.</param>
        /// <param name="processingStrategyType">
        /// Type of the processing strategy (class derived from IProcessingStrategy with
        /// parametreless constructor).
        /// </param>
        /// <param name="displayName">How it will be displayed in nodes list window</param>
        /// <param name="templateKey">The template key. Null mean default</param>
        public NodeCreationConfig(string nodeName, DefaultNodeSpawnStrategy spawnStraregy, Type processingStrategyType, string displayName = null,
            string templateKey = null)
        {
            NodeName = nodeName;
            FilterUpperName = nodeName.ToUpper();
            SpawnStraregy = spawnStraregy;
            ProcessingStrategyType = processingStrategyType;
            DisplayName = displayName ?? nodeName;
            TemplateKey = templateKey ?? "NodeDefaultTemplate";
        }

        public string NodeName { get; }
        public Type ProcessingStrategyType { get; }
        public DefaultNodeSpawnStrategy SpawnStraregy { get; }
        public string TemplateKey { get; }
        internal MethodInfo StrategyMethod { get; set; }
        internal PropertyInfo StrategyProperty { get; set; }
        internal Type StrategyCtorType { get; set; }
        public bool IsPreviewOpened { get; set; }
        public int VariableRefId { get; set; } = -1;

        #region NodesWindow

        internal string FilterUpperName { get; }
        public string DisplayName { get; }
        public string ReflectionSource { get; set; }

        public Visibility IsSelectedInNodesList
        {
            get => _isSelectedInNodesList;
            set => SetProperty(ref _isSelectedInNodesList, value);
        }

        #endregion
    }
}
