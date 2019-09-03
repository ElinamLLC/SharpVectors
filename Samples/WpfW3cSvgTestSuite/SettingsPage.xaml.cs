using System;
using System.IO;
using System.Collections.Generic;

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

            txtSvgSuitePath.Text       = _optionSettings.GetPath(_optionSettings.LocalSuitePath);
            txtSvgSuitePathWeb.Text    = _optionSettings.WebSuitePath;

            txtSvgSuitePath.IsReadOnly = _optionSettings.HidePathsRoot;

            chkHidePathsRoot.IsChecked         = _optionSettings.HidePathsRoot;

            chkTextAsGeometry.IsChecked        = _wpfSettings.TextAsGeometry;
            chkIncludeRuntime.IsChecked        = _wpfSettings.IncludeRuntime;

            chkIgnoreRootViewbox.IsChecked     = _wpfSettings.IgnoreRootViewbox;
            chkEnsureViewboxSize.IsChecked     = _wpfSettings.EnsureViewboxSize;
            chkEnsureViewboxPosition.IsChecked = _wpfSettings.EnsureViewboxPosition;

            var testSuites = _optionSettings.TestSuites;
            if (testSuites != null && testSuites.Count != 0)
            {
                cboTestSuites.Items.Clear();

                int selectedIndex = 0;

                for (int i = 0; i < testSuites.Count; i++)
                {
                    var testSuite = testSuites[i];
                    ComboBoxItem comboxItem = new ComboBoxItem();
                    comboxItem.Content = testSuite.Description;
                    comboxItem.Tag = testSuite;

                    cboTestSuites.Items.Add(comboxItem);

                    if (testSuite.IsSelected)
                    {
                        selectedIndex = i;
                    }
                }

                cboTestSuites.SelectedIndex = selectedIndex;
            }

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

                txtSvgSuitePath.Text = _optionSettings.GetPath(_optionSettings.LocalSuitePath);
            }

            _isInitialising = false;
        }

        private void OnTestSuitesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded || _isInitialising || _optionSettings == null)
            {
                return;
            }

            var selectedItem = cboTestSuites.SelectedItem as ComboBoxItem;
            if (selectedItem == null || selectedItem.Tag == null)
            {
                return;
            }
            var testSuite = selectedItem.Tag as SvgTestSuite;
            if (testSuite == null)
            {
                return;
            }
            string localSuitePath = testSuite.LocalSuitePath;
            if (OptionSettings.IsTestSuiteAvailable(localSuitePath) == false)
            {
                PromptDialog dlg = new PromptDialog();
                dlg.Owner = _mainWindow;
                dlg.OptionSettings = _optionSettings;

                var dialogResult = dlg.ShowDialog();

                if (dialogResult == null || dialogResult.Value == false)
                {
                    return;
                }
            }

            _isInitialising = true;

            txtSvgSuitePath.Text    = _optionSettings.GetPath(testSuite.LocalSuitePath);
            txtSvgSuitePathWeb.Text = testSuite.WebSuitePath;

            _optionSettings.LocalSuitePath = testSuite.LocalSuitePath;
            _optionSettings.WebSuitePath   = testSuite.WebSuitePath;

            testSuite.SetSelected(_optionSettings.TestSuites);

            _isGeneralModified = true;

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

            OptionSettings.OpenFolderAndSelectItem(filePath, null);
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

        #endregion
    }
}
