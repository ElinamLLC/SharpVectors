using System;
using System.Windows;
using System.ComponentModel;

using SharpVectors.Renderers.Wpf;

namespace WpfXXETestBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string AppTitle      = "WpfXXETestBox";
        public const string AppErrorTitle = "WpfXXETestBox - Error";

        #region Private Fields

        private bool _isShown;

        private DebugPage _debugPage;
        private SettingsPage _settingsPage;
        private XXEInputPage _xxeInputPage;
        private XXEInjectionPage _xxeInjectionPage;

        private WpfDrawingSettings _wpfSettings;

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded  += OnWindowLoaded;
            this.Closing += OnWindowClosing;
        }

        #endregion

        #region Public Properties

        public WpfDrawingSettings ConversionSettings
        {
            get {
                return _wpfSettings;
            }
            set {
                if (value != null)
                {
                    _wpfSettings = value;

                    if (_xxeInputPage != null)
                    {
                        _xxeInputPage.ConversionSettings = value;
                    }
                    if (_xxeInjectionPage != null)
                    {
                        _xxeInjectionPage.ConversionSettings = value;
                    }
                }
            }
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

            if (_xxeInputPage != null)
            {
                _xxeInputPage.OnContentRendered(e);
            }
            if (_xxeInjectionPage != null)
            {
                _xxeInjectionPage.OnContentRendered(e);
            }
        }

        #endregion

        #region Private Methods

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Retrieve the display pages...
            _xxeInputPage     = frameXXEInput.Content as XXEInputPage;
            _xxeInjectionPage = frameXXEInjection.Content as XXEInjectionPage;
            _debugPage        = frameDebugging.Content as DebugPage;
            _settingsPage     = frameSettings.Content as SettingsPage;

            if (_settingsPage != null)
            {
                _settingsPage.Window = this;
            }

            if (_debugPage != null)
            {
                _debugPage.Startup();
            }

            tabXXEInput.IsSelected = true;
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (_xxeInputPage != null)
            {
                _xxeInputPage.OnContentCleared(e);
            }
            if (_xxeInjectionPage != null)
            {
                _xxeInjectionPage.OnContentCleared(e);
            }

            if (_debugPage != null)
            {
                _debugPage.Shutdown();
            }
        }

        #endregion
    }
}
