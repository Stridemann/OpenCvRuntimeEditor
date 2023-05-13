namespace OpenCvRuntimeEditor.Behaviours
{
    using System.Windows;
    using System.Windows.Controls;
    using Core;
    using Core.NodeSpawn;
    using Core.ProcessingStrategies;
    using Microsoft.Xaml.Behaviors;
    using Utils;
    using ViewModels;

    public class VariableDragBehaviour : Behavior<Grid>
    {
        public static VariableDragBehaviour Instance;

        public VariableDragBehaviour()
        {
            Instance = this;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Drop += AssociatedObjectOnDrop;
            AssociatedObject.AllowDrop = true;
        }

        private void AssociatedObjectOnDrop(object sender, DragEventArgs e)
        {
            var variableViewModel = (VariableViewModel)e.Data.GetData(typeof(VariableViewModel));

            if (variableViewModel == null)
                return;//in case if s1 drop something that is not a variable

            var currentMousePos = e.GetPosition(AssociatedObject);
            CreateVariableNode(variableViewModel, currentMousePos);
        }

        public void CreateVariableNode(VariableViewModel variable, Point currentMousePos)
        {
            var nodePos = CoordsTransformUtils.GlobalToCanvas(currentMousePos);

            var nodeConfig = new NodeCreationConfig(variable.Name, new DefaultNodeSpawnStrategy(null, new[]
            {
                new NodePinSpawnConfig(string.Empty, variable.Type)
            }), typeof(VariableProcessingStrategy));

            nodeConfig.VariableRefId = variable.Id;

            NodesCanvasViewModel.Current.AddNode(nodeConfig, nodePos);
        }
    }
}
