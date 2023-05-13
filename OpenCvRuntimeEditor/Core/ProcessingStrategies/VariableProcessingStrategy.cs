namespace OpenCvRuntimeEditor.Core.ProcessingStrategies
{
    using System.Linq;
    using ViewModels;

    public class VariableProcessingStrategy : IProcessingStrategy
    {
        public VariableViewModel Variable { get; private set; }

        public void Process(NodeViewModel node)
        {
            if (Variable == null)
                Variable = NodesCanvasViewModel.Current.Variables.FirstOrDefault(x => x.Id == node.VariableRefId);

            if (Variable == null)
            {
                node.ErrorInfo.AddPipelineError($"Unexpected error. Cannot find variable with id: {node.VariableRefId}");
                return;
            }

            node.SetOutPinData(0, Variable.Data);
            node.PipelineData.UpdatePreviewImage(Variable.Data);
        }
    }
}
