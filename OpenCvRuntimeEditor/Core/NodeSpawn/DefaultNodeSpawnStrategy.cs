namespace OpenCvRuntimeEditor.Core.NodeSpawn
{
    using System;
    using ViewModels;

    public class DefaultNodeSpawnStrategy
    {
        public NodePinSpawnConfig[] InPinSpawnConfigs { get; }
        public NodePinSpawnConfig[] OutPinSpawnConfigs { get; }

        public DefaultNodeSpawnStrategy(NodePinSpawnConfig[] inPinSpawnConfigs, NodePinSpawnConfig[] outPinSpawnConfigs)
        {
            InPinSpawnConfigs = inPinSpawnConfigs;
            OutPinSpawnConfigs = outPinSpawnConfigs;
        }

        public NodeViewModel Spawn(NodeCreationConfig config)
        {
            var newNodeViewModel = new NodeViewModel(config);

            if (InPinSpawnConfigs != null)
            {
                foreach (var pinConfig in InPinSpawnConfigs)
                {
                    var pin = new NodePinViewModel(pinConfig.PinName, pinConfig.PinType, PinDirection.In, newNodeViewModel, pinConfig.IsOptional);
                    pin.PinData.SetData(pinConfig.DefaultValue);
                    newNodeViewModel.InPins.Add(pin);
                }
            }

            if (OutPinSpawnConfigs != null)
            {
                foreach (var pinConfig in OutPinSpawnConfigs)
                {
                    newNodeViewModel.OutPins.Add(new NodePinViewModel(pinConfig.PinName, pinConfig.PinType, PinDirection.Out, newNodeViewModel));
                }
            }

            return newNodeViewModel;
        }
    }

    public class NodePinSpawnConfig
    {
        public NodePinSpawnConfig(string pinName, Type pinType, object defaultValue = null, bool isOptional = false, bool cloneToOutPin = false)
        {
            PinName = pinName;
            PinType = pinType;
            CloneToOutPin = cloneToOutPin;
            DefaultValue = defaultValue;
            IsOptional = isOptional;
        }

        public string PinName { get; }
        public Type PinType { get; }
        public object DefaultValue { get; }
        public bool IsOptional { get; }
        public bool CloneToOutPin { get; }
    }
}
