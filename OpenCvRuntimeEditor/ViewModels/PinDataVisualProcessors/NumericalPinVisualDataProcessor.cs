namespace OpenCvRuntimeEditor.ViewModels.PinDataVisualProcessors
{
    using Base;
    using Core;

    public class NumericalPinVisualDataProcessor<T> : BasePinVisualDataProcessor where T : struct
    {
        private double _max = 100;
        private double _min;
        private T _value;

        public NumericalPinVisualDataProcessor(IRuntimeDataContainer dataContainer) : base(dataContainer)
        {
            var data = DataContainer.GetData<object>();

            if (data is T value)
                Value = value;

            Min = 0;
            Max = 200;
        }

        public double Min
        {
            get => _min;
            set => SetProperty(ref _min, value);
        }

        public double Max
        {
            get => _max;
            set => SetProperty(ref _max, value);
        }

        public T Value
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
