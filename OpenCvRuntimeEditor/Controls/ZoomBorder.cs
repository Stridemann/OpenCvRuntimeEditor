namespace OpenCvRuntimeEditor.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public class ZoomBorder : Border
    {
        public static ZoomBorder Instance { get; private set; }
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(ZoomBorder), new PropertyMetadata(0d));

        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(ZoomBorder), new PropertyMetadata(0d));

        public static readonly DependencyProperty ScaleXProperty =
            DependencyProperty.Register(nameof(ScaleX), typeof(double), typeof(ZoomBorder), new PropertyMetadata(1d));

        public static readonly DependencyProperty ScaleYProperty =
            DependencyProperty.Register(nameof(ScaleY), typeof(double), typeof(ZoomBorder), new PropertyMetadata(1d));

        public static readonly DependencyProperty CanvasRootGridProperty = 
            DependencyProperty.Register(nameof(CanvasRootGrid), typeof(Grid), typeof(ZoomBorder), new PropertyMetadata(default(Grid)));

        private UIElement _child;
        private Point _origin;
        private ScaleTransform _scaleTransform;
        private Point _startMovePos;
        private TranslateTransform _translateTransform;

        public static event Action OnTrnasformChanged = delegate { };

        public override UIElement Child
        {
            get => base.Child;
            set
            {
                if (value != null && value != Child)
                    Initialize(value);

                base.Child = value;
            }
        }

        public Grid CanvasRootGrid
        {
            get => (Grid) GetValue(CanvasRootGridProperty);
            set => SetValue(CanvasRootGridProperty, value);
        }

        public double OffsetX
        {
            get => (double) GetValue(OffsetXProperty);
            set => SetValue(OffsetXProperty, value);
        }

        public double OffsetY
        {
            get => (double) GetValue(OffsetYProperty);
            set => SetValue(OffsetYProperty, value);
        }

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

        public Vector AbsoluteOffset { get; set; }

        public void Initialize(UIElement element)
        {
            _child = element;

            if (_child != null)
            {
                var group = new TransformGroup();

                _scaleTransform = new ScaleTransform();
                group.Children.Add(_scaleTransform);

                _translateTransform = new TranslateTransform();
                group.Children.Add(_translateTransform);
                _child.RenderTransform = group;
                _child.RenderTransformOrigin = new Point(0.0, 0.0);

                CanvasRootGrid.MouseDown += OnMouseButtonDown;
                CanvasRootGrid.MouseWheel += OnMouseWheel;
                CanvasRootGrid.MouseUp += OnMouseButtonUp;
                CanvasRootGrid.MouseMove += OnMouseMove;
                CanvasRootGrid.SizeChanged += OnCanvasRootGridSizeChanged;

                if(Instance != null)
                    throw new InvalidOperationException("Instance is already set. Multiple instances are not supported");
                Instance = this;
            }
        }

        #region Child Events

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_child != null)
            {
                var st = _scaleTransform;
                var tt = _translateTransform;

                var zoom = e.Delta > 0 ? .1 : -.1;

                if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                    return;

                if (!(e.Delta < 0) && (st.ScaleX > 0.9 || st.ScaleY > 0.9))
                    return;

                var relative = e.GetPosition(_child);
                var abosuluteX = relative.X * st.ScaleX + tt.X;
                var abosuluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX += zoom;
                st.ScaleY += zoom;
                ScaleX = st.ScaleX;
                ScaleY = st.ScaleY;

                tt.X = abosuluteX - relative.X * st.ScaleX;
                tt.Y = abosuluteY - relative.Y * st.ScaleY;

                AbsoluteOffset = new Vector(tt.X, tt.Y);
                OffsetX = tt.X / ActualWidth / ScaleX;
                OffsetY = tt.Y / ActualHeight / ScaleX;
                OnTrnasformChanged();
            }
        }

        private void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                return;

            if (_child != null)
            {
                var tt = _translateTransform;
                _startMovePos = e.GetPosition(this);
                _origin = new Point(tt.X, tt.Y);
                Cursor = Cursors.Hand;
                _child.CaptureMouse();
            }
        }

        private void OnMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                return;

            if (_child != null)
            {
                _child.ReleaseMouseCapture();
                Cursor = Cursors.Arrow;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_child != null)
            {
                if (_child.IsMouseCaptured)
                {
                    var deltaMouse = _startMovePos - Mouse.GetPosition(this);
                    var tt = _translateTransform;
                    tt.X = _origin.X - deltaMouse.X;
                    tt.Y = _origin.Y - deltaMouse.Y;

                    AbsoluteOffset = new Vector(tt.X, tt.Y);
                    OffsetX = tt.X / ActualWidth / ScaleX;
                    OffsetY = tt.Y / ActualHeight / ScaleX;

                    OnTrnasformChanged();
                }
            }
        }

        private void OnCanvasRootGridSizeChanged(object sender, SizeChangedEventArgs e) //this is a fix when window resizing
        {
            var tt = _translateTransform;
            OffsetX = tt.X / ActualWidth / ScaleX;
            OffsetY = tt.Y / ActualHeight / ScaleX;

            OnTrnasformChanged();
        }

        #endregion
    }
}
