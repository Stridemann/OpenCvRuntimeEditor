namespace OpenCvRuntimeEditor.Behaviours
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.Xaml.Behaviors;

    public class NodeCanvasHotkeysBehaviour : Behavior<Grid>
    {
        public static bool ControlPressed { get; private set; }

        protected override void OnAttached()
        {
            AssociatedObject.Focusable = true;
            AssociatedObject.KeyDown += AssociatedObjectOnKeyDown;
            AssociatedObject.KeyUp += AssociatedObjectOnKeyUp;
            AssociatedObject.MouseLeftButtonUp += AssociatedObjectOnMouseLeftButtonUp;
        }

        private void AssociatedObjectOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AssociatedObject.Focus(); //https://stackoverflow.com/a/15241720
        }

        private void AssociatedObjectOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                CanvasObjectsSelectionBehaviour.Instance.OnDeletePressed();
            }
            else if (e.Key == Key.LeftCtrl)
            {
                ControlPressed = true;
            }

            if (ControlPressed)
            {
                if (e.Key == Key.C)
                    CanvasObjectsSelectionBehaviour.Instance.CopyCommand();
                else if (e.Key == Key.V)
                    CanvasObjectsSelectionBehaviour.Instance.PasteCommand();
            }
        }

        private void AssociatedObjectOnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
            {
                ControlPressed = false;
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyDown -= AssociatedObjectOnKeyDown;
            AssociatedObject.KeyUp -= AssociatedObjectOnKeyUp;
            AssociatedObject.MouseLeftButtonUp -= AssociatedObjectOnMouseLeftButtonUp;
        }
    }
}
