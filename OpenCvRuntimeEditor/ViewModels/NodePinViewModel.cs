namespace OpenCvRuntimeEditor.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using Core;
    using PinDataVisualProcessors.Base;

    public class NodePinViewModel : BaseViewModel
    {
        private Point _pos;

        internal NodePinViewModel()
        {
            PinData = new PinData(this);
        }

        public NodePinViewModel(string name,
            Type pinType,
            PinDirection direction,
            NodeViewModel owner,
            bool isOptional = false) : this()
        {
            Name = name;
            Owner = owner;
            Direction = direction;
            PinType = pinType;
            IsOptional = isOptional;
        }

        public string Name { get; set; }
        public bool IsOptional { get; set; }
        public PinDirection Direction { get; set; }
        public NodeViewModel Owner { get; internal set; }
        public BasePinVisualDataProcessor VisualDataProcessor { get; set; }
        public bool IsLinked => Links.Count > 0;
        public ObservableCollection<PinLinkViewModel> Links { get; set; } = new ObservableCollection<PinLinkViewModel>();
        public ErrorInfo ErrorInfo { get; } = new ErrorInfo();
        public Type PinType { get; internal set; }
        public PinData PinData { get; }
        public bool IsVisible { get; set; } = true;

        public Point Pos
        {
            get => _pos;
            set
            {
                _pos = value;
                OnPosChanged();
            }
        }

        public event Action OnPosChanged = delegate { };
    }

    public enum PinDirection
    {
        Undefined,
        In,
        Out
    }
}
