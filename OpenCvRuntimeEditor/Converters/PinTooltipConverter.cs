namespace OpenCvRuntimeEditor.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Xml;
    using Utils;
    using Utils.Documentation;
    using ViewModels;

    public class PinTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NodePinViewModel pin)
            {
                var renamedType = TypeRenamingUtils.RenameType(pin.PinType);

                var typeDocumentation = DocumentationDatabase.GetXmlDocumentation(pin.PinType.IsConstructedGenericType
                                                                                      ? pin.PinType.GetGenericTypeDefinition()
                                                                                      : pin.PinType);

                var summary = typeDocumentation?["summary"];

                if (summary != null)
                    renamedType += $" ({summary.InnerText.Trim()})";

                var node = pin.Owner;

                if (node.PipelineData.StrategyMethod != null || node.PipelineData.StrategyProperty != null)
                {
                    var nodeDocumentation = DocumentationDatabase.GetXmlDocumentation((object)node.PipelineData.StrategyMethod ??
                                                                                      node.PipelineData.StrategyProperty);

                    if (nodeDocumentation != null)
                    {
                        var found = false;

                        foreach (XmlNode nodeDocumentationChildNode in nodeDocumentation.ChildNodes)
                        {
                            if (nodeDocumentationChildNode.Name != "param" || nodeDocumentationChildNode.Attributes == null)
                                continue;

                            foreach (XmlNode attribute in nodeDocumentationChildNode.Attributes)
                            {
                                if (attribute.Value == pin.Name)
                                {
                                    renamedType += $"\r\n{nodeDocumentationChildNode.InnerText}";
                                    found = true;

                                    break;
                                }
                            }

                            if (found)
                                break;
                        }

                        if (!found && pin.Direction == PinDirection.Out)
                        {
                            var returnsDoc = nodeDocumentation["returns"];

                            if (returnsDoc != null)
                            {
                                renamedType += $"\r\n{returnsDoc.InnerText.Trim()}";
                            }
                        }
                    }
                }

                var pinValue = pin.PinData.GetData<object>();
                var formatedValue = TypeRenamingUtils.RenameValue(pinValue);
                renamedType += $"{Environment.NewLine}Value: {formatedValue}";

                return renamedType;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
