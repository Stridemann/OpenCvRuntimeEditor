namespace OpenCvRuntimeEditor.Styles.TemplateSelectors
{
    using System.Windows;
    using System.Windows.Controls;
    using ViewModels;

    public class PinTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement fe && item is NodePinViewModel pin)
            {
                return fe.FindResource(pin.Direction == PinDirection.In ? "InPinDefaultTemplate" : "OutPinDefaultTemplate") as DataTemplate;
            }

            return null;
        }
    }
}
