namespace OpenCvRuntimeEditor.Core.Serialization
{
    using System;
    using JsonConverters;
    using Newtonsoft.Json;
    using Utils;
    using ViewModels;
    using ViewModels.PinDataVisualProcessors.Base;

    internal class PinSaveData
    {
        [JsonConstructor]
        public PinSaveData()
        {
        }

        public PinSaveData(NodePinViewModel pin, CanvasSaveData canvasSaveData)
        {
            Name = pin.Name;
            IsOptional = pin.IsOptional;
            IsVisible = pin.IsVisible;
            PinTypeCachedIndex = canvasSaveData.TypeSerializationContainer.GetSerializedTypeIndex(pin.PinType);

            if (pin.Direction == PinDirection.In)
                pin.Links.Foreach(x => canvasSaveData.Links.Add(new PinLinkSaveData(x)));

            if (pin.VisualDataProcessor != null)
                VisualPinObjectData = new JsoSerializableObjectContainer {Data = pin.PinData.GetData<object>()};
        }

        public void Setup(NodePinViewModel pin, CanvasSaveData canvasSaveData)
        {
            pin.Name = Name;
            pin.IsOptional = IsOptional;
            pin.IsVisible = IsVisible;
            pin.PinType = canvasSaveData.TypeSerializationContainer.GetTypeByIndex(PinTypeCachedIndex);

            if (VisualPinObjectData != null)
                pin.PinData.SetData(VisualPinObjectData.Data);

            if (pin.PinType != typeof(ErrorLoadingType))
            {
                var pinDataViewModelType = NodeDatabase.GetRegisteredPinDataViewModelForType(pin.PinType);

                if (pinDataViewModelType != null)
                    pin.VisualDataProcessor = (BasePinVisualDataProcessor) Activator.CreateInstance(pinDataViewModelType, pin.PinData);
            }
        }

        public string Name { get; set; }
        public bool IsOptional { get; set; }
        public bool IsVisible { get; set; }
        public int PinTypeCachedIndex { get; set; } = -1;
        [JsonConverter(typeof(ObjectJsonConverter))]
        public JsoSerializableObjectContainer VisualPinObjectData { get; set; }
    }
}
