using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using SharpVectors.Renderers.Wpf;

using FolderBrowserDialog = ShellFileDialogs.FolderBrowserDialog;

namespace WpfTestSvgControl
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        #region Private Fields

        private bool _isInitialising;
        private bool _isGeneralModified;
        private bool _isConversionModified;

        private MainWindow _mainWindow;

        private OptionSettings _optionSettings;
        private WpfDrawingSettings _wpfSettings;

        #endregion

        #region Constructors and Destructor

        public SettingsPage()
        {
            InitializeComponent();

            this.Loaded   += OnPageLoaded;
            this.Unloaded += OnPageUnloaded;
        }

        #endregion

        #region Public Properties

        public MainWindow MainWindow
        {
            get {
                return _mainWindow;
            }
            set {
                _mainWindow = value;
            }
        }

        #endregion

        #region Public Properties

        public void PageSelected(bool isSelected)
        {
            if (isSelected)
            {
                txtSvgPath.Focus();
            }
        }

        #endregion

        #region Private Methods

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

            txtSvgPath.Text       = _optionSettings.GetPath(_optionSettings.CurrentSvgPath);
            txtSvgPathDefault.Text    = _optionSettings.GetPath(_optionSettings.DefaultSvgPath);

            txtSvgPath.IsReadOnly = _optionSettings.HidePathsRoot;

            chkHidePathsRoot.IsChecked         = _optionSettings.HidePathsRoot;
            chkRecursiveSearch.IsChecked       = _optionSettings.RecursiveSearch;
            chkShowInputFile.IsChecked         = _optionSettings.ShowInputFile;
            chkShowOutputFile.IsChecked        = _optionSettings.ShowOutputFile;

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
                    txtSvgPath.IsReadOnly = true;
                }
                else
                {
                    txtSvgPath.IsReadOnly = false;
                }

                txtSvgPath.Text    = _optionSettings.GetPath(_optionSettings.CurrentSvgPath);
                txtSvgPathDefault.Text = _optionSettings.GetPath(_optionSettings.DefaultSvgPath);
            }

            _optionSettings.RecursiveSearch = chkRecursiveSearch.IsChecked.Value;
            _optionSettings.ShowInputFile   = chkShowInputFile.IsChecked.Value;
            _optionSettings.ShowOutputFile  = chkShowOutputFile.IsChecked.Value;

            _isInitialising = false;
        }

        private void OnOpenForSvgPathDefault(object sender, RoutedEventArgs e)
        {
            var filePath = txtSvgPathDefault.Text;
            if (string.IsNullOrWhiteSpace(filePath) || Directory.Exists(filePath) == false)
            {
                return;
            }

            OptionSettings.OpenFolderAndSelectItem(filePath, null);
        }

        private void OnBrowseForSvgPath(object sender, RoutedEventArgs e)
        {
            string selectedDirectory = FolderBrowserDialog.ShowDialog(IntPtr.Zero, 
                "Select the location of the W3C SVG 1.1 Full Test ", null);
            if (selectedDirectory != null)
            {
                txtSvgPath.Text = selectedDirectory;
            }
        }

        private void OnOpenSvgPath(object sender, RoutedEventArgs e)
        {
            var filePath = txtSvgPath.Text;
            if (string.IsNullOrWhiteSpace(filePath) || Directory.Exists(filePath) == false)
            {
                return;
            }

            OptionSettings.OpenFolderAndSelectItem(filePath, null);
        }

        private void OnSvgPathTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.IsLoaded)
            {
                return;
            }

            string selectePath = txtSvgPath.Text;
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

            _isGeneralModified = true;
            _optionSettings.CurrentSvgPath = selectePath;
        }

        #endregion
    }
}
