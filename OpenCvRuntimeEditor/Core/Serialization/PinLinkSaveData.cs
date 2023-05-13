namespace OpenCvRuntimeEditor.Core.Serialization
{
    using Newtonsoft.Json;
    using ViewModels;

    public class PinLinkSaveData
    {
        [JsonConstructor]
        public PinLinkSaveData()
        {
        }

        public PinLinkSaveData(PinLinkViewModel link)
        {
            FromNode = link.OutPin.Owner.Id;
            ToNode = link.InPin.Owner.Id;
            OutPinIndex = link.OutPin.Owner.OutPins.IndexOf(link.OutPin);
            InPinIndex = link.InPin.Owner.InPins.IndexOf(link.InPin);
        }

        public int FromNode { get; set; }
        public int ToNode { get; set; }
        public int OutPinIndex { get; set; }
        public int InPinIndex { get; set; }
    }
}