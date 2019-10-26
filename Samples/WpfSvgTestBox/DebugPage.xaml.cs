using System;
using System.Diagnostics;

using System.Windows.Controls;

namespace WpfSvgTestBox
{
    /// <summary>
    /// Interaction logic for DebugPage.xaml
    /// </summary>
    public partial class DebugPage : Page, ITraceTextSink
    {
        private delegate void AppendTextDelegate(string msg, string style);

        private TraceListener _listener;

        public DebugPage()
        {
            InitializeComponent();

            textEditor.IsReadOnly = true;

            Trace.UseGlobalLock = true;

            _listener = new TraceTextSource(this);
            Trace.Listeners.Add(_listener);
        }

        public void Startup()
        {
            if (_listener == null)
            {
                _listener = new TraceTextSource(this);
                Trace.Listeners.Add(_listener);
            }

            Trace.WriteLine("Startup");
        }

        public void Shutdown()
        {
            Trace.WriteLine("Shutdown");
            if (_listener != null)
            {
                Trace.Listeners.Remove(_listener);
                _listener.Dispose();
                _listener = null;
            }
        }

        public void Event(string msg, TraceEventType eventType)
        {
            Append(msg, eventType.ToString());
        }

        public void Fail(string msg)
        {
            Append(msg, "Fail");
        }

        private void Append(string msg, string style)
        {
            if (Dispatcher.CheckAccess())
            {
                if (!this.IsLoaded)
                {
                    return;
                }
                if (string.IsNullOrWhiteSpace(style))
                {
                    textEditor.AppendText(msg + Environment.NewLine);
                }
                else
                {
                    textEditor.AppendText(style + ": " + msg + Environment.NewLine);
                }
            }
            else
            {
                Dispatcher.Invoke(new AppendTextDelegate(Append), msg, style);
            }
        }
    }
}
