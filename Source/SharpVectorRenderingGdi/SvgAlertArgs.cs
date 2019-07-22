using System;

namespace SharpVectors.Renderers
{
    public sealed class SvgAlertArgs : EventArgs
    {
        public SvgAlertArgs(string message)
        {
            this.Message = message;
        }

        public string Message { get; }
        public bool Handled { get; set; }
    }
}
