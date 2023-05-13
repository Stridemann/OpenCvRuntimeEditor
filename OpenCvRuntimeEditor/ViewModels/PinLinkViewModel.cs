namespace OpenCvRuntimeEditor.ViewModels
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Utils;

    public class PinLinkViewModel : BaseViewModel
    {
        private readonly PolyQuadraticBezierSegment _segment;
        private readonly PathFigure _splineFigure;

        public PinLinkViewModel(NodePinViewModel outPin, NodePinViewModel inPin)
        {
            OutPin = outPin;
            InPin = inPin;

            _segment = new PolyQuadraticBezierSegment(new Point[0], true);
            _splineFigure = new PathFigure(new Point(), new[] {_segment}, false);
            var splineGeomerty = new PathGeometry();
            splineGeomerty.Figures.Add(_splineFigure);

            Path = new Path
            {
                Stroke = ColorConverterUtils.BrushFromType(outPin.PinType),
                StrokeThickness = 2,
                Data = splineGeomerty,
                SnapsToDevicePixels = true
            };

            InPin.OnPosChanged += UpdateSplinePoints;
            OutPin.OnPosChanged += UpdateSplinePoints;
            UpdateSplinePoints();
        }

        public bool IsVisible
        {
            set => Path.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public NodePinViewModel InPin { get; set; }
        public NodePinViewModel OutPin { get; set; }
        public Path Path { get; set; }

        public bool IsSelected
        {
            set => Path.StrokeThickness = value ? 5 : 2;
        }

        public void UpdateSplinePoints()
        {
            var start = CoordsTransformUtils.CanvasToGlobal(OutPin.Pos);
            var end = CoordsTransformUtils.CanvasToGlobal(InPin.Pos);
            var points = BezierHelpers.GenerateBasierCurve(start, end);

            _splineFigure.StartPoint = start;
            _segment.Points = new PointCollection(points);
        }
    }
}
