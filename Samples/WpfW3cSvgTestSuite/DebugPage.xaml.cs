using System;

using System.Windows.Controls;

namespace WpfW3cSvgTestSuite
{
    /// <summary>
    /// Interaction logic for DebugPage.xaml
    /// </summary>
    public partial class DebugPage : Page
    {
        public DebugPage()
        {
            InitializeComponent();
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

    }
}
