namespace OpenCvRuntimeEditor.ViewModels.PinDataVisualProcessors
{
    using System;
    using System.Collections.ObjectModel;
    using Base;
    using Core;
    using Emgu.CV.Structure;

    public class TypeSelectorVisualDataProcessor : BasePinVisualDataProcessor
    {
        private Type _value;

        public TypeSelectorVisualDataProcessor(IRuntimeDataContainer dataContainer) : base(dataContainer)
        {
            Value = dataContainer.GetData<Type>();

            VariantTypes = new ObservableCollection<Type>
            {
                typeof(Bgr),
                typeof(Bgr565),
                typeof(Bgra),
                typeof(Gray),
                typeof(Hls),
                typeof(Hsv),
                typeof(Lab),
                typeof(Luv),
                typeof(Rgb),
                typeof(Rgba),
                typeof(Xyz),
                typeof(Ycc),

                typeof(byte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(float),
                typeof(double),
                typeof(long),
                typeof(ulong)
            };
        }

        public ObservableCollection<Type> VariantTypes { get; set; }

        public Type Value
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
