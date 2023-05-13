namespace OpenCvRuntimeEditor.Behaviours
{
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Xaml.Behaviors;
    using ViewModels;

    public class NodeResizeTriggerBehaviour : Behavior<Grid>
    {
        protected override void OnAttached()
        {
            AssociatedObject.SizeChanged += AssociatedObjectOnSizeChanged;
        }

        private void AssociatedObjectOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var nodeViewModel = (NodeViewModel) AssociatedObject.DataContext;
            nodeViewModel?.RaisePropertyChanged(nameof(nodeViewModel.Pos));
        }
    }
}
