namespace OpenCvRuntimeEditor.ViewModels.PinDataVisualProcessors
{
    using System.Drawing;
    using Base;
    using Core;

    public class SizePinVisualDataProcessor : BasePinVisualDataProcessor
    {
        private int _width;
        private int _height;

        public SizePinVisualDataProcessor(IRuntimeDataContainer dataContainer) : base(dataContainer)
        {
            var data = dataContainer.GetData<object>();

            if (data is Size size)
                Value = size;
        }

        public Size Value
        {
            get => new Size(_width, _height);
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        public int Width
        {
            get => _width;
            set
            {
                SetProperty(ref _width, value);
                DataContainer.SetData(Value, true);
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                SetProperty(ref _height, value);
                DataContainer.SetData(Value, true);
            }
        }
    }
}
