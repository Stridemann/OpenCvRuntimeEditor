namespace OpenCvRuntimeEditor.ViewModels.PinDataVisualProcessors
{
    using Base;
    using Core;

    public class StringPinVisualDataProcessor : BasePinVisualDataProcessor
    {
        private string _value;

        public StringPinVisualDataProcessor(IRuntimeDataContainer dataContainer) : base(dataContainer)
        {
            _value = dataContainer.GetData<string>();
        }

        public string Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
                DataContainer.SetData(Value, true);
            }
        }
    }
}
