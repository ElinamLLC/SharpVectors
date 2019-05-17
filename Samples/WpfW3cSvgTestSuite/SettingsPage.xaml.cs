using System;
using System.Windows;
using System.Windows.Controls;

using SharpVectors.Renderers.Wpf;

namespace WpfW3cSvgTestSuite
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private bool _isInitialising;
        private bool _isModified;

        private MainWindow _mainWindow;

        private WpfDrawingSettings _wpfSettings;

        public SettingsPage()
        {
            InitializeComponent();

            this.Loaded += OnPageLoaded;
            this.Unloaded += OnPageUnloaded;
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

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            if (_isModified && _mainWindow != null && _wpfSettings != null)
            {
                _mainWindow.ConversionSettings = _wpfSettings;
            }
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (_mainWindow == null)
            {
                return;
            }
            var wpfSettings = _mainWindow.ConversionSettings;
            if (wpfSettings != null)
            {
                _wpfSettings = wpfSettings.Clone();
            }
            if (_wpfSettings == null)
            {
                return;
            }

            _isInitialising = true;

            chkTextAsGeometry.IsChecked        = _wpfSettings.TextAsGeometry;
            chkIncludeRuntime.IsChecked        = _wpfSettings.IncludeRuntime;

            chkIgnoreRootViewbox.IsChecked     = _wpfSettings.IgnoreRootViewbox;
            chkEnsureViewboxSize.IsChecked     = _wpfSettings.EnsureViewboxSize;
            chkEnsureViewboxPosition.IsChecked = _wpfSettings.EnsureViewboxPosition;

            _isModified = false;

            _isInitialising = false;
        }

        private void OnSettingsChanged(object sender, RoutedEventArgs e)
        {
            if (_mainWindow == null || _wpfSettings == null)
            {
                return;
            }
            if (_isInitialising)
            {
                return;
            }

            _isInitialising = true;

            _wpfSettings.TextAsGeometry        = chkTextAsGeometry.IsChecked.Value;
            _wpfSettings.IncludeRuntime        = chkIncludeRuntime.IsChecked.Value;

            _wpfSettings.IgnoreRootViewbox     = chkIgnoreRootViewbox.IsChecked.Value;
            _wpfSettings.EnsureViewboxSize     = chkEnsureViewboxSize.IsChecked.Value;
            _wpfSettings.EnsureViewboxPosition = chkEnsureViewboxPosition.IsChecked.Value;

            _isModified = true;

            _isInitialising = false;
        }

    }
}
