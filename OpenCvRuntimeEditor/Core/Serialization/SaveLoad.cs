namespace OpenCvRuntimeEditor.Core.Serialization
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using ViewModels;

    internal static class SaveLoad
    {
        public static void Save(NodesCanvasViewModel canvas, string filePath)
        {
            var data = new CanvasSaveData();

            foreach (var node in canvas.Nodes)
            {
                data.Nodes.Add(new NodeSaveData(node, data));
            }

            foreach (var variable in canvas.Variables)
            {
                data.Variables.Add(new VariableSaveData(variable, data));
            }

            var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }

        public static SaveLoadResult Load(string filePath)
        {
            var canvas = new NodesCanvasViewModel();
            var saveLoadResult = new SaveLoadResult {CanvasViewModel = canvas};
            var jsonDataStr = File.ReadAllText(filePath);
            var canvasData = JsonConvert.DeserializeObject<CanvasSaveData>(jsonDataStr);
            var nodes = new Dictionary<int, NodeViewModel>();

            foreach (var node in canvasData.Nodes)
            {
                var nodeVewModel = new NodeViewModel();
                node.Setup(nodeVewModel, canvasData);
                canvas.Nodes.Add(nodeVewModel);
                nodes.Add(nodeVewModel.Id, nodeVewModel);
            }

            foreach (var link in canvasData.Links)
            {
                var fromNode = nodes[link.FromNode];
                var toNode = nodes[link.ToNode];

                var outPin = fromNode.OutPins[link.OutPinIndex];
                var inPin = toNode.InPins[link.InPinIndex];

                var newLink = new PinLinkViewModel(outPin, inPin);
                outPin.Links.Add(newLink);
                inPin.Links.Add(newLink);
                saveLoadResult.Links.Add(newLink);
            }

            foreach (var variable in canvasData.Variables)
            {
                var newVar = new VariableViewModel();
                variable.Setup(newVar, canvasData);
                canvas.Variables.Add(newVar);
            }

            return saveLoadResult;
        }
    }
}
