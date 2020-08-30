using System;

namespace SharpVectors.Runtime
{
    public sealed class SvgErrorArgs : EventArgs
    {
        private bool _isHandled;
        private string _message;
        private Exception _exception;

        public SvgErrorArgs(string message)
            : this(message, null)
        {
        }
        
        public SvgErrorArgs(Exception exception)
            : this(null, exception)
        {
        }
        
        public SvgErrorArgs(string message, Exception exception)
        {
            _message   = message;
            _exception = exception;

            if (string.IsNullOrWhiteSpace(message) && exception != null)
            {
                _message = exception.Message;
            }
        }

        public bool IsException
        {
            get {
                return (_exception != null);
            }
        }

        public bool Handled 
        { 
            get {
                return _isHandled;
            }
            set {
                _isHandled = value;
            }
        }

        public string Message 
        { 
            get {
                return _message;
            }
            private set {
                _message = value;
            } 
        }

        public Exception Exception 
        { 
            get {
                return _exception;
            }
            private set {
                _exception = value;
            } 
        }
    }
}
