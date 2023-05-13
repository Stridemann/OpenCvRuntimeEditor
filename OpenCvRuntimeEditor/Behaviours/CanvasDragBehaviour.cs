namespace OpenCvRuntimeEditor.Behaviours
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.Xaml.Behaviors;
    using ViewModels.Nodes;

    public class CanvasDragBehaviour : Behavior<Panel>
    {
        public static readonly DependencyProperty ScaleXProperty = DependencyProperty.Register(
            nameof(ScaleX), typeof(double), typeof(CanvasDragBehaviour), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty ScaleYProperty = DependencyProperty.Register(
            nameof(ScaleY), typeof(double), typeof(CanvasDragBehaviour), new PropertyMetadata(default(double)));

        public double ScaleX
        {
            get => (double) GetValue(ScaleXProperty);
            set => SetValue(ScaleXProperty, value);
        }

        public double ScaleY
        {
            get => (double) GetValue(ScaleYProperty);
            set => SetValue(ScaleYProperty, value);
        }

        private static Point _mousePos;
        private static BaseGridObject _currentMovableObject;
        private static Point _startDragMousePos;
        private static Point _startDragPos;

        public static void BeginDragObject(BaseGridObject movable)
        {
            _currentMovableObject = movable;
            _startDragMousePos = _mousePos;
            _startDragPos = movable.Pos;
            Mouse.Capture(MainWindow.Instance.BgCanvas);
        }

        private void AssociatedObjectOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            _currentMovableObject = null;
        }

        private void AssociatedObjectOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            _mousePos = mouseEventArgs.GetPosition(AssociatedObject);

            if (_currentMovableObject == null)
                return;

            if (MainWindow.Instance.BgCanvas.IsMouseCaptured)
            {
        
                var dist = _startDragMousePos - _mousePos;
                dist.X /= ScaleX;
                dist.Y /= ScaleY;

                var finalPos = _startDragPos - dist;
                _currentMovableObject.Pos = finalPos;
            }
        }

        protected override void OnAttached()
        {
            AssociatedObject.MouseMove += AssociatedObjectOnMouseMove;
            AssociatedObject.MouseUp += AssociatedObjectOnMouseUp;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseMove -= AssociatedObjectOnMouseMove;
            AssociatedObject.MouseUp -= AssociatedObjectOnMouseUp;
        }
    }
}
