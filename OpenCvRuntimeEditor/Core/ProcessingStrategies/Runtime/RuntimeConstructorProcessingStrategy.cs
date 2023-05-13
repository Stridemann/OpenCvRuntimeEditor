namespace OpenCvRuntimeEditor.Core.ProcessingStrategies.Runtime
{
    using System;
    using System.Collections.Generic;
    using ViewModels;

    public class RuntimeConstructorProcessingStrategy : IProcessingStrategy
    {
        public delegate object ObjectActivator();

        public void Process(NodeViewModel node)
        {
            var owner = node.PipelineData.StrategyConstructorType;
            var args = new List<object>();
            var argTypes = new List<Type>();

            foreach (var pin in node.InPins)
            {
                var pinData = pin.PinData.GetData<object>();

                argTypes.Add(pinData.GetType());
                args.Add(pinData);
            }

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

            //https://www.wintellect.com/getting-to-know-dynamicmethod/
            //https://vagifabilov.wordpress.com/2010/04/02/dont-use-activator-createinstance-or-constructorinfo-invoke-use-compiled-lambda-expressions/
            var result = Activator.CreateInstance(owner, args.ToArray());
            node.SetOutPinData(0, result);
            node.PipelineData.UpdatePreviewImage(result);
        }
    }
}
