namespace OpenCvRuntimeEditor.Core.ProcessingStrategies
{
    using ViewModels;

    public interface IProcessingStrategy
    {
        void Process(NodeViewModel node);
    }
}
