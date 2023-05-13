namespace OpenCvRuntimeEditor.Core
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class ErrorInfo
    {
        private readonly ObservableCollection<ErrorMessage> _messages = new ObservableCollection<ErrorMessage>();
        public IReadOnlyCollection<ErrorMessage> Messages => _messages;

        public void AddPipelineError(string message)
        {
            _messages.Add(new ErrorMessage(message, ErrorInfoType.Pipeline));
        }

        internal void AddError(string message, ErrorInfoType type)
        {
            _messages.Add(new ErrorMessage(message, type));
        }

        public void ClearMessages(ErrorInfoType ofType)
        {
            var messagesToRemove = _messages.Where(x => x.Type == ofType).ToList();
            foreach (var errorMessage in messagesToRemove)
            {
                _messages.Remove(errorMessage);
            }
        }
    }

    public class ErrorMessage
    {
        public ErrorMessage(string message, ErrorInfoType type)
        {
            Message = message;
            Type = type;
        }

        public string Message { get; }
        public ErrorInfoType Type { get; }
    }

    public enum ErrorInfoType
    {
        Core,
        Loading,
        Pipeline
    }
}
