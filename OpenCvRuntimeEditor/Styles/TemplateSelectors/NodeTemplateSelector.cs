namespace OpenCvRuntimeEditor.Styles.TemplateSelectors
{
    using System.Windows;
    using System.Windows.Controls;

    public class NodeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement fe && fe.DataContext is ITemplateType templateType)
            {
                var template = fe.FindResource(templateType.TemplateKey) as DataTemplate;
                return template;
            }
            return null;
        }
    }
}
