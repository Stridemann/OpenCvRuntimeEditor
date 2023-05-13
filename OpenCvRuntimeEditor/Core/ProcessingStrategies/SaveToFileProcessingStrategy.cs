namespace OpenCvRuntimeEditor.Core.ProcessingStrategies
{
    using System.IO;
    using Emgu.CV;
    using Utils;
    using ViewModels;

    public class SaveToFileProcessingStrategy : IProcessingStrategy
    {
        public void Process(NodeViewModel node)
        {
            var input = node.GetInPinData<IInputArray>(0);
            var inputType = input.GetType();

            Mat mat = null;

            if (inputType.IsGenericType)
            {
                if (inputType.GetGenericTypeDefinition() == typeof(Image<,>))
                {
                    mat = CvUtils.MatFromImage(input);
                }
            }
            else if (inputType == typeof(Mat))
            {
                mat = input as Mat;
            }

            if (mat == null)
            {
                node.ErrorInfo.AddPipelineError($"Saving to file is not supported for type '{TypeRenamingUtils.RenameType(inputType)}'");
                return;
            }

            var path = node.GetInPinData<string>(1);
            path = Path.GetFullPath(path);

            var directoryName = Path.GetDirectoryName(path);

            if (!Directory.Exists(directoryName))
            {
                node.ErrorInfo.AddPipelineError($"Directory '{directoryName}' doesnt exist");
                return;
            }

            if (Path.GetExtension(path) != ".png")
            {
                node.ErrorInfo.AddPipelineError("Expecting .png file extension");
                return;
            }

            mat.Save(path);
        }
    }
}
