using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;

namespace WpfTestSvgControl
{
    public partial class TraceDocument : FlowDocument, ITraceTextSink
    {
        private delegate void AppendTextDelegate(string msg, string style);

        private TraceListener _listener;

        public TraceDocument()
        {
            AutoAttach = false;
            InitializeComponent();
        }

        public bool AutoAttach { get; set; }

        public void Event(string msg, TraceEventType eventType)
        {
            Append(msg, eventType.ToString());
        }

        public void Fail(string msg)
        {
            Append(msg, "Fail");
        }

        public void Startup()
        {
            if (_listener == null)
            {
                _listener = new TraceTextSource(this);
                Trace.Listeners.Add(_listener);
            }
        }

        public void Shutdown()
        {
            if (_listener != null)
            {
                Trace.Listeners.Remove(_listener);
                _listener.Dispose();
                _listener = null;
            }
        }

        private void Append(string msg, string style)
        {
            if (Dispatcher.CheckAccess())
            {
                Debug.Assert(Blocks.LastBlock != null);
                Debug.Assert(Blocks.LastBlock is Paragraph);
                var run = new Run(msg);

                run.Style = (Style)(Resources[style]);
                if (run.Style == null)
                    run.Style = (Style)(Resources["Information"]);

                ((Paragraph)Blocks.LastBlock).Inlines.Add(run);

                ScrollParent(this);
            }
            else
            {
                Dispatcher.Invoke(new AppendTextDelegate(Append), msg, style);
            }
        }

        private static void ScrollParent(FrameworkContentElement element)
        {
            if (element != null)
            {
                if (element.Parent is TextBoxBase)
                    ((TextBoxBase)element.Parent).ScrollToEnd();

                else if (element.Parent is ScrollViewer)
                    ((ScrollViewer)element.Parent).ScrollToEnd();

                else
                    ScrollParent(element.Parent as FrameworkContentElement);
            }
        }

        private void Document_Loaded(object sender, RoutedEventArgs e)
        {
            if (AutoAttach && _listener == null)
            {
                _listener = new TraceTextSource(this);
                Trace.Listeners.Add(_listener);
            }
        }

        private void Document_Unloaded(object sender, RoutedEventArgs e)
        {
            if (AutoAttach && _listener != null)
            {
                Trace.Listeners.Remove(_listener);
                _listener.Dispose();
                _listener = null;
            }
        }
    }
}
