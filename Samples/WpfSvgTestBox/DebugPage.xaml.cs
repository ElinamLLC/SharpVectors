using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfSvgTestBox
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
