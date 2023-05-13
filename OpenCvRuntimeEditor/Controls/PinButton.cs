namespace OpenCvRuntimeEditor.Controls
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using Behaviours;
    using ViewModels;

    public class PinButton : Button
    {
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
            NodeLinkingBehaviour.Instance.PrepareLinkPins((NodePinViewModel)DataContext);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            e.Handled = true;
            NodeLinkingBehaviour.Instance.FinishLinkPins((NodePinViewModel)DataContext);
        }
    }
}
