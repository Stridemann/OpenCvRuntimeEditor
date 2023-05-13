namespace OpenCvRuntimeEditor.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using Core;
    using Core.ProcessingStrategies;
    using Nodes;
    using Prism.Commands;
    using Styles.TemplateSelectors;
    using WIndows;

    public class NodeViewModel : BaseGridObject, ITemplateType
    {
        private bool _isSelected;
        private string _name;
        private ICommand _openPreviewWindowCommand;

        public NodeViewModel()
        {
            PipelineData = new PipelineData(this);
        }

        public NodeViewModel(NodeCreationConfig creationConfig) : this()
        {
            Name = creationConfig.NodeName;
            TemplateKey = creationConfig.TemplateKey;
            IsPreviewOpened = creationConfig.IsPreviewOpened;
        }

        public IProcessingStrategy ProcessingStrategy { get; internal set; }
        public PipelineData PipelineData { get; }
        public int Id { get; internal set; }
        public ErrorInfo ErrorInfo { get; } = new ErrorInfo();
        public ObservableCollection<NodePinViewModel> InPins { get; set; } = new ObservableCollection<NodePinViewModel>();
        public ObservableCollection<NodePinViewModel> OutPins { get; set; } = new ObservableCollection<NodePinViewModel>();
        internal bool IsDeletingNode { get; set; }
        public bool IsPreviewOpened { get; set; }
        public ICommand OpenPreviewWindowCommand =>
            _openPreviewWindowCommand ?? (_openPreviewWindowCommand = new DelegateCommand(OnOpenPreviewWindow));
        public int VariableRefId { get; set; } = -1;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public string TemplateKey { get; internal set; }

        private void OnOpenPreviewWindow()
        {
            var window = new NodeResultPreviewWindow();
            window.DataContext = this;
            window.Show();
            window.Width = PipelineData.PreviewImage.Width;
            window.Height = PipelineData.PreviewImage.Height;
        }

        public T GetInPinData<T>(int pinIndex)
        {
            return InPins[pinIndex].PinData.GetData<T>();
        }

        public void SetInPinData(int pinIndex, object data)
        {
            InPins[pinIndex].PinData.SetData(data);
        }

        public T GetOutPinData<T>(int pinIndex)
        {
            return OutPins[pinIndex].PinData.GetData<T>();
        }

        public void SetOutPinData(int pinIndex, object data)
        {
            OutPins[pinIndex].PinData.SetData(data);
        }
    }
}
