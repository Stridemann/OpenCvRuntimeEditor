namespace OpenCvRuntimeEditor.Behaviours
{
    using System.Windows.Input;
    using System.Windows.Shapes;
    using Microsoft.Xaml.Behaviors;
    using ViewModels;

    public class LinkBehaviour : Behavior<Path>
    {
        private PinLinkViewModel _pinViewModel;

        protected override void OnAttached()
        {
            AssociatedObject.MouseLeftButtonDown += AssociatedObjectOnMouseLeftButtonUp;
        }

        private void AssociatedObjectOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_pinViewModel == null)
                _pinViewModel = (PinLinkViewModel) AssociatedObject.DataContext;

            CanvasObjectsSelectionBehaviour.Instance.LinkSelected(_pinViewModel);
        }
    }
}
