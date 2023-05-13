namespace OpenCvRuntimeEditor.ViewModels.PinDataVisualProcessors
{
    using System;
    using Base;
    using Core;

    public class EnumPinVisualDataProcessor : BasePinVisualDataProcessor
    {
        private Enum _value;

        public EnumPinVisualDataProcessor(IRuntimeDataContainer dataContainer) : base(dataContainer)
        {
            var defValue = dataContainer.GetData<object>();
            if (defValue is Enum value)
            {
                _value = value;
                EnumValues = Enum.GetValues(value.GetType());
            }
        }

        public Array EnumValues { get; }

        public Enum Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
                DataContainer.SetData(value, true);
            }
        }
    }
}
