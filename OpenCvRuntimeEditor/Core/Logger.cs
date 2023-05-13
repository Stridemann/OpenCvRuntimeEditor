namespace OpenCvRuntimeEditor.Core
{
    using System.Windows;

    public static class Logger
    {
        public static void LogMessage(string message)
        {
            //TODO: Implement me
        }

        public static void LogWarning(string message)
        {
            //TODO: Implement me
        }

        public static void LogError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void LogError(string message, params object[] args)
        {
            LogError(string.Format(message, args));
        }
    }
}
