namespace OpenCvRuntimeEditor.Behaviours
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.Xaml.Behaviors;
    using ViewModels;

    public class NodeBehaviour : Behavior<UserControl>
    {
        private NodeViewModel _nodeViewModel;
        private bool _firstTime;
        private double _nodeWidth;

        protected override void OnAttached()
        {
            AssociatedObject.MouseLeftButtonDown += AssociatedObjectOnMouseLeftButtonUp;
            AssociatedObject.LayoutUpdated += AssociatedObjectOnLayoutUpdated;
        }

        private void AssociatedObjectOnLayoutUpdated(object sender, EventArgs e)
        {
            if (_firstTime && Math.Abs(_nodeWidth - AssociatedObject.ActualWidth) < double.Epsilon)
                return;

            _nodeWidth = AssociatedObject.ActualWidth;
            _firstTime = true;

            if (_nodeViewModel == null)
                _nodeViewModel = (NodeViewModel) AssociatedObject.DataContext;

            _nodeViewModel.RaisePropertyChanged(nameof(_nodeViewModel.Pos));
        }

        private void AssociatedObjectOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_nodeViewModel == null)
                _nodeViewModel = (NodeViewModel) AssociatedObject.DataContext;

            CanvasDragBehaviour.BeginDragObject(_nodeViewModel);
            CanvasObjectsSelectionBehaviour.Instance.NodeSelected(_nodeViewModel);
        }
    }
}
