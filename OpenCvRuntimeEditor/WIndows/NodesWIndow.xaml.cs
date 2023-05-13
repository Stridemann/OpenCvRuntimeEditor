namespace OpenCvRuntimeEditor.WIndows
{
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for NodesWIndow.xaml
    /// </summary>
    public partial class NodesWIndow : Window
    {
        public static NodesWIndow Instance;
        public NodesWIndow()
        {
            Instance = this;
            InitializeComponent();
        }

        public void SetFocusToInputTextBox()
        {
            Keyboard.Focus(TextInputBox);//TODO: Fixme
            TextInputBox.Focus();
            //FocusManager.SetFocusedElement(TextInputBox, TextInputBox);
        }
    }
}
