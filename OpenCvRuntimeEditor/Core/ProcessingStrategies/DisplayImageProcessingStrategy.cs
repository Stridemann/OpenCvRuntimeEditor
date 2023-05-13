namespace OpenCvRuntimeEditor.Core.ProcessingStrategies
{
    using Emgu.CV;
    using ViewModels;

    public class DisplayImageProcessingStrategy : IProcessingStrategy
    {
        public void Process(NodeViewModel node)
        {
            var src = node.GetInPinData<IInputArray>(0);
            node.PipelineData.UpdatePreviewImage(src);
        }
    }
}
