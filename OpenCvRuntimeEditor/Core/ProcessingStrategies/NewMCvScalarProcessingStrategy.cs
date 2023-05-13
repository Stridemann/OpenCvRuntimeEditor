namespace OpenCvRuntimeEditor.Core.ProcessingStrategies
{
    using Emgu.CV.Structure;
    using ViewModels;

    public class NewMCvScalarProcessingStrategy : IProcessingStrategy
    {
        public void Process(NodeViewModel node)
        {
            var scalar = new MCvScalar();
            node.SetOutPinData(0, scalar);
            node.PipelineData.UpdatePreviewImage((object)null);
        }
    }
}
