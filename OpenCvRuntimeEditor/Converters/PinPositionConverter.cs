namespace OpenCvRuntimeEditor.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class PinPositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is Button pinButton && values[1] is Point nodePos /*&& values[3] is double nodeWidth*/)
            {
                var root = MainWindow.Instance.NodesRoot;
                try//this is only to ignore VS designer exceptions
                {
                    var relativePoint = pinButton.TransformToAncestor(root)
                        .Transform(new Point(pinButton.ActualWidth / 2, pinButton.ActualHeight / 2));

                    return relativePoint;
                }
                catch
                {
                    return new Point(0, 0);
                }
            }

            return new Point(0, 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
