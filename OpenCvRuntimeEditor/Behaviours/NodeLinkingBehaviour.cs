namespace OpenCvRuntimeEditor.Behaviours
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Controls;
    using Core;
    using Emgu.CV;
    using Microsoft.Xaml.Behaviors;
    using Utils;
    using ViewModels;

    public class NodeLinkingBehaviour : Behavior<Grid>
    {
        public static readonly DependencyProperty NodesLinksRootGridProperty = DependencyProperty.Register(
            nameof(NodesLinksRootGrid), typeof(Grid), typeof(NodeLinkingBehaviour),
            new PropertyMetadata(default(Grid)));

        private readonly NodePinViewModel _visualLinkEnd;

        public NodeLinkingBehaviour()
        {
            if (Instance != null)
                throw new InvalidOperationException("Instance is already set. Multiple instances are not supported");

            Instance = this;
            _visualLinkEnd = new NodePinViewModel(string.Empty, typeof(Mat), PinDirection.In, null);
        }

        public PinLinkViewModel VisualHelperLink { get; set; }
        public NodePinViewModel LinkingPin { get; set; }
        public static NodeLinkingBehaviour Instance { get; private set; }

        public Grid NodesLinksRootGrid
        {
            get => (Grid) GetValue(NodesLinksRootGridProperty);
            set => SetValue(NodesLinksRootGridProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.MouseMove += AssociatedObjectOnMouseMove;
            AssociatedObject.MouseLeftButtonUp += AssociatedObjectOnMouseLeftButtonUp;
            ZoomBorder.OnTrnasformChanged += () => { VisualHelperLink?.UpdateSplinePoints(); };

            NodesMenuWindowOpenBehaviour.Instance.OnClose += NodesMenuWindowOnClose;
        }

        private void NodesMenuWindowOnClose()
        {
            if (VisualHelperLink == null)
                return;

            VisualHelperLink.IsVisible = false;
            LinkingPin = null;
        }

        private void AssociatedObjectOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (VisualHelperLink == null || LinkingPin == null)
                return;

            NodesMenuWindowOpenBehaviour.Instance.Open(e.GetPosition(AssociatedObject));
        }

        public void PrepareLinkPins(NodePinViewModel pin)
        {
            if (VisualHelperLink == null)
            {
                VisualHelperLink = new PinLinkViewModel(_visualLinkEnd, _visualLinkEnd);
                NodesLinksRootGrid.Children.Add(VisualHelperLink.Path);
            }

            LinkingPin = pin;
            _visualLinkEnd.PinType = pin.PinType;
            VisualHelperLink.Path.Stroke = ColorConverterUtils.BrushFromType(pin.PinType);
            if (pin.Direction == PinDirection.In)
            {
                VisualHelperLink.InPin = pin;
                VisualHelperLink.OutPin = _visualLinkEnd;
                VisualHelperLink.UpdateSplinePoints();
                VisualHelperLink.IsVisible = true;
            }
            else
            {
                VisualHelperLink.OutPin = pin;
                VisualHelperLink.InPin = _visualLinkEnd;
                VisualHelperLink.UpdateSplinePoints();
                VisualHelperLink.IsVisible = true;
            }

            _visualLinkEnd.Pos = pin.Pos;
        }

        public void FinishLinkPins(NodePinViewModel pin)
        {
            if (LinkingPin != null)
            {
                LinkNodePins(LinkingPin, pin);
                VisualHelperLink.IsVisible = false;
                LinkingPin = null;
            }
        }

        private void AssociatedObjectOnMouseMove(object sender, MouseEventArgs e)
        {
            if (LinkingPin != null && !NodesMenuWindowOpenBehaviour.Instance.IsOpened)
            {
                var pos = e.GetPosition(NodesLinksRootGrid);
                _visualLinkEnd.Pos = CoordsTransformUtils.GlobalToCanvas(pos);
            }
        }

        private void LinkNodePins(NodePinViewModel pin1, NodePinViewModel pin2)
        {
            if (pin1.Direction == pin2.Direction || pin1.Owner == pin2.Owner)
                return;

            if (pin1.Direction == PinDirection.In)
                ProcessPinsLinking(pin1, pin2);
            else
                ProcessPinsLinking(pin2, pin1);
        }

        private void ProcessPinsLinking(NodePinViewModel inPin, NodePinViewModel outPin)
        {
            var linkingResult = TypeLinkingUtils.CheckPinsLinking(outPin.PinType, inPin.PinType);

            if (!linkingResult.Allowed)
                return;

            if (linkingResult.ConverterType != ConverterNodeType.None)
            {
                var converterNode = InsertConverter(inPin, outPin, linkingResult.ConverterType);
                var converterIn = converterNode.InPins[0];
                var converterOut = converterNode.OutPins[0];

                LinkPins(converterIn, outPin);
                LinkPins(inPin, converterOut);

                return;
            }

            LinkPins(inPin, outPin);
        }

        private NodeViewModel InsertConverter(NodePinViewModel inPin, NodePinViewModel outPin, ConverterNodeType converterType)
        {
            var converterNodeConfig = NodeDatabase.Converters[converterType];
            var converterNodePos = PointUtils.GetMidPoint(inPin.Owner.Pos, outPin.Owner.Pos, -50);

            return NodesCanvasViewModel.Current.AddNode(converterNodeConfig, converterNodePos);
        }

        public static void LinkPins(NodePinViewModel inPin, NodePinViewModel outPin)
        {
            if (inPin.Links.Count > 0)
            {
                NodesCanvasViewModel.Current.DeleteLink(inPin.Links[0]);
                inPin.Links.Clear();
            }

            var linkViewModel = new PinLinkViewModel(outPin, inPin);
            inPin.Links.Add(linkViewModel);
            outPin.Links.Add(linkViewModel);

            NodesCanvasViewModel.Current.AddLink(linkViewModel);

            Pipeline.Recalculate(inPin.Owner);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseMove -= AssociatedObjectOnMouseMove;
            AssociatedObject.MouseLeftButtonUp -= AssociatedObjectOnMouseLeftButtonUp;
        }
    }
}
