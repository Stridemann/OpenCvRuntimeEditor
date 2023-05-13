namespace OpenCvRuntimeEditor.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Behaviours;
    using Controls;
    using Core;
    using Core.ProcessingStrategies;
    using Core.Serialization;
    using Microsoft.Xaml.Behaviors;
    using PinDataVisualProcessors.Base;
    using Prism.Commands;
    using Utils;

    public class NodesCanvasViewModel : BaseViewModel
    {
        public NodesCanvasViewModel()
        {
            Current = this;
        }

        public static NodesCanvasViewModel Current { get; private set; }
        public ObservableCollection<NodeViewModel> Nodes { get; set; } = new ObservableCollection<NodeViewModel>();
        public ObservableCollection<VariableViewModel> Variables { get; set; } = new ObservableCollection<VariableViewModel>();

        #region SaveLoad

        public void DoAfterLoad(SaveLoadResult result)
        {
            MainWindow.Instance.NodesRoot.Children.Clear();
            MainWindow.Instance.NodesLinksRoot.Children.Clear();

            if (NodeLinkingBehaviour.Instance.VisualHelperLink != null)
                MainWindow.Instance.NodesLinksRoot.Children.Add(NodeLinkingBehaviour.Instance.VisualHelperLink.Path);

            foreach (var nodeViewModel in Nodes)
            {
                AddCanvasNode(nodeViewModel);
            }

            foreach (var link in result.Links)
            {
                AddLink(link);
            }

            Pipeline.Recalculate(Nodes.ToList());
        }



        #endregion

        #region Nodes


        public NodeViewModel AddNode(NodeCreationConfig nodeCreationConfig, Point pos)
        {
            var id = GetUniqNodeId();

            var nodeViewModel = NodeFactory.CreateNode(nodeCreationConfig, id);
            AddCanvasNode(nodeViewModel);
            Nodes.Add(nodeViewModel);
            nodeViewModel.Pos = pos;
            Pipeline.Recalculate(nodeViewModel);
            return nodeViewModel;
        }
        
        public void AddCanvasNode(NodeViewModel nodeViewModel)
        {
            var nodeView = new NodeControl
            {
                DataContext = nodeViewModel
            };

            MainWindow.Instance.NodesRoot.Children.Add(nodeView);
        }

        public void AddLink(PinLinkViewModel linkViewModel)
        {
            linkViewModel.Path.DataContext = linkViewModel;
            MainWindow.Instance.NodesLinksRoot.Children.Add(linkViewModel.Path);
            var behaviours = Interaction.GetBehaviors(linkViewModel.Path);
            behaviours.Add(new LinkBehaviour());

            ZoomBorder.OnTrnasformChanged += linkViewModel.UpdateSplinePoints;
        }

        public void DeleteNodes(List<NodeViewModel> nodes)
        {
            nodes.ForEach(x => x.IsDeletingNode = true);

            var recalculatingNodes = new List<NodeViewModel>();

            foreach (var node in nodes)
            {
                foreach (var pin in node.InPins)
                {
                    foreach (var link in pin.Links)
                    {
                        if (!link.OutPin.Owner.IsDeletingNode)
                            DeleteLink(link);
                    }
                }

                foreach (var pin in node.OutPins)
                {
                    foreach (var link in pin.Links)
                    {
                        if (!link.InPin.Owner.IsDeletingNode)
                        {
                            recalculatingNodes.Add(link.InPin.Owner);
                            DeleteLink(link);
                        }
                    }
                }

                Nodes.Remove(node);
            }

            var viewsToDelete = new List<UIElement>();

            foreach (UIElement child in MainWindow.Instance.NodesRoot.Children)
            {
                if (child is NodeControl nodeView && nodes.Contains(nodeView.DataContext))
                    viewsToDelete.Add(child);
            }

            viewsToDelete.ForEach(x => MainWindow.Instance.NodesRoot.Children.Remove(x));

            Pipeline.Recalculate(recalculatingNodes.Distinct().ToList());
        }

        public void DeleteLinks(List<PinLinkViewModel> links)
        {
            links.ForEach(DeleteLink);
            var nodesToRecalculate = links.Select(x => x.InPin.Owner).Distinct().ToList();
            Pipeline.Recalculate(nodesToRecalculate);
        }

        public void DeleteLink(PinLinkViewModel link)
        {
            if (!link.InPin.Owner.IsDeletingNode)
                link.InPin.Links.Remove(link);

            if (!link.OutPin.Owner.IsDeletingNode)
                link.OutPin.Links.Remove(link);

            ZoomBorder.OnTrnasformChanged += link.UpdateSplinePoints;
            MainWindow.Instance.NodesLinksRoot.Children.Remove(link.Path);
        }

        public int GetUniqNodeId()
        {
            var ordered = Nodes.Select(x => x.Id).OrderBy(x => x);
            var orderedArray = ordered.ToArray();

            if (orderedArray.Length > 0 && orderedArray.Last() == orderedArray.Length - 1)
                return orderedArray.Length;

            return FindFirstMissingId(orderedArray, 0, orderedArray.Length - 1);
        }

        private int FindFirstMissingId(int[] array, int start, int end)
        {
            if (start > end)
                return end + 1;

            if (start != array[start])
                return start;

            var mid = (start + end) / 2;

            if (array[mid] == mid)
                return FindFirstMissingId(array, mid + 1, end);

            return FindFirstMissingId(array, start, mid);
        }

        #endregion

        #region Variables

        private ICommand _addVariableCommand;

        private void OnAddVariable()
        {
        }

        public ICommand AddVariableCommand => _addVariableCommand ?? (_addVariableCommand = new DelegateCommand(OnAddVariable));
        private ICommand _removeVariableCommand;
        public ICommand RemoveVariableCommand =>
            _removeVariableCommand ?? (_removeVariableCommand = new DelegateCommand<VariableViewModel>(OnRemoveVariable));

        private void OnRemoveVariable(VariableViewModel variable)
        {
            Variables.Remove(variable);

            var nodesToDelete = Nodes.Where(x => x.ProcessingStrategy is VariableProcessingStrategy &&
                                                 x.VariableRefId == variable.Id).ToList();

            DeleteNodes(nodesToDelete);
        }

        public void AddVariable(Type type)
        {
            var data = ReflectionUtils.GetDefaultValue(type);
            var id = GetUniqVariableId();
            var dataProcessorType = NodeDatabase.GetRegisteredPinDataViewModelForType(type);
            var newVar = new VariableViewModel($"{TypeRenamingUtils.RenameType(type)}_var", data, id, type);
            var pinDataViewModelInstance = (BasePinVisualDataProcessor) Activator.CreateInstance(dataProcessorType, newVar);
            newVar.VisualDataProcessor = pinDataViewModelInstance;
            Variables.Add(newVar);
        }

        private int GetUniqVariableId()
        {
            var ordered = Variables.Select(x => x.Id).OrderBy(x => x);
            var orderedArray = ordered.ToArray();

            if (orderedArray.Length > 0 && orderedArray.Last() == orderedArray.Length - 1)
                return orderedArray.Length;

            return FindFirstMissingId(orderedArray, 0, orderedArray.Length - 1);
        }

        #endregion
    }
}
