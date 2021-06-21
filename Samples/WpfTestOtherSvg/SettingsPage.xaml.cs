using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using SharpVectors.Renderers.Wpf;

using FolderBrowserDialog = ShellFileDialogs.FolderBrowserDialog;

namespace WpfTestOtherSvg
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
                txtPngDirectory.Focus();
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

            txtPngDirectory.Text = _optionSettings.GetPath(_optionSettings.PngDirectory);
            txtSvgDirectory.Text = _optionSettings.GetPath(_optionSettings.SvgDirectory);

            txtPngDirectory.IsReadOnly = _optionSettings.HidePathsRoot;

            txtFontDirectory.Text = _optionSettings.FontDirectory;
            txtImageDirectory.Text = _optionSettings.ImageDirectory;

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
                    txtPngDirectory.IsReadOnly = true;
                }
                else
                {
                    txtPngDirectory.IsReadOnly = false;
                }

                txtPngDirectory.Text = _optionSettings.GetPath(_optionSettings.PngDirectory);
                txtSvgDirectory.Text = _optionSettings.GetPath(_optionSettings.SvgDirectory);
            }

            _optionSettings.RecursiveSearch = chkRecursiveSearch.IsChecked.Value;
            _optionSettings.ShowInputFile   = chkShowInputFile.IsChecked.Value;
            _optionSettings.ShowOutputFile  = chkShowOutputFile.IsChecked.Value;

            _isInitialising = false;
        }

        private void OnOpenSvgDirectory(object sender, RoutedEventArgs e)
        {
            var filePath = txtSvgDirectory.Text;
            if (string.IsNullOrWhiteSpace(filePath) || Directory.Exists(filePath) == false)
            {
                return;
            }

            OptionSettings.OpenFolderAndSelectItem(filePath, null);
        }

        private void OnBrowseSvgDirectory(object sender, RoutedEventArgs e)
        {
            string selectedDirectory = FolderBrowserDialog.ShowDialog(IntPtr.Zero, 
                "Select the location of the Test SVG Files", null);
            if (selectedDirectory != null)
            {
                txtSvgDirectory.Text = selectedDirectory;
            }
        }

        private void OnBrowsePngDirectory(object sender, RoutedEventArgs e)
        {
            string selectedDirectory = FolderBrowserDialog.ShowDialog(IntPtr.Zero, 
                "Select the location of the Test Expected Images", null);
            if (selectedDirectory != null)
            {
                txtPngDirectory.Text = selectedDirectory;
            }
        }

        private void OnOpenPngDirectory(object sender, RoutedEventArgs e)
        {
            var filePath = txtPngDirectory.Text;
            if (string.IsNullOrWhiteSpace(filePath) || Directory.Exists(filePath) == false)
            {
                return;
            }

            OptionSettings.OpenFolderAndSelectItem(filePath, null);
        }

        private void OnBrowseFontDirectory(object sender, RoutedEventArgs e)
        {
            string selectedDirectory = FolderBrowserDialog.ShowDialog(IntPtr.Zero,
                "Select the location of the Test Fonts", null);
            if (selectedDirectory != null)
            {
                txtFontDirectory.Text = selectedDirectory;
            }
        }

        private void OnOpenFontDirectory(object sender, RoutedEventArgs e)
        {
            var filePath = txtFontDirectory.Text;
            if (string.IsNullOrWhiteSpace(filePath) || Directory.Exists(filePath) == false)
            {
                return;
            }

            OptionSettings.OpenFolderAndSelectItem(filePath, null);
        }

        private void OnBrowseImageDirectory(object sender, RoutedEventArgs e)
        {
            string selectedDirectory = FolderBrowserDialog.ShowDialog(IntPtr.Zero,
                "Select the location of the Test Images", null);
            if (selectedDirectory != null)
            {
                txtImageDirectory.Text = selectedDirectory;
            }
        }

        private void OnOpenImageDirectory(object sender, RoutedEventArgs e)
        {
            var filePath = txtImageDirectory.Text;
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

            string selectePath = txtSvgDirectory.Text;
            if (selectePath != null)
            {
                selectePath = selectePath.Trim();
            }
            if (string.IsNullOrWhiteSpace(selectePath) || !Directory.Exists(selectePath))
            {
                btnPngOpen.IsEnabled = false;

                return;
            }

            btnPngOpen.IsEnabled = true;

            _isGeneralModified = true;
            _optionSettings.SvgDirectory = selectePath;
        }

        private void OnPngPathTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.IsLoaded)
            {
                return;
            }

            string selectePath = txtPngDirectory.Text;
            if (selectePath != null)
            {
                selectePath = selectePath.Trim();
            }
            if (string.IsNullOrWhiteSpace(selectePath) || !Directory.Exists(selectePath))
            {
                btnPngOpen.IsEnabled = false;

                return;
            }

            btnPngOpen.IsEnabled = true;

            _isGeneralModified = true;
            _optionSettings.PngDirectory = selectePath;
        }

        #endregion
    }
}
