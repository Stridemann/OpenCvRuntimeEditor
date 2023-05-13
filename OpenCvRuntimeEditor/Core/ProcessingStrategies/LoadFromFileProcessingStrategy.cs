namespace OpenCvRuntimeEditor.Core.ProcessingStrategies
{
    using System;
    using System.IO;
    using Emgu.CV;
    using ViewModels;

    public class LoadFromFileProcessingStrategy : IProcessingStrategy
    {
        public void Process(NodeViewModel node)
        {
            try
            {
                var path = node.GetInPinData<string>(0);

                if (!File.Exists(path))
                {
                    node.PipelineData.CalculationSuccessfull = false;
                    node.ErrorInfo.AddPipelineError("File not found");
                    node.PipelineData.ClearPreviewImage();
                    return;
                }

                //var image = Image.FromFile(path);
                //var bitmap = new Bitmap(image);
                
                //var imageCv = new Image<Bgr, byte>(bitmap);
                //CvInvoke.Normalize(imageCv, imageCv, 0, 255, NormType.MinMax);

                var loadedMat = CvInvoke.Imread(path);
                node.SetOutPinData(0, loadedMat);
                node.PipelineData.UpdatePreviewImage(loadedMat);
            }
            catch (Exception e)
            {
                node.PipelineData.CalculationSuccessfull = false;
                node.ErrorInfo.AddPipelineError(e.Message);
                node.PipelineData.ClearPreviewImage();
            }
        }
    }
}
