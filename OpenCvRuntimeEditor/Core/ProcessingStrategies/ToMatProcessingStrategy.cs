namespace OpenCvRuntimeEditor.Core.ProcessingStrategies
{
    using ViewModels;

    public class ToMatProcessingStrategy : IProcessingStrategy
    {
        public void Process(NodeViewModel node)
        {
            var image = node.GetInPinData<object>(0);
            var matProperty = image.GetType().GetProperty("Mat");
            var invokeMethod = matProperty.GetGetMethod();
            var mat = invokeMethod.Invoke(image, null);
            node.SetOutPinData(0, mat);
            node.PipelineData.UpdatePreviewImage(mat);
        }
    }
}
