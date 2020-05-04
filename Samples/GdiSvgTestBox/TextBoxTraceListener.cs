using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace GdiSvgTestBox
{
    public sealed class TextBoxTraceListener : TraceListener
    {
        private const string AppName = "GdiSvgTestBox.exe";

        private RichTextBox _textBox;
        private NotifyIcon _notifyIcon;

        public TextBoxTraceListener(RichTextBox textBox, Icon icon)
        {
            _textBox = textBox;

            _notifyIcon = new NotifyIcon();

            _notifyIcon.Visible         = true;
            _notifyIcon.Text            =  MainForm.AppTitle;
            _notifyIcon.BalloonTipIcon  = ToolTipIcon.Error;
            _notifyIcon.BalloonTipTitle = MainForm.AppTitle;
            _notifyIcon.BalloonTipText  = "An exception or error occurred.";

            if (icon != null)
            {
                _notifyIcon.Icon = icon;
            }
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

            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }

            _textBox = null;
            _notifyIcon = null;

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

            if (_textBox.TextLength >= _textBox.MaxLength)
            {
                _textBox.Clear();
            }

            if (message.StartsWith(AppName, StringComparison.OrdinalIgnoreCase))
            {
                message = message.Remove(0, AppName.Length + 1);
            }

            if (message.StartsWith("Error", StringComparison.OrdinalIgnoreCase))
            {
                _notifyIcon.ShowBalloonTip(3000);

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
