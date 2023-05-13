namespace OpenCvRuntimeEditor.Core.Serialization
{
    using System.Collections.Generic;

    internal class CanvasSaveData
    {
        public List<PinLinkSaveData> Links = new List<PinLinkSaveData>();
        public List<NodeSaveData> Nodes = new List<NodeSaveData>();
        public List<VariableSaveData> Variables = new List<VariableSaveData>();
        public TypeSerializationContainer TypeSerializationContainer = new TypeSerializationContainer();
    }
}
