using System;
using System.Windows;
using System.ComponentModel;

namespace WpfSvgTestBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string AppTitle      = "WpfSvgTestBox";
        public const string AppErrorTitle = "WpfSvgTestBox - Error";

        #region Private Fields

        private bool _isShown;

        private MainPage _mainPage;

        //private SvgPage _svgPage;
        //private XamlPage _xamlPage;
        //private DebugPage _debugPage;
        //private SettingsPage _settingsPage;

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded  += OnWindowLoaded;
            this.Closing += OnWindowClosing;
        }

        #endregion

        #region Protected Methods

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_isShown)
            {
                return;
            }
            _isShown = true;

            if (_mainPage != null)
            {
                _mainPage.ContentRendered(this, e);
            }

            //if (_svgPage != null)
            //{
            //    _svgPage.InitializeDocument();
            //}
        }

        #endregion

        #region Private Methods

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Retrieve the display pages...
            _mainPage = mainPage.Content as MainPage;
            if (_mainPage != null)
            {
                _mainPage.WindowLoaded(sender, e);
            }

            //_svgPage      = frameSvgInput.Content as SvgPage;
            //_xamlPage     = frameXamlOutput.Content as XamlPage;
            //_debugPage    = frameDebugging.Content as DebugPage;
            //_settingsPage = frameSettings.Content as SettingsPage;

            //if (_svgPage != null && _xamlPage != null)
            //{
            //    _svgPage.XamlPage = _xamlPage;
            //}

            //if (_svgPage != null && _settingsPage != null)
            //{
            //    _settingsPage.SvgPage = _svgPage;
            //}

            //if (_debugPage != null)
            //{
            //    _debugPage.Startup();
            //}
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (_mainPage != null)
            {
                _mainPage.WindowClosing(sender, e);
            }
            //if (_debugPage != null)
            //{
            //    _debugPage.Startup();
            //}
        }

        #endregion
    }
}
