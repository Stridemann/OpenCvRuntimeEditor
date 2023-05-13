namespace OpenCvRuntimeEditor.Styles.TemplateSelectors
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Core;
    using ViewModels;
    using ViewModels.PinDataVisualProcessors.Base;

    public class PinDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var pin = item as NodePinViewModel;
            var basePinVisualDataProcessor = pin?.VisualDataProcessor ?? (BasePinVisualDataProcessor)item;
            
            if (container is FrameworkElement fe && basePinVisualDataProcessor != null)
            {
                var type = basePinVisualDataProcessor.GetType();

                var typeName = type.Name;
                const string POSTFIX = "VisualDataProcessor";
                var index = typeName.IndexOf(POSTFIX, StringComparison.Ordinal);

                if (index == -1)
                {
                    pin?.ErrorInfo.AddError($"Expecting '{POSTFIX}' in {nameof(pin.VisualDataProcessor)} type name to find template", ErrorInfoType.Core);
                    return null;
                }

                typeName = typeName.Substring(0, index + POSTFIX.Length);
                var resourceName = $"{typeName}Template";
                var resource = fe.TryFindResource(resourceName);

                if (resource != null)
                    return resource as DataTemplate;

                pin?.ErrorInfo.AddError($"Can't find pin data template from generated name: {resourceName} name.", ErrorInfoType.Core);
            }

            return null;
        }
    }
}
