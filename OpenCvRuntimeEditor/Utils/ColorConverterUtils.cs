namespace OpenCvRuntimeEditor.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;
    using Emgu.CV;
    using Emgu.CV.Structure;

    public static class ColorConverterUtils
    {
        private static readonly Dictionary<Type, Brush> _cachedTypeColors = new Dictionary<Type, Brush>();

        static ColorConverterUtils()
        {
            _cachedTypeColors.Add(typeof(Mat), Brushes.Wheat);
            _cachedTypeColors.Add(typeof(Image<Gray, byte>), Brushes.DarkGray);
        }

        public static Brush BrushFromType(Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
                type = type.GetGenericTypeDefinition();

            if (_cachedTypeColors.TryGetValue(type, out var brush))
            {
                return brush;
            }

            var typeHashCodeAbs = Math.Abs(type.GetHashCode());
            var colorParms = new byte[3];

            colorParms[0] = (byte)(typeHashCodeAbs & 0xff);
            colorParms[1] = (byte)((typeHashCodeAbs & 0xff00) >> 8);
            colorParms[2] = (byte)((typeHashCodeAbs & 0xff0000) >> 16);

            var color = Color.FromRgb(colorParms[0], colorParms[1], colorParms[2]);
            brush = new SolidColorBrush(color);
            _cachedTypeColors.Add(type, brush);
            return brush;
        }
    }
}
