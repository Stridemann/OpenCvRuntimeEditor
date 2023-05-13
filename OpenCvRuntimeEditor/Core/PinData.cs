namespace OpenCvRuntimeEditor.Core
{
    using ViewModels;

    public class PinData : IRuntimeDataContainer
    {
        private object _data;

        public PinData(NodePinViewModel ownerPin)
        {
            OwnerPin = ownerPin;
        }

        public NodePinViewModel OwnerPin { get; }

        public T GetData<T>()
        {
            if (OwnerPin.Direction == PinDirection.In && OwnerPin.IsLinked)
            {
                return OwnerPin.Links[0].OutPin.PinData.GetData<T>();
            }

            if (_data == null)
                return default;

            return (T) _data;
        }

        public void SetData(object data, bool recalculate = false)
        {
            _data = data;

            if (recalculate)
                Pipeline.Recalculate(OwnerPin.Owner);
        }
    }
}
