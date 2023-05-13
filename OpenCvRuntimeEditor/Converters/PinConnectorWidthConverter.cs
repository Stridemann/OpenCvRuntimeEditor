namespace OpenCvRuntimeEditor.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class PinConnectorWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int count)
                return count > 0 ? 0d : 10d;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
