namespace OpenCvRuntimeEditor.ViewModels
{
    using System;
    using System.Linq;
    using Core;
    using Core.ProcessingStrategies;
    using PinDataVisualProcessors.Base;

    public class VariableViewModel : BaseViewModel, IRuntimeDataContainer
    {
        private object _data;
        private string _name;

        public VariableViewModel()
        {
        }

        public VariableViewModel(string name, object data, int id, Type type)
        {
            _name = name;
            _data = data;
            Id = id;
            Type = type;
        }

        public int Id { get; set; }
        public Type Type { get; set; }
        public BasePinVisualDataProcessor VisualDataProcessor { get; set; }

        public object Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public T GetData<T>()
        {
            if (_data == null)
                return default;

            return (T) _data;
        }

        public void SetData(object data, bool recalculate = false)
        {
            _data = data;
            UpdateNodes();
        }

        private void UpdateNodes()
        {
            var nodesToUpdate =
                NodesCanvasViewModel.Current.Nodes.Where(x => x.ProcessingStrategy is VariableProcessingStrategy && x.VariableRefId == Id);

            Pipeline.Recalculate(nodesToUpdate.ToList());
        }
    }
}
