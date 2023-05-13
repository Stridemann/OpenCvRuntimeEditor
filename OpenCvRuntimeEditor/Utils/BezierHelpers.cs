namespace OpenCvRuntimeEditor.Utils
{
    using System.Windows;

    public static class BezierHelpers
    {
        public static unsafe Point[] GenerateBasierCurve(Point pinStart, Point pinEnd)
        {
            var bezierTangentLength = Point.Subtract(pinStart, pinEnd).Length / 3;

            var points = new Point[4];
            fixed (void* ptr = &points[0])
            {
                var iteratorPtr = (double*)ptr;
                *iteratorPtr++ = pinStart.X + bezierTangentLength;
                *iteratorPtr++ = pinStart.Y;
                *iteratorPtr++ = (pinStart.X + pinEnd.X) / 2;
                *iteratorPtr++ = (pinStart.Y + pinEnd.Y) / 2;
                *iteratorPtr++ = pinEnd.X - bezierTangentLength;
                *iteratorPtr++ = pinEnd.Y;
                *iteratorPtr++ = pinEnd.X;
                *iteratorPtr = pinEnd.Y;
            }

            return points;
        }
    }
}
