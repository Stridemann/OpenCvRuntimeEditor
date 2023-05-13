namespace OpenCvRuntimeEditor.Core.Serialization
{
    using System.Collections.Generic;
    using ViewModels;

    public class SaveLoadResult
    {
        public NodesCanvasViewModel CanvasViewModel { get; set; }
        public List<PinLinkViewModel> Links { get; } = new List<PinLinkViewModel>();
    }
}