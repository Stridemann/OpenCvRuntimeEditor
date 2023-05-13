using OpenCvRuntimeEditor.Core;
using OpenCvRuntimeEditor.Settings;
using OpenCvRuntimeEditor.Utils;
using OpenCvRuntimeEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpenCvRuntimeEditor
{
    using System.IO;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        private bool _layoutUpdatedOnce;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            ContentRendered += OnLayoutUpdated;
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            if (_layoutUpdatedOnce)
                return;

            _layoutUpdatedOnce = true;

            if (!string.IsNullOrEmpty(GeneralSettings.Instance.LastFilePath))
            {
                if (File.Exists(GeneralSettings.Instance.LastFilePath))
                {
                    var vm = (MainWindowViewModel)DataContext;

                    try
                    {
                        vm.LoadFile(GeneralSettings.Instance.LastFilePath);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Failed to load previous file: {ex.Message}");
                    }
                }
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            GeneralSettings.Save();
        }

        private void AddVariableClick(object sender, RoutedEventArgs e)
        {
            var contextMenu = new ContextMenu(); //todo: make better way

            foreach (var type in NodeDatabase.NumericalTypeViewModels)
            {
                AddMenu(type, contextMenu);
            }

            AddMenu(typeof(string), contextMenu);
            contextMenu.PlacementTarget = sender as Button;
            contextMenu.IsOpen = true;
        }

        private void AddMenu(Type type, ContextMenu menu)
        {
            var item = new MenuItem { Header = TypeRenamingUtils.RenameType(type) };
            item.Click += delegate { NodesCanvasViewModel.Current.AddVariable(type); };
            menu.Items.Add(item);
        }
    }
}
