namespace OpenCvRuntimeEditor.Behaviours
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.Xaml.Behaviors;
    using ViewModels;

    public class VariableStartDragBehaviour : Behavior<Button>
    {
        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObjectOnMouseLeftButtonDown;
        }

        private void AssociatedObjectOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var lbl = (Button) sender;
            var variable = (VariableViewModel) lbl.DataContext;
            DragDrop.DoDragDrop(lbl, variable, DragDropEffects.Copy);
        }
    }
}
