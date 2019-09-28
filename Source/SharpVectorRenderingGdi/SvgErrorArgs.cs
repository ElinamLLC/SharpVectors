using System;

namespace SharpVectors.Renderers
{
    public sealed class SvgErrorArgs : EventArgs
    {
        public SvgErrorArgs(string message)
        {
            this.Message = message;
        }
        
        public SvgErrorArgs(string message, Exception exception)
        {
            this.Message   = message;
            this.Exception = exception;
        }

        public bool Handled { get; set; }

        public string Message { get; private set; }

        public Exception Exception { get; private set; }
    }
}
