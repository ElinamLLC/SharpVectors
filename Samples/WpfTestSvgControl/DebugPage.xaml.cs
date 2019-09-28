using System;

using System.Windows.Controls;

namespace WpfTestSvgControl
{
    /// <summary>
    /// Interaction logic for DebugPage.xaml
    /// </summary>
    public partial class DebugPage : Page
    {
        private MainWindow _mainWindow;

        public DebugPage()
        {
            InitializeComponent();
        }

        public MainWindow MainWindow
        {
            get {
                return _mainWindow;
            }
            set {
                _mainWindow = value;
            }
        }

        public void Startup()
        {
            if (traceDocument != null)
            {
                traceDocument.Startup();
            }
        }

        public void Shutdown()
        {
            if (traceDocument != null)
            {
                traceDocument.Shutdown();
            }
        }

        public void PageSelected(bool isSelected)
        {
            if (isSelected)
            {
                debugBox.Focus();
            }
        }
    }
}
