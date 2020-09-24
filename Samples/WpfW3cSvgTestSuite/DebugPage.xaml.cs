using System;
using System.Diagnostics;

using System.Windows;
using System.Windows.Controls;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Search;

namespace WpfW3cSvgTestSuite
{
    /// <summary>
    /// Interaction logic for DebugPage.xaml
    /// </summary>
    public partial class DebugPage : Page
    {
        private bool _isTraceStarted;
        private TextBoxTraceListener _listener;

        private SearchPanel _searchPanel;

        public DebugPage()
        {
            InitializeComponent();

            _isTraceStarted = false;

            this.Loaded += OnPageLoaded;

            Trace.UseGlobalLock = true;
        }

        public bool IsTraceStarted
        {
            get {
                return _isTraceStarted;
            }
        }

        public void Startup()
        {
            if (_listener == null)
            {
                _listener = new TextBoxTraceListener(textEditor);
                Trace.Listeners.Add(_listener);

                _isTraceStarted = true;
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

            _isTraceStarted = false;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            this.Startup();

            if (_searchPanel == null)
            {
                TextEditorOptions options = textEditor.Options;
                if (options != null)
                {
                    //options.AllowScrollBelowDocument = true;
                    options.EnableHyperlinks = true;
                    options.EnableEmailHyperlinks = true;
                    options.EnableVirtualSpace = false;
                    options.HighlightCurrentLine = true;
                    //options.ShowSpaces               = true;
                    //options.ShowTabs                 = true;
                    //options.ShowEndOfLine            = true;              
                }

                textEditor.IsReadOnly = true;
                textEditor.WordWrap = false;
                textEditor.ShowLineNumbers = true;

                _searchPanel = SearchPanel.Install(textEditor);
            }

            if (_listener == null)
            {
                _listener = new TextBoxTraceListener(textEditor);
                Trace.Listeners.Add(_listener);
            }
        }
    }
}
