using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace GdiW3cSvgTestSuite
{
    public sealed class TextBoxTraceListener : TraceListener
    {
        private RichTextBox _textBox;

        public TextBoxTraceListener(RichTextBox textBox)
        {
            _textBox = textBox;
        }

        public override void Write(string message)
        {
            SetText(message);
        }

        public override void WriteLine(string message)
        {
            if (message == null)
            {
                return;
            }
            SetText(message + Environment.NewLine);
        }

        protected override void Dispose(bool disposing)
        {
            if (_textBox != null)
            {
                if (_textBox.IsDisposed == false)
                {
                    _textBox.Dispose();
                }
            }

            _textBox = null;

            base.Dispose(disposing);
        }

        private void SetText(string message)
        {
            if (_textBox == null || message == null || _textBox.IsDisposed)
            {
                return;
            }

            if (_textBox.InvokeRequired)
            {
                MethodInvoker del = delegate {
                    SetText(message);
                };
                _textBox.Invoke(del);
                return;
            }

            if (message.StartsWith("Error", StringComparison.OrdinalIgnoreCase))
            {
                _textBox.SelectionStart = _textBox.TextLength;
                _textBox.SelectionLength = 0;

                _textBox.SelectionColor = Color.Red;
                _textBox.AppendText(message);
                _textBox.SelectionColor = _textBox.ForeColor;
            }
            else if (message.StartsWith("Warn", StringComparison.OrdinalIgnoreCase))
            {
                _textBox.SelectionStart = _textBox.TextLength;
                _textBox.SelectionLength = 0;

                _textBox.SelectionColor = Color.Green;
                _textBox.AppendText(message);
                _textBox.SelectionColor = _textBox.ForeColor;
            }
            else
            {
                _textBox.AppendText(message);
            }
            _textBox.Focus();
            _textBox.Select(_textBox.TextLength, 0);
            _textBox.SelectionStart = _textBox.TextLength;
            _textBox.ScrollToCaret();
        }
    }
}
