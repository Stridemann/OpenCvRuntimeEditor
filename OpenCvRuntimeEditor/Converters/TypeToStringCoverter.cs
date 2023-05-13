namespace OpenCvRuntimeEditor.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Utils;

    public class TypeToStringCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Type type)
                return TypeRenamingUtils.RenameType(type);

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
