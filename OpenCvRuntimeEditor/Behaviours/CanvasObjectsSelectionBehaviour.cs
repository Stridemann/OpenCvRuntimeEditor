namespace OpenCvRuntimeEditor.Behaviours
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Core;
    using Microsoft.Xaml.Behaviors;
    using ViewModels;

    public class CanvasObjectsSelectionBehaviour : Behavior<Grid>
    {
        private readonly List<NodeViewModel> _selectedNodes = new List<NodeViewModel>();
        private readonly List<PinLinkViewModel> _selectedLinks = new List<PinLinkViewModel>();

        public CanvasObjectsSelectionBehaviour()
        {
            if(Instance != null)
                throw new InvalidOperationException("Instance is already set. Multiple instances are not supported");
            Instance = this;
        }

        public static CanvasObjectsSelectionBehaviour Instance { get; private set; }

        public void LinkSelected(PinLinkViewModel link)
        {
            if (!NodeCanvasHotkeysBehaviour.ControlPressed)
                ClearSelection();

            link.IsSelected = true;

            if (!_selectedLinks.Contains(link))
                _selectedLinks.Add(link);
        }

        public void NodeSelected(NodeViewModel node)
        {
            if (!NodeCanvasHotkeysBehaviour.ControlPressed)
                ClearSelection();

            node.IsSelected = true;

            if (!_selectedNodes.Contains(node))
                _selectedNodes.Add(node);
        }

        private void ClearSelection()
        {
            _selectedNodes.ForEach(x => x.IsSelected = false);
            _selectedLinks.ForEach(x => x.IsSelected = false);
            _selectedNodes.Clear();
            _selectedLinks.Clear();
        }

        public void CopyCommand()
        {
            CopyPaste.CopyNodes(_selectedNodes);
        }

        public void PasteCommand()
        {
            CopyPaste.PasteNodes();
        }

        protected override void OnAttached()
        {
            AssociatedObject.Focusable = true;
            AssociatedObject.Loaded += AssociatedObjectOnLoaded;
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObjectOnMouseLeftButtonDown;
        }

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Focus(); //https://stackoverflow.com/a/15241720
        }

        private void AssociatedObjectOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (NodeCanvasHotkeysBehaviour.ControlPressed)
                return;

            ClearSelection();
        }

        public void OnDeletePressed()
        {
            NodesCanvasViewModel.Current.DeleteNodes(_selectedNodes);
            NodesCanvasViewModel.Current.DeleteLinks(_selectedLinks);
            _selectedNodes.Clear();
            _selectedLinks.Clear();
        }
    }
}
