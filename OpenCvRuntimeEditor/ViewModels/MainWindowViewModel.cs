using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;

namespace OpenCvRuntimeEditor.ViewModels
{
    using System.IO;
    using System.Windows;
    using Core;
    using Core.Serialization;
    using Microsoft.Win32;
    using Prism.Mvvm;
    using Settings;
    using Utils;

    public class MainWindowViewModel : BindableBase
    {
        public const string WINDOW_TITLE = "CvEditor: ";
        public const string FILE_EXTENSION = ".cpe";
        public const string FILE_FILTER = "CvEditor files|*" + FILE_EXTENSION;
        private string _currentFilePath;
        private ICommand _loadCommand;
        private ICommand _loadFileCommand;
        private NodesCanvasViewModel _nodesCanvas;
        private ICommand _saveAsCommand;
        private ICommand _saveCommand;
        private string _wIndowTitle;
        public ObservableCollection<LastFile> LastOpenedFiles => GeneralSettings.Instance.LastOpenedFiles;

        public MainWindowViewModel()
        {
            GeneralSettings.Load();
            GeneralSettings.Instance.LastOpenedFiles.RemoveAll(x => !File.Exists(x.FilePath));
            NodeDatabase.Load();
            NodesCanvas = new NodesCanvasViewModel();
        }

        public string CurrentFilePath
        {
            get => _currentFilePath;
            set => SetProperty(ref _currentFilePath, value);
        }

        public string WIndowTitle
        {
            get => $"{WINDOW_TITLE}{_wIndowTitle}";
            set => SetProperty(ref _wIndowTitle, value);
        }

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(OnSave));
        public ICommand LoadCommand => _loadCommand ?? (_loadCommand = new DelegateCommand(OnLoad));
        public ICommand SaveAsCommand => _saveAsCommand ?? (_saveAsCommand = new DelegateCommand(SaveAs));
        public ICommand LoadFileCommand => _loadFileCommand ?? (_loadFileCommand = new DelegateCommand<string>(LoadFile));

        public NodesCanvasViewModel NodesCanvas
        {
            get => _nodesCanvas;
            set => SetProperty(ref _nodesCanvas, value);
        }

        private void OnSave()
        {
            if (!string.IsNullOrEmpty(CurrentFilePath))
                SaveLoad.Save(NodesCanvas, CurrentFilePath);
            else
                SaveAs();
        }

        private void SaveAs()
        {
            var saveDialog = new SaveFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Filter = FILE_FILTER
            };

            if (saveDialog.ShowDialog(Application.Current.MainWindow).Value)
            {
                SaveLoad.Save(NodesCanvas, saveDialog.FileName);
                UpdateCurrentFileOpened(saveDialog.FileName);
            }
        }

        private void OnLoad()
        {
            var fileDialog = new OpenFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Multiselect = false,
                Filter = FILE_FILTER
            };

            if (fileDialog.ShowDialog(Application.Current.MainWindow).Value)
                LoadFile(fileDialog.FileName);
        }

        public void LoadFile(string filePath)
        {
            var result = SaveLoad.Load(filePath);
            NodesCanvas = result.CanvasViewModel;
            NodesCanvas.DoAfterLoad(result);
            UpdateCurrentFileOpened(filePath);
        }

        private void UpdateCurrentFileOpened(string filePath)
        {
            CurrentFilePath = filePath;
            WIndowTitle = Path.GetFileName(filePath);
            GeneralSettings.Instance.LastFilePath = filePath;

            var containIndex = GeneralSettings.Instance.LastOpenedFiles.IndexOf(x => x.FilePath == filePath);

            if (containIndex != -1)
            {
                GeneralSettings.Instance.LastOpenedFiles.Move(containIndex, 0);
            }
            else
            {
                GeneralSettings.Instance.LastOpenedFiles.Insert(0, new LastFile
                {
                    FilePath = filePath,
                    FileName = Path.GetFileNameWithoutExtension(filePath)
                });
            }
        }
    }
}
