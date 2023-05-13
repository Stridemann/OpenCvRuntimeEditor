namespace OpenCvRuntimeEditor.Converters
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Data;
    using Utils;

    public class TypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Type type)
            {
                return ColorConverterUtils.BrushFromType(type);
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
