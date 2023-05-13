namespace OpenCvRuntimeEditor.Core.ProcessingStrategies.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Emgu.CV;
    using Utils;
    using ViewModels;

    public class RuntimeMethodProcessingStrategy : IProcessingStrategy
    {
        public void Process(NodeViewModel node)
        {
            object result;

            var owner = node.PipelineData.StrategyMethod.IsStatic
                ? node.PipelineData.StrategyMethod.ReflectedType
                : node.InPins.First().PinData.GetData<object>()?.GetType() ?? node.InPins.First().PinType;

            var args = new List<object>();
            var argTypes = new List<Type>();

            foreach (var pin in node.InPins)
            {
                var pinData = pin.PinData.GetData<object>();
                var pinType = pinData == null ? pin.PinType : pinData.GetType();

                if (pin.PinType == typeof(IOutputArray))
                {
                    pinData = new Mat();
                    pin.PinData.SetData(pinData);
                }

                argTypes.Add(pinType);

                if(node.PipelineData.StrategyMethod.Name == "_Erode" || node.PipelineData.StrategyMethod.Name == "_Dilate")
                    args.Add(CvUtils.CloneObj(pinData));
                else
                    args.Add(pinData);
            }

            if (!node.PipelineData.StrategyMethod.IsStatic)
                argTypes.RemoveAt(0);

            if (owner.IsGenericTypeDefinition)
            {
                foreach (var argType in argTypes)
                {
                    if (argType.IsGenericType)
                    {
                        if (argType.GetGenericTypeDefinition() == owner)
                        {
                            owner = argType;

                            break;
                        }
                    }
                }
            }

            var method = owner.GetMethod(node.PipelineData.StrategyMethod.Name, argTypes.ToArray());

            if (method == null)
            {
                node.ErrorInfo.AddPipelineError($"Cannot find method {node.PipelineData.StrategyMethod.Name} in {owner.Name}");
            }
            else
            {
                if (!node.PipelineData.StrategyMethod.IsStatic)
                {
                    var target = args.FirstOrDefault();
                    result = method.Invoke(target, args.Skip(1).ToArray());
                }
                else
                    result = method.Invoke(null, args.ToArray());


                if (method.ReturnType != typeof(void))
                {
                    node.SetOutPinData(0, result);
                }
                else if (node.OutPins.Count > 0)
                {
                    //this is hotfix for void Image<,>.Erode();
                    var inResultToOut = args.FirstOrDefault(x => x is IInputArray);

                    if (inResultToOut != null)
                    {
                        var matchOut = node.OutPins.FirstOrDefault(x => typeof(IInputArray).IsAssignableFrom(x.PinType));
                        matchOut?.PinData.SetData(inResultToOut);
                    }
                }
            }
       
        
            var outAdditionalArgs = node.InPins.Where(x => x.PinType == typeof(IOutputArray)).ToArray();

            foreach (var pin in outAdditionalArgs)
            {
                var outPinDuplicate = node.OutPins.FirstOrDefault(x => x.Name == pin.Name);

                if (outPinDuplicate != null)
                {
                    var data = pin.PinData.GetData<object>();
                    outPinDuplicate.PinData.SetData(data);

                    if (data != null)
                    {
                        outPinDuplicate.PinType = data.GetType();
                    }
                }
            }

            var iOutput = node.OutPins.Where(x => typeof(IOutputArray).IsAssignableFrom(x.PinType)).ToList();

            if (iOutput.Count > 0)
            {
                if (iOutput.Count > 1)
                {
                    var dstFiltered = iOutput.FirstOrDefault(x => x.Name == "dst");

                    if (dstFiltered != null)
                    {
                        node.PipelineData.UpdatePreviewImage(dstFiltered.PinData.GetData<object>());

                        return;
                    }

                    node.ErrorInfo.AddPipelineError("Multiple possible preview sources. TODO: handle me");
                }

                node.PipelineData.UpdatePreviewImage(iOutput.First().PinData.GetData<object>());
            }
        }
    }
}
