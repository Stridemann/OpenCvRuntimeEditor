namespace OpenCvRuntimeEditor.Core.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Newtonsoft.Json;
    using ProcessingStrategies;
    using Utils;
    using ViewModels;

    internal class NodeSaveData
    {
        [JsonConstructor]
        public NodeSaveData()
        {
        }

        public NodeSaveData(NodeViewModel node, CanvasSaveData canvasSaveData)
        {
            Pos = node.Pos;
            Name = node.Name;
            StrategyTypeCacheIndex = canvasSaveData.TypeSerializationContainer.GetSerializedTypeIndex(node.ProcessingStrategy.GetType());
            Id = node.Id;
            TemplateKey = node.TemplateKey;
            VariableRefId = node.VariableRefId;
            IsPreviewOpened = node.IsPreviewOpened;

            if (node.PipelineData.StrategyMethod != null)
                StrategyMethodSerializationData = canvasSaveData.TypeSerializationContainer.SerializeMethod(node.PipelineData.StrategyMethod);

            if (node.PipelineData.StrategyProperty != null)
                StrategyPropertySerializationData = canvasSaveData.TypeSerializationContainer.SerializeProperty(node.PipelineData.StrategyProperty);

            if (node.PipelineData.StrategyConstructorType != null)
                SerializedConstructorStrategyTypeIndex =
                    canvasSaveData.TypeSerializationContainer.GetSerializedTypeIndex(node.PipelineData.StrategyConstructorType);

            node.InPins.Foreach(x => InPinsData.Add(new PinSaveData(x, canvasSaveData)));
            node.OutPins.Foreach(x => OutPinsData.Add(new PinSaveData(x, canvasSaveData)));
        }

        public Point Pos { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string TemplateKey { get; set; }
        public int StrategyTypeCacheIndex { get; set; }
        public SerializedMethod StrategyMethodSerializationData { get; set; }
        public SerializedProperty StrategyPropertySerializationData { get; set; }
        public int SerializedConstructorStrategyTypeIndex { get; set; } = -1;
        public List<PinSaveData> InPinsData { get; set; } = new List<PinSaveData>();
        public List<PinSaveData> OutPinsData { get; set; } = new List<PinSaveData>();
        public bool IsPreviewOpened { get; set; }
        public int VariableRefId { get; set; }

        public void Setup(NodeViewModel node, CanvasSaveData canvasSaveData)
        {
            node.Pos = Pos;
            node.Name = Name;
            node.Id = Id;
            node.TemplateKey = TemplateKey;
            node.VariableRefId = VariableRefId;
            node.IsPreviewOpened = IsPreviewOpened;

            var strategyType = canvasSaveData.TypeSerializationContainer.GetTypeByIndex(StrategyTypeCacheIndex);

            if (strategyType == null)
                node.ErrorInfo.AddError("Can't load type for ProcessingStrategy. (type load error already thrown)", ErrorInfoType.Loading);
            else
                node.ProcessingStrategy = (IProcessingStrategy) Activator.CreateInstance(strategyType);

            if (StrategyMethodSerializationData != null)
            {
                var method = canvasSaveData.TypeSerializationContainer.DeserializeMethod(StrategyMethodSerializationData);

                if (method == null)
                {
                    node.ErrorInfo.AddError(
                        $"Can't load MethodInfo for StrategyMethod: method '{StrategyMethodSerializationData.MethodName}' is not found.",
                        ErrorInfoType.Loading);
                }
                else
                    node.PipelineData.StrategyMethod = method;
            }

            if (StrategyPropertySerializationData != null)
            {
                var property = canvasSaveData.TypeSerializationContainer.DeserializeProperty(StrategyPropertySerializationData);

                if (property == null)
                {
                    node.ErrorInfo.AddError(
                        $"Can't load PropertyInfo for StrategyProperty: property '{StrategyPropertySerializationData.PropertyName}' is not found.",
                        ErrorInfoType.Loading);
                }
                else
                    node.PipelineData.StrategyProperty = property;
            }

            if (SerializedConstructorStrategyTypeIndex != -1)
            {
                node.PipelineData.StrategyConstructorType =
                    canvasSaveData.TypeSerializationContainer.GetTypeByIndex(SerializedConstructorStrategyTypeIndex);
            }

            foreach (var pin in InPinsData)
            {
                var newPin = new NodePinViewModel
                {
                    Direction = PinDirection.In,
                    Owner = node
                };

                pin.Setup(newPin, canvasSaveData);
                node.InPins.Add(newPin);
            }

            foreach (var pin in OutPinsData)
            {
                var newPin = new NodePinViewModel
                {
                    Direction = PinDirection.Out,
                    Owner = node
                };

                pin.Setup(newPin, canvasSaveData);
                node.OutPins.Add(newPin);
            }
        }
    }
}
