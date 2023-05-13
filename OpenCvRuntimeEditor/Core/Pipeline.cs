namespace OpenCvRuntimeEditor.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using ViewModels;

    public static class Pipeline
    {
        public static void Recalculate(List<NodeViewModel> nodes)
        {
            nodes.ForEach(x => x.PipelineData.NeedRecalculate = true);

            foreach (var node in nodes)
            {
                if (!node.PipelineData.NeedRecalculate)
                    continue;

                Recalculate(node);
            }
        }

        public static void Recalculate(NodeViewModel node)
        {
            node.ErrorInfo.ClearMessages(ErrorInfoType.Pipeline);

            if (node.PipelineData.IsProcessing)
            {
                node.ErrorInfo.AddPipelineError("Circular dependency detected (node result is used to obtain it's input data).\r\n" +
                                                "Remove link that cause closed processing cycle");

                var recursizeCheckList = new List<NodeViewModel>();
                MarkAllNextAsUnsuccessfull(node, recursizeCheckList);
                return;
            }

            node.PipelineData.IsProcessing = true;
            node.PipelineData.NeedRecalculate = true;

            var success = true;

            foreach (var pin in node.InPins)
            {
                pin.ErrorInfo.ClearMessages(ErrorInfoType.Pipeline);
                var link = pin.Links.FirstOrDefault();

                if (link == null)
                {
                    if (!pin.IsOptional)
                    {
                        pin.ErrorInfo.AddPipelineError("Require connected pin");
                        success = false;
                    }
                }
                else
                {
                    var connectedNode = link.OutPin.Owner;

                    if (connectedNode.PipelineData.NeedRecalculate)
                        Recalculate(connectedNode);

                    if (!connectedNode.PipelineData.CalculationSuccessfull)
                    {
                        success = false;
                    }
                }
            }

            node.PipelineData.CalculationSuccessfull = success;
            node.PipelineData.NeedRecalculate = false;

            if (success)
            {
                try
                {
                    node.ProcessingStrategy.Process(node);

                    foreach (var outPin in node.OutPins)
                    {
                        if (!outPin.IsLinked)
                            continue;

                        foreach (var link in outPin.Links)
                        {
                            if (!link.InPin.Owner.PipelineData.IsProcessing)
                                Recalculate(link.InPin.Owner);
                        }
                    }
                }
                catch (TargetInvocationException e) when (e.InnerException != null)
                {
                    node.PipelineData.CalculationSuccessfull = false;
                    node.ErrorInfo.AddPipelineError(e.InnerException.Message);
                }
                catch (Exception e) when (e.InnerException != null)
                {
                    node.PipelineData.CalculationSuccessfull = false;
                    node.ErrorInfo.AddPipelineError(e.InnerException.Message);
                }
                catch (Exception e)
                {
                    node.PipelineData.CalculationSuccessfull = false;
                    node.ErrorInfo.AddPipelineError(e.Message);
                }
            }

            node.PipelineData.IsProcessing = false;

            if (!node.PipelineData.CalculationSuccessfull)
            {
                var recursizeCheckList = new List<NodeViewModel>();
                MarkAllNextAsUnsuccessfull(node, recursizeCheckList);
            }
        }

        private static void MarkAllNextAsUnsuccessfull(NodeViewModel node, List<NodeViewModel> checkedNodes)
        {
            checkedNodes.Add(node);
            node.PipelineData.NeedRecalculate = false;
            node.PipelineData.CalculationSuccessfull = false;
            node.PipelineData.ClearPreviewImage();

            if (node.PipelineData.IsProcessing)
                return;

            foreach (var outPin in node.OutPins)
            {
                if (!outPin.IsLinked)
                    continue;

                foreach (var link in outPin.Links)
                {
                    if (!checkedNodes.Contains(link.InPin.Owner))
                        MarkAllNextAsUnsuccessfull(link.InPin.Owner, checkedNodes);
                }
            }
        }
    }
}
