namespace OpenCvRuntimeEditor.Core.ProcessingStrategies.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Emgu.CV;
    using ViewModels;

    public class RuntimePropertyProcessingStrategy : IProcessingStrategy
    {
        public void Process(NodeViewModel node)
        {
            object result;

            var origGetMethod = node.PipelineData.StrategyProperty.GetGetMethod();
            var isStatic = origGetMethod.IsStatic;

            var ownerType = isStatic
                ? node.PipelineData.StrategyProperty.ReflectedType
                : node.InPins.First().PinData.GetData<object>()?.GetType() ?? node.InPins.First().PinType;

            var args = new List<object>();
            var argTypes = new List<Type>();

            foreach (var pin in node.InPins)
            {
                var pinData = pin.PinData.GetData<object>();

                if (pin.PinType == typeof(IOutputArray))
                    pinData = new Mat();

                argTypes.Add(pinData.GetType());
                args.Add(pinData);
            }

            if (!isStatic)
                argTypes.RemoveAt(0);

            if (ownerType.IsGenericTypeDefinition)
            {
                foreach (var argType in argTypes)
                {
                    if (argType.IsGenericType)
                    {
                        if (argType.GetGenericTypeDefinition() == ownerType)
                        {
                            ownerType = argType;
                            break;
                        }
                    }
                }
            }

            var property = ownerType.GetProperty(node.PipelineData.StrategyProperty.Name);
            var getMethod = property.GetGetMethod();

            if (!isStatic)
            {
                var target = args.FirstOrDefault();
                args.RemoveAt(0);
                result = getMethod.Invoke(target, args.ToArray());
            }
            else
                result = getMethod.Invoke(null, args.ToArray());

            node.SetOutPinData(0, result);

            var iOutput = node.OutPins.Where(x => typeof(IOutputArray).IsAssignableFrom(x.PinType)).ToList();

            if (iOutput.Count > 0)
            {
                node.PipelineData.UpdatePreviewImage(iOutput.First().PinData.GetData<object>());
            }
        }
    }
}
