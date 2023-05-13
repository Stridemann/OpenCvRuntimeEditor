namespace OpenCvRuntimeEditor.Core.Serialization
{
    using System;
    using JsonConverters;
    using Newtonsoft.Json;
    using ViewModels;
    using ViewModels.PinDataVisualProcessors.Base;

    internal class VariableSaveData
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int TypeCachedIndex { get; set; }
        [JsonConverter(typeof(ObjectJsonConverter))]
        public JsoSerializableObjectContainer DataContainer { get; set; }

        [JsonConstructor]
        public VariableSaveData()
        {
        }

        public VariableSaveData(VariableViewModel variable, CanvasSaveData canvasSaveData)
        {
            Name = variable.Name;
            Id = variable.Id;
            TypeCachedIndex = canvasSaveData.TypeSerializationContainer.GetSerializedTypeIndex(variable.Type);
            if(variable.Data != null)
                DataContainer = new JsoSerializableObjectContainer{Data = variable.Data};
        }

        public void Setup(VariableViewModel variable, CanvasSaveData canvasSaveData)
        {
            variable.Name = Name;
            variable.Id = Id;
            variable.Type = canvasSaveData.TypeSerializationContainer.GetTypeByIndex(TypeCachedIndex);
            variable.Data = DataContainer.Data;

            if (variable.Type != typeof(ErrorLoadingType))
            {
                var pinDataViewModelType = NodeDatabase.GetRegisteredPinDataViewModelForType(variable.Type);
                if (pinDataViewModelType != null)
                    variable.VisualDataProcessor = (BasePinVisualDataProcessor) Activator.CreateInstance(pinDataViewModelType, variable);
            }
        }
    }
}
