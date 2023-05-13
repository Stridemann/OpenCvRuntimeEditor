namespace OpenCvRuntimeEditor.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    public class ShittyBindingHelper : Control
    {
        public static readonly DependencyProperty InPositionProperty = 
            DependencyProperty.Register(nameof(InPosition), typeof(Point), typeof(ShittyBindingHelper),
            new PropertyMetadata(default(Point), NodePositionChangedCallback));

        public static readonly DependencyProperty OutPositionProperty = 
            DependencyProperty.Register(nameof(OutPosition), typeof(Point), typeof(ShittyBindingHelper),
            new PropertyMetadata(default(Point), NodePositionChangedCallback));

        public Point InPosition
        {
            get => (Point) GetValue(InPositionProperty);
            set => SetValue(InPositionProperty, value);
        }

        public Point OutPosition
        {
            get => (Point) GetValue(OutPositionProperty);
            set => SetValue(OutPositionProperty, value);
        }

        private static void NodePositionChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var helper = (ShittyBindingHelper) d;
            helper.OutPosition = helper.InPosition;
        }
    }
}
