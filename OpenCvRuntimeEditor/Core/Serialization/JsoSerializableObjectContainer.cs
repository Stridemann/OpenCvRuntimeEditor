namespace OpenCvRuntimeEditor.Core.Serialization
{
    using Newtonsoft.Json;

    public class JsoSerializableObjectContainer
    {
        public string ObjectTypeAssemblyName { get; set; }
        public string ObjectTypeName { get; set; }
        public string ObjectTypeData { get; set; }
        [JsonIgnore]
        public object Data { get; set; }
    }
}
