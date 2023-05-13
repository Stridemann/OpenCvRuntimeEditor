namespace OpenCvRuntimeEditor.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Behaviours;
    using Core;
    using Prism.Commands;
    using Utils;
    using WIndows;

    public class NodesWindowViewModel : BaseViewModel
    {
        private ObservableCollection<NodeCreationConfig> _availableNodes;
        private ICommand _enterPressedCommand;
        private string _filterInput = string.Empty;
        private ICommand _navigateDownCommand;
        private ICommand _navigateUpCommand;
        private ICommand _nodeSelectedCommand;
        private NodeCreationConfig _selectedConfig;

        public ObservableCollection<NodeCreationConfig> AvailableNodes
        {
            get => _availableNodes;
            set => SetProperty(ref _availableNodes, value);
        }

        public ICommand NodeSelectedCommand => _nodeSelectedCommand ??
                                               (_nodeSelectedCommand = new DelegateCommand<NodeCreationConfig>(OnNodeSelectedCommand));

        public NodeCreationConfig SelectedConfig
        {
            get => _selectedConfig;
            set => SetProperty(ref _selectedConfig, value);
        }

        public string FilterInput
        {
            get => _filterInput;
            set
            {
                if (string.IsNullOrEmpty(value))
                    value = string.Empty;

                SetProperty(ref _filterInput, value);
                FilterNodes();
            }
        }

        public ICommand EnterPressedCommand => _enterPressedCommand ?? (_enterPressedCommand = new DelegateCommand(OnEnterPressed));
        public ICommand NavigateDownCommand => _navigateDownCommand ?? (_navigateDownCommand = new DelegateCommand(OnNavigateDown));
        public ICommand NavigateUpCommand => _navigateUpCommand ?? (_navigateUpCommand = new DelegateCommand(OnNavigateUp));

        private void OnEnterPressed()
        {
            if (SelectedConfig != null)
            {
                SelectedConfig.IsSelectedInNodesList = Visibility.Collapsed;
                OnNodeSelected(SelectedConfig);
            }
        }

        public Action<NodeCreationConfig> OnNodeSelected = delegate { };

        private void OnNodeSelectedCommand(NodeCreationConfig nodeCreationConfig)
        {
            OnNodeSelected(nodeCreationConfig);
        }

        private void OnNavigateDown()
        {
            if (SelectedConfig == null)
                return;

            if (SelectedConfig != null)
                SelectedConfig.IsSelectedInNodesList = Visibility.Collapsed;

            if (AvailableNodes.Count == 0)
            {
                return;
            }

            var index = AvailableNodes.IndexOf(SelectedConfig);
            index++;

            if (index < AvailableNodes.Count)
            {
                SelectedConfig = AvailableNodes[index];
            }
            else
            {
                SelectedConfig = AvailableNodes.First();
            }

            SelectedConfig.IsSelectedInNodesList = Visibility.Visible;
        }

        private void OnNavigateUp()
        {
            if (SelectedConfig == null)
                return;

            if (SelectedConfig != null)
                SelectedConfig.IsSelectedInNodesList = Visibility.Collapsed;

            if (AvailableNodes.Count == 0)
            {
                return;
            }

            var index = AvailableNodes.IndexOf(SelectedConfig);
            index--;

            if (index >= 0)
            {
                SelectedConfig = AvailableNodes[index];
            }
            else
            {
                SelectedConfig = AvailableNodes.Last();
            }

            SelectedConfig.IsSelectedInNodesList = Visibility.Visible;
        }

        public void OnClosing()
        {
            if (SelectedConfig != null)
                SelectedConfig.IsSelectedInNodesList = Visibility.Collapsed;

            SelectedConfig = null;
        }

        public void OnOpening()
        {
            _filterInput = string.Empty;
            RaisePropertyChanged(nameof(FilterInput));
            NodesWIndow.Instance.SetFocusToInputTextBox();

            FilterNodes();
        }

        private void FilterNodes()
        {
            if (SelectedConfig != null)
            {
                SelectedConfig.IsSelectedInNodesList = Visibility.Collapsed;
                SelectedConfig = null;
            }

            if (NodeLinkingBehaviour.Instance.LinkingPin == null && string.IsNullOrEmpty(_filterInput))
            {
                AvailableNodes = NodeDatabase.RegisteredNodes;
            }
            else if (!string.IsNullOrEmpty(_filterInput))
            {
                if (NodeLinkingBehaviour.Instance.LinkingPin != null)
                {
                    Func<NodeCreationConfig, Type, bool> predicate;

                    if (NodeLinkingBehaviour.Instance.LinkingPin.Direction == PinDirection.In)
                        predicate = FilterByOutPin;
                    else
                        predicate = FilterByInPin;

                    var filterPinType = NodeLinkingBehaviour.Instance.LinkingPin.PinType;

                    var inputUpper = _filterInput.ToUpper();
                    var filtered = NodeDatabase.RegisteredNodes.Where(x => x.FilterUpperName.Contains(inputUpper) && predicate(x, filterPinType));
                    AvailableNodes = new ObservableCollection<NodeCreationConfig>(filtered);
                }
                else
                {
                    var inputUpper = _filterInput.ToUpper();
                    var filtered = NodeDatabase.RegisteredNodes.Where(x => x.FilterUpperName.Contains(inputUpper));
                    AvailableNodes = new ObservableCollection<NodeCreationConfig>(filtered);
                }
            }
            else if (NodeLinkingBehaviour.Instance.LinkingPin != null)
            {
                Func<NodeCreationConfig, Type, bool> predicate;

                if (NodeLinkingBehaviour.Instance.LinkingPin.Direction == PinDirection.In)
                    predicate = FilterByOutPin;
                else
                    predicate = FilterByInPin;

                var filterPinType = NodeLinkingBehaviour.Instance.LinkingPin.PinType;

                var filtered = NodeDatabase.RegisteredNodes.Where(x => predicate(x, filterPinType));
                AvailableNodes = new ObservableCollection<NodeCreationConfig>(filtered);
            }

            SelectedConfig = AvailableNodes.FirstOrDefault();

            if (SelectedConfig != null)
                SelectedConfig.IsSelectedInNodesList = Visibility.Visible;
        }

        private bool FilterByInPin(NodeCreationConfig config, Type outPinType)
        {
            if (config.SpawnStraregy.InPinSpawnConfigs == null)
                return false;

            return config.SpawnStraregy.InPinSpawnConfigs.Any(x => TypeLinkingUtils.CheckPinsLinking(outPinType, x.PinType).Allowed);
        }

        private bool FilterByOutPin(NodeCreationConfig config, Type inPinType)
        {
            if (config.SpawnStraregy.OutPinSpawnConfigs == null)
                return false;

            return config.SpawnStraregy.OutPinSpawnConfigs.Any(x => TypeLinkingUtils.CheckPinsLinking(x.PinType, inPinType).Allowed);
        }
    }
}
