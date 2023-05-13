namespace OpenCvRuntimeEditor.Core.Serialization.JsonConverters
{
    using System;
    using Newtonsoft.Json;
    using Utils;

    public class ObjectJsonConverter : JsonConverter
    {
        private const string TYPE_CONSTRAINT = "%Type%";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dataObj = (JsoSerializableObjectContainer) value;

            if (dataObj?.Data == null)
                return;

            if (dataObj.Data is Type type)
            {
                dataObj.ObjectTypeName = type.FullName;
                dataObj.ObjectTypeAssemblyName = type.Assembly.GetName().Name;
                dataObj.ObjectTypeData = TYPE_CONSTRAINT;
            }
            else
            {
                var objType = dataObj.Data.GetType();

                dataObj.ObjectTypeName = objType.FullName;
                dataObj.ObjectTypeAssemblyName = objType.Assembly.GetName().Name;
                dataObj.ObjectTypeData = JsonConvert.SerializeObject(dataObj.Data);
            }

            var ser = JsonSerializer.Create();
            ser.Serialize(writer, dataObj);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonSerializer = JsonSerializer.Create();
            var container = jsonSerializer.Deserialize<JsoSerializableObjectContainer>(reader);

            if (container != null)
            {
                var deserializationType = TypeLoadUtils.FindType(container.ObjectTypeName, container.ObjectTypeAssemblyName);

                if (container.ObjectTypeData == TYPE_CONSTRAINT)
                {
                    container.Data = deserializationType;
                }
                else
                {
                    if (deserializationType == null)
                    {
                        Logger.LogError(
                            $"Can't find type for object deserialization {container.ObjectTypeName}"); //Probably type was deleted from code
                    }
                    else
                    {
                        var myObject = JsonConvert.DeserializeObject(container.ObjectTypeData, deserializationType);
                        container.Data = myObject;
                    }
                }

                return container;
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JsoSerializableObjectContainer);
        }
    }
}
