namespace OpenCvRuntimeEditor.Behaviours
{
    using System.Windows;
    using System.Windows.Input;

    //public class PinLinkingBehaviour : Behavior<FrameworkElement>
    //{
    //    protected override void OnAttached()
    //    {
    //        AssociatedObject.MouseDown += AssociatedObjectOnMouseDown;
    //        AssociatedObject.MouseUp += AssociatedObjectOnMouseUp;
    //    }

    //    private void AssociatedObjectOnMouseDown(object sender, MouseButtonEventArgs e)
    //    {
    //        NodeLinkingBehaviour.Instance.PrepareLinkPins((NodePinViewModel)AssociatedObject.DataContext);
    //    }

    //    private void AssociatedObjectOnMouseUp(object sender, MouseButtonEventArgs e)
    //    {
    //        NodeLinkingBehaviour.Instance.FinishLinkPins((NodePinViewModel)AssociatedObject.DataContext);
    //    }

    //    protected override void OnDetaching()
    //    {
    //        AssociatedObject.MouseDown -= AssociatedObjectOnMouseDown;
    //        AssociatedObject.MouseUp -= AssociatedObjectOnMouseUp;
    //    }
    //}

    public static class PinLinkingBehaviour
    {
        #region Dependecy Property

        private static readonly DependencyProperty MouseDownCommandProperty = DependencyProperty.RegisterAttached
        (
            "MouseDownCommand",
            typeof(ICommand),
            typeof(PinLinkingBehaviour),
            new PropertyMetadata(MouseDownCommandPropertyChangedCallBack)
        );

        public static void SetMouseDownCommand(this UIElement inUIElement, ICommand inCommand)
        {
            inUIElement.SetValue(MouseDownCommandProperty, inCommand);
        }

        private static ICommand GetMouseDownCommand(UIElement inUIElement)
        {
            return (ICommand)inUIElement.GetValue(MouseDownCommandProperty);
        }

        #endregion

        #region CallBack Method

        private static void MouseDownCommandPropertyChangedCallBack(DependencyObject inDependencyObject,
            DependencyPropertyChangedEventArgs inEventArgs)
        {
            UIElement uiElement = inDependencyObject as UIElement;
            if (null == uiElement) return;

            uiElement.PreviewMouseLeftButtonDown += (sender, args) =>
            {
                GetMouseDownCommand(uiElement).Execute(args);
                args.Handled = true;
            };
        }

        #endregion
    }
}
