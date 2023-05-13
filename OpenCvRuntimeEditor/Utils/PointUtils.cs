namespace OpenCvRuntimeEditor.Utils
{
    using System.Windows;

    public static class PointUtils
    {
        public static Point GetMidPoint(Point p1, Point p2, double offsetX)
        {
            var converterNodePos = new Point((p1.X + p2.X) / 2d, (p1.Y + p2.Y) / 2d);
            return converterNodePos + new Vector(offsetX, 0);
        }
    }
}
