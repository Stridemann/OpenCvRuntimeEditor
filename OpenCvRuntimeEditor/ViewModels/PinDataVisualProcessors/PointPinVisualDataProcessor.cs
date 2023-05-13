namespace OpenCvRuntimeEditor.ViewModels.PinDataVisualProcessors
{
    using System.Drawing;
    using Base;
    using Core;

    public class PointPinVisualDataProcessor : BasePinVisualDataProcessor
    {
        private int _x;
        private int _y;

        public PointPinVisualDataProcessor(IRuntimeDataContainer dataContainer) : base(dataContainer)
        {
            var data = dataContainer.GetData<object>();

            if (data is Point point)
                Value = point;
        }

        public Point Value
        {
            get => new Point(_x, _y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public int X
        {
            get => _x;
            set
            {
                SetProperty(ref _x, value);
                DataContainer.SetData(Value, true);
            }
        }

        public int Y
        {
            get => _y;
            set
            {
                SetProperty(ref _y, value);
                DataContainer.SetData(Value, true);
            }
        }
    }
}
