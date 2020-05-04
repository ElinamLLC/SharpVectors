using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;

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
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        #region Private Fields

        private bool _isShown;

        private SvgPage _svgPage;
        private XamlPage _xamlPage;
        private DebugPage _debugPage;
        private SettingsPage _settingsPage;

        #endregion

        #region Constructors

        public MainPage()
        {
            InitializeComponent();

            this.Loaded += OnPageLoaded;
        }

        #endregion

        #region Public Methods

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        public void ContentRendered(object sender, EventArgs e)
        {
            if (_isShown)
            {
                return;
            }
            _isShown = true;

            // Retrieve the display pages...
            _svgPage      = frameSvgInput.Content as SvgPage;
            _xamlPage     = frameXamlOutput.Content as XamlPage;
            _debugPage    = frameDebugging.Content as DebugPage;
            _settingsPage = frameSettings.Content as SettingsPage;

            if (_svgPage != null && _xamlPage != null)
            {
                _svgPage.XamlPage = _xamlPage;
            }

            if (_svgPage != null && _settingsPage != null)
            {
                _settingsPage.SvgPage = _svgPage;
            }

            if (_debugPage != null)
            {
                if (!_debugPage.IsTraceStarted)
                {
                    _debugPage.Startup();
                }
            }

            if (_svgPage != null)
            {
                _svgPage.InitializeDocument();
            }
        }

        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (_debugPage != null)
            {
                if (!_debugPage.IsTraceStarted)
                {
                    _debugPage.Startup();
                }
            }
        }

        public void WindowClosing(object sender, CancelEventArgs e)
        {
            if (_debugPage != null)
            {
                _debugPage.Shutdown();
            }
        }

        #endregion

        #region Private Methods

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
        }

        #endregion

    }
}
