namespace OpenCvRuntimeEditor.Behaviours
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Core;
    using Microsoft.Xaml.Behaviors;
    using Utils;
    using ViewModels;
    using WIndows;
    using Control = System.Windows.Forms.Control;

    public class NodesMenuWindowOpenBehaviour : Behavior<Grid>
    {
        private readonly NodesWIndow _nodesWIndow;
        private readonly NodesWindowViewModel _nodesWindowViewModel;
        public bool IsOpened { get; set; }
        private Point _mouseDownPoint;
        public static NodesMenuWindowOpenBehaviour Instance;
        public event Action OnClose = delegate { };

        public NodesMenuWindowOpenBehaviour()
        {
            Instance = this;
            _nodesWindowViewModel = new NodesWindowViewModel();

            _nodesWIndow = new NodesWIndow
            {
                DataContext = _nodesWindowViewModel,
                WindowStartupLocation = WindowStartupLocation.Manual,
            };
        }

        private void AssociatedObjectOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsOpened)
            {
                CloseWindow();
                return;
            }

            if (e.ChangedButton != MouseButton.Right)
                return;

            _mouseDownPoint = e.GetPosition((IInputElement) sender);
        }

        private void AssociatedObjectOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right)
                return;

            var currentMousePos = e.GetPosition(AssociatedObject);

            if (Point.Subtract(currentMousePos, _mouseDownPoint).Length > 5)
                return;

            Open(currentMousePos);
        }

        public void Open(Point currentMousePos)
        {
            if (IsOpened)
                CloseWindow();

            _nodesWindowViewModel.OnNodeSelected = delegate(NodeCreationConfig config)
            {
                var node = NodesCanvasViewModel.Current.AddNode(config, CoordsTransformUtils.GlobalToCanvas(currentMousePos));

                if (NodeLinkingBehaviour.Instance.LinkingPin != null)
                {
                    List<NodePinViewModel> otherPins;

                    if (NodeLinkingBehaviour.Instance.LinkingPin.Direction == PinDirection.In)
                    {
                        otherPins = node.OutPins.Where(x =>
                            TypeLinkingUtils.CheckPinsLinking(x.PinType, NodeLinkingBehaviour.Instance.LinkingPin.PinType).Allowed).ToList();
                    }
                    else
                    {
                        otherPins = node.InPins.Where(x =>
                            TypeLinkingUtils.CheckPinsLinking(NodeLinkingBehaviour.Instance.LinkingPin.PinType, x.PinType).Allowed).ToList();
                    }

                    if (otherPins.Count == 1)
                    {
                        NodeLinkingBehaviour.Instance.FinishLinkPins(otherPins[0]);
                    }
                }

                CloseWindow();
            };

            var mousePos = Control.MousePosition;
            var pos = Application.Current.MainWindow.TranslatePoint(new Point(mousePos.X, mousePos.Y), Application.Current.MainWindow);
            _nodesWindowViewModel.OnOpening();
            _nodesWIndow.Left = pos.X + 3;
            _nodesWIndow.Top = pos.Y;
            _nodesWIndow.Visibility = Visibility.Visible;
            IsOpened = true;
            _nodesWIndow.Owner = Application.Current.MainWindow;
        }

        private void CloseWindow()
        {
            _nodesWindowViewModel.OnClosing();
            _nodesWIndow.Visibility = Visibility.Hidden;
            IsOpened = false;
            OnClose();
        }

        protected override void OnAttached()
        {
            AssociatedObject.MouseDown += AssociatedObjectOnMouseDown;
            AssociatedObject.MouseUp += AssociatedObjectOnMouseUp;
            Application.Current.MainWindow.Closing += delegate { _nodesWIndow.Close(); };
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseDown -= AssociatedObjectOnMouseDown;
            AssociatedObject.MouseUp -= AssociatedObjectOnMouseUp;
        }
    }
}
