namespace OpenCvRuntimeEditor.ViewModels.PinDataVisualProcessors
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using Core;
    using Microsoft.Win32;
    using Prism.Commands;

    public class SelectFilePathVisualDataProcessor : StringPinVisualDataProcessor
    {
        private ICommand _selectFileCommand;

        public SelectFilePathVisualDataProcessor(IRuntimeDataContainer dataContainer) : base(dataContainer)
        {
        }

        public ICommand SelectFileCommand => _selectFileCommand ?? (_selectFileCommand = new DelegateCommand(OnSelectFile));

        private void OnSelectFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false
            };

            var dialogResult = openFileDialog.ShowDialog(Application.Current.MainWindow);

            if (!dialogResult.Value)
                return;



            Value = Path.GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, openFileDialog.FileName);
            //openFileDialog.FileName.Replace(AppDomain.CurrentDomain.BaseDirectory, string.Empty);
        }
    }
}
