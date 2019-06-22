using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using SharpVectors.Renderers.Wpf;

using FolderBrowserDialog = ShellFileDialogs.FolderBrowserDialog;

namespace WpfW3cSvgTestSuite
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private bool _isInitialising;
        private bool _isGeneralModified;
        private bool _isConversionModified;

        private MainWindow _mainWindow;

        private OptionSettings _optionSettings;
        private WpfDrawingSettings _wpfSettings;

        public SettingsPage()
        {
            InitializeComponent();

            this.Loaded   += OnPageLoaded;
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

        private string GetPath(string inputPath)
        {
            if (string.IsNullOrWhiteSpace(inputPath))
            {
                return inputPath;
            }
            if (_optionSettings.HidePathsRoot)
            {
                Uri fullPath = new Uri(inputPath, UriKind.Absolute);

                // Make relative path to the SharpVectors folder...
                int indexOf = inputPath.IndexOf("SharpVectors", StringComparison.OrdinalIgnoreCase);
                if (indexOf > 0)
                {
                    Uri relRoot = new Uri(inputPath.Substring(0, indexOf), UriKind.Absolute);

                    string relPath = relRoot.MakeRelativeUri(fullPath).ToString();
                    relPath = relPath.Replace('/', '\\');

                    relPath = Uri.UnescapeDataString(relPath);
                    if (!relPath.StartsWith("..\\", StringComparison.OrdinalIgnoreCase))
                    {
                        relPath = "..\\" + relPath;
                    }

                    return relPath;
                }
            }
            return inputPath;
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            if (_isGeneralModified || _isConversionModified)
            {
                if (_mainWindow != null && _optionSettings != null)
                {
                    if (_isConversionModified)
                    {
                        _optionSettings.ConversionSettings = _wpfSettings;
                    }

                    _mainWindow.OptionSettings = _optionSettings;
                }
            }

        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (_mainWindow == null || _mainWindow.OptionSettings == null)
            {
                return;
            }
            _optionSettings = _mainWindow.OptionSettings;

            var wpfSettings = _optionSettings.ConversionSettings;
            if (wpfSettings != null)
            {
                _wpfSettings = wpfSettings.Clone();
            }
            if (_wpfSettings == null)
            {
                return;
            }

            _isInitialising = true;

            txtSvgSuitePath.Text    = this.GetPath(_optionSettings.LocalSuitePath);
            txtSvgSuitePathWeb.Text = _optionSettings.WebSuitePath;

            txtSvgSuitePath.IsReadOnly = _optionSettings.HidePathsRoot;

            chkHidePathsRoot.IsChecked         = _optionSettings.HidePathsRoot;

            chkTextAsGeometry.IsChecked        = _wpfSettings.TextAsGeometry;
            chkIncludeRuntime.IsChecked        = _wpfSettings.IncludeRuntime;

            chkIgnoreRootViewbox.IsChecked     = _wpfSettings.IgnoreRootViewbox;
            chkEnsureViewboxSize.IsChecked     = _wpfSettings.EnsureViewboxSize;
            chkEnsureViewboxPosition.IsChecked = _wpfSettings.EnsureViewboxPosition;

            _isConversionModified = false;

            _isInitialising = false;
        }

        private void OnSettingsChanged(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded)
            {
                return;
            }

            if (_mainWindow == null || _optionSettings == null || _wpfSettings == null)
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

            _isConversionModified = true;

            _isInitialising = false;
        }

        private void OnGeneralSettingsChanged(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded)
            {
                return;
            }

            if (_mainWindow == null || _optionSettings == null || _wpfSettings == null)
            {
                return;
            }
            if (_isInitialising)
            {
                return;
            }

            _isInitialising = true;

            _isGeneralModified = true;

            if (sender == chkHidePathsRoot)
            {
                _optionSettings.HidePathsRoot = chkHidePathsRoot.IsChecked.Value;

                if (chkHidePathsRoot.IsChecked != null && chkHidePathsRoot.IsChecked.Value)
                {
                    txtSvgSuitePath.IsReadOnly = true;
                }
                else
                {
                    txtSvgSuitePath.IsReadOnly = false;
                }

                txtSvgSuitePath.Text = this.GetPath(_optionSettings.LocalSuitePath);
            }

            _isInitialising = false;
        }

        private void OnBrowseForSvgSuitePath(object sender, RoutedEventArgs e)
        {
            string selectedDirectory = FolderBrowserDialog.ShowDialog(IntPtr.Zero, 
                "Select the location of the W3C SVG 1.1 Full Test Suite", null);
            if (selectedDirectory != null)
            {
                txtSvgSuitePath.Text = selectedDirectory;
            }
        }

        private void OnOpenSvgSuitePath(object sender, RoutedEventArgs e)
        {
            var filePath = txtSvgSuitePath.Text;
            if (string.IsNullOrWhiteSpace(filePath) || Directory.Exists(filePath) == false)
            {
                return;
            }

            System.Diagnostics.Process.Start("explorer.exe", @"/select, " + filePath);
        }

        private void OnSvgSuitePathTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.IsLoaded)
            {
                return;
            }

            string selectePath = txtSvgSuitePath.Text;
            if (selectePath != null)
            {
                selectePath = selectePath.Trim();
            }
            if (string.IsNullOrWhiteSpace(selectePath) || !Directory.Exists(selectePath))
            {
                btnPathLocate.IsEnabled = false;

                return;
            }

            btnPathLocate.IsEnabled = true;

            if (OptionSettings.IsTestSuiteAvailable(selectePath))
            {
                _isGeneralModified = true;
                _optionSettings.LocalSuitePath = selectePath;
            }
        }

    }
}
