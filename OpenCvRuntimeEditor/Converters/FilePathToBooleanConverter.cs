namespace OpenCvRuntimeEditor.Converters
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows.Data;

    public class FilePathToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path)
            {
                return File.Exists(path);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
