namespace OpenCvRuntimeEditor.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Forms;
    using Behaviours;
    using Serialization;
    using Utils;
    using ViewModels;

    public static class CopyPaste
    {
        private static CanvasSaveData _saveData;
        private static Point _mousePos;

        public static void CopyNodes(List<NodeViewModel> nodes)
        {
            var mousePos = Control.MousePosition;
            _mousePos = CoordsTransformUtils.GlobalToCanvas(new Point(mousePos.X, mousePos.Y));
            _saveData = new CanvasSaveData();

            foreach (var node in nodes)
            {
                _saveData.Nodes.Add(new NodeSaveData(node, _saveData));
            }
        }

        public static void PasteNodes()
        {
            if (_saveData == null)
                return;

            var mousePosD = Control.MousePosition;
            var mousePos = CoordsTransformUtils.GlobalToCanvas(new Point(mousePosD.X, mousePosD.Y));

            var newNodesDict = new Dictionary<int, NodeViewModel>();
            var oldNodesDict = new HashSet<int>();
            var nodeIdRedirect = new Dictionary<int, int>();
            var mouseOffset = _mousePos - mousePos;

            foreach (var node in _saveData.Nodes)
            {
                oldNodesDict.Add(node.Id);
                var nodeVewModel = new NodeViewModel();
                node.Setup(nodeVewModel, _saveData);
                var newId = NodesCanvasViewModel.Current.GetUniqNodeId();
                nodeIdRedirect.Add(nodeVewModel.Id, newId);
                nodeVewModel.Id = newId;
                nodeVewModel.Pos -= mouseOffset;
                NodesCanvasViewModel.Current.AddCanvasNode(nodeVewModel);
                NodesCanvasViewModel.Current.Nodes.Add(nodeVewModel);
                newNodesDict.Add(nodeVewModel.Id, nodeVewModel);
            }

            foreach (var link in _saveData.Links)
            {
                if (oldNodesDict.Contains(link.FromNode) && oldNodesDict.Contains(link.ToNode))
                {
                    var newFromId = nodeIdRedirect[link.FromNode];
                    var newToId = nodeIdRedirect[link.ToNode];
                    var fromNode = newNodesDict[newFromId];
                    var toNode = newNodesDict[newToId];

                    var fromPin = fromNode.OutPins[link.OutPinIndex];
                    var toPin = toNode.InPins[link.InPinIndex];

                    NodeLinkingBehaviour.LinkPins(toPin, fromPin);
                }
            }

            Pipeline.Recalculate(newNodesDict.Values.ToList());
        }
    }
}
