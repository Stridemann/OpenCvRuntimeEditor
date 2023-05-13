namespace OpenCvRuntimeEditor.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Utils.Documentation;
    using ViewModels;

    public class NodeDocumentationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NodeViewModel node)
            {
                if (node.PipelineData.StrategyMethod != null || node.PipelineData.StrategyProperty != null)
                {
                    var typeDocumentation = DocumentationDatabase.GetXmlDocumentation((object)node.PipelineData.StrategyMethod ?? node.PipelineData.StrategyProperty);

                    var summary = typeDocumentation?["summary"];

                    if (summary != null)
                        return summary.InnerText.Trim();
                }
            }

            return string.Empty;// "Documentation is not found";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
