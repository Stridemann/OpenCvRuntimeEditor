namespace OpenCvRuntimeEditor.Core.ProcessingStrategies
{
    using System;
    using System.Reflection;
    using Emgu.CV;
    using ViewModels;

    public class ToImageProcessingStrategy : IProcessingStrategy
    {
        private static MethodInfo _toImageGenericMethod;

        static ToImageProcessingStrategy()
        {
            _toImageGenericMethod = typeof(Mat).GetMethod("ToImage");
        }

        public void Process(NodeViewModel node)
        {
            var mat = node.GetInPinData<Mat>(0);
            var genericArg1 = node.GetInPinData<Type>(1);
            var genericArg2 = node.GetInPinData<Type>(2);
            var shareData = node.GetInPinData<bool>(3);

            var genericMethod = _toImageGenericMethod.MakeGenericMethod(genericArg1, genericArg2);
            var image = genericMethod.Invoke(mat, new object[]{shareData});
            node.SetOutPinData(0, image);
            node.OutPins[0].PinType = image.GetType();
            node.PipelineData.UpdatePreviewImage(image);
        }
    }
}
