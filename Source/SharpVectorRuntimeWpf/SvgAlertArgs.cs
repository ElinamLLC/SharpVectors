using System;

namespace SharpVectors.Runtime
{
    public sealed class SvgAlertArgs : EventArgs
    {
        private bool _isHandled;
        private string _message;

        public SvgAlertArgs(string message)
        {
            _message = message;
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
    }
}
