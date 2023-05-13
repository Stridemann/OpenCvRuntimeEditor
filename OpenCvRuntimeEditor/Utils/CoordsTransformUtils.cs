namespace OpenCvRuntimeEditor.Utils
{
    using System.Windows;
    using Controls;

    public static class CoordsTransformUtils
    {
        public static Point CanvasToGlobal(Point pos)
        {
            var start = new Point(pos.X * ZoomBorder.Instance.ScaleX, pos.Y * ZoomBorder.Instance.ScaleY);
            return start + ZoomBorder.Instance.AbsoluteOffset;
        }

        public static Point GlobalToCanvas(Point pos)
        {       
            var offset = new Vector(
            ZoomBorder.Instance.AbsoluteOffset.X / ZoomBorder.Instance.ScaleX,
            ZoomBorder.Instance.AbsoluteOffset.Y / ZoomBorder.Instance.ScaleY);
            pos = new Point(pos.X / ZoomBorder.Instance.ScaleX, pos.Y / ZoomBorder.Instance.ScaleY);
            return pos - offset;
        }
    }
}
