namespace OpenCvRuntimeEditor.ViewModels.PinDataVisualProcessors
{
    using Base;
    using Core;

    public class BoolPinVisualDataProcessor : BasePinVisualDataProcessor
    {
        private bool _value;

        public BoolPinVisualDataProcessor(IRuntimeDataContainer dataContainer) : base(dataContainer)
        {
            if (dataContainer.GetData<object>() is bool value)
                _value = value;
        }

        public bool Value
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
