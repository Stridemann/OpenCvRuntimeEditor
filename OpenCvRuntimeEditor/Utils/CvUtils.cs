namespace OpenCvRuntimeEditor.Utils
{
    using Emgu.CV;

    public static class CvUtils
    {
        public static Mat MatFromImage(object image)
        {
            var matProperty = image.GetType().GetProperty("Mat");
            var matValue = (Mat) matProperty.GetValue(image);
            return matValue;
        }

        public static object CloneObj(object image)
        {
            if (image == null)
                return null;

            if (image is Mat mat)
            {
                var clonedMat = mat.Clone();

                return clonedMat;
            }

            var type = image.GetType();

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Image<,>))
            {
                var cloneMethod = type.GetMethod("Clone");
                return cloneMethod.Invoke(image, new object[0]);
            }

            return image;
        }
    }
}
