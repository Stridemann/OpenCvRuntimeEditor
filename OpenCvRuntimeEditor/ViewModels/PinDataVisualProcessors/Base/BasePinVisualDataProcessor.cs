namespace OpenCvRuntimeEditor.ViewModels.PinDataVisualProcessors.Base
{
    using Core;

    public abstract class BasePinVisualDataProcessor : BaseViewModel
    {
        protected BasePinVisualDataProcessor(IRuntimeDataContainer dataContainer)
        {
            DataContainer = dataContainer;
        }

        public IRuntimeDataContainer DataContainer { get; }
    }
}
