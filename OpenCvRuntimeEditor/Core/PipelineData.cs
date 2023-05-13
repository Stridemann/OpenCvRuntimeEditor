namespace OpenCvRuntimeEditor.Core
{
    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Windows.Media;
    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Structure;
    using Utils;
    using ViewModels;

    public class PipelineData : BaseViewModel
    {
        private readonly NodeViewModel _owner;
        private bool _calculationSuccessfull;
        private ImageSource _previewImage;

        public PipelineData(NodeViewModel owner)
        {
            _owner = owner;
        }

        internal Type StrategyConstructorType { get; set; }
        internal MethodInfo StrategyMethod { get; set; }
        internal PropertyInfo StrategyProperty { get; set; }

        public ImageSource PreviewImage
        {
            get => _previewImage;
            set => SetProperty(ref _previewImage, value);
        }

        /// <summary>
        /// Marking nodes which was copied/pasted. Need to do recalculation
        /// </summary>
        public bool NeedRecalculate { get; set; }
        /// <summary>
        /// Used for detecting circular dependency
        /// </summary>
        public bool IsProcessing { get; set; }

        public bool CalculationSuccessfull
        {
            get => _calculationSuccessfull;
            set => SetProperty(ref _calculationSuccessfull, value);
        }

        public void UpdatePreviewImage(object obj)
        {
            if (obj == null)
            {
                PreviewImage = null;
                return;
            }

            if (obj is Mat mat)
            {
                UpdatePreviewImage(mat);
                return;
            }

            if (obj is Bitmap bitmap)
            {
                UpdatePreviewImage(bitmap);
                return;
            }

            var objType = obj.GetType();

            if (objType.IsGenericType)
            {
                if (objType.GetGenericTypeDefinition() == typeof(Image<,>))
                {
                    var matValue = CvUtils.MatFromImage(obj);
                    UpdatePreviewImage(matValue);
                    return;
                }
            }

            //here we can get any type at all
            //_owner.ErrorInfo.AddPipelineError($"Updating preview for {obj.GetType().Name} type is not supported. \r\n" +
            //                                  $"Convert to Bitmap manually and use another overload {nameof(UpdatePreviewImage)}(Bitmap bitmap)");
        }

        public void UpdatePreviewImage(Mat mat)
        {
            //var resultMat = new Mat();
            //mat.ConvertTo(resultMat, DepthType.Cv8U);

            var displayResult = new Mat();
            CvInvoke.Normalize(mat, displayResult, 0, byte.MaxValue, NormType.MinMax, DepthType.Cv8U);


            var bitmap = displayResult.ToBitmap();

            if (bitmap == null)
            {
                bitmap = mat.ToImage<Gray, byte>().Mat.ToBitmap();
            }
            UpdatePreviewImage(bitmap);
        }

        public void ClearPreviewImage()
        {
            PreviewImage = null;
        }

        public void UpdatePreviewImage(Image<Gray, byte> image)
        {
            UpdatePreviewImage(image.Mat.ToBitmap());
        }

        public void UpdatePreviewImage(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                ClearPreviewImage();

                return;
            }
            PreviewImage = PipelineUtils.BitmapToImageSource(bitmap);
        }
    }
}
