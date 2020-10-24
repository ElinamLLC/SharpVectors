using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using FolderBrowserDialog = ShellFileDialogs.FolderBrowserDialog;

namespace GdiW3cSvgTestSuite
{
    public partial class SettingsDockPanel : DockPanelContent, ITestPagePanel
    {
        #region Private Fields

        private bool _isInitialising;
        private bool _wasDeselected;
        private bool _isGeneralModified;
        private bool _isConversionModified;

        #endregion

        #region Constructors and Destructor

        public SettingsDockPanel()
        {
            InitializeComponent();

            this.DockAreas     = DockAreas.Document | DockAreas.Float;
            this.AutoScaleMode = AutoScaleMode.Dpi;            

            this.CloseButton   = false;

            groupBoxGeneral.Font    = new Font(PanelDefaultFont, 18F, FontStyle.Bold, GraphicsUnit.World);
            groupBoxConversion.Font = new Font(PanelDefaultFont, 18F, FontStyle.Bold, GraphicsUnit.World);

            panelGeneral.Font       = new Font(PanelDefaultFont, 14F, FontStyle.Regular, GraphicsUnit.World);

            chkHidePathsRoot.Font   = new Font(chkHidePathsRoot.Font, FontStyle.Bold);
            labelWeb.Font           = new Font(labelWeb.Font, FontStyle.Bold);
            labelLocal.Font         = new Font(labelLocal.Font, FontStyle.Bold);
            lblTestSuitesTitle.Font = new Font(lblTestSuitesTitle.Font, FontStyle.Bold);
        }

        #endregion

        #region ITestPagePanel Members

        public bool LoadDocument(string documentFilePath, SvgTestInfo testInfo, object extraInfo)
        {
            return true;
        }

        public void UnloadDocument()
        {
        }

        #endregion

        #region Public Methods

        public override void OnPageDeselected(EventArgs e)
        {
            base.OnPageDeselected(e);

            if (_isGeneralModified || _isConversionModified)
            {
                if (_mainForm != null && _optionSettings != null)
                {
                    _mainForm.OptionSettings = _optionSettings;
                }
            }

            _wasDeselected = true;
        }

        public override void OnPageSelected(EventArgs e)
        {
            base.OnPageSelected(e);

            if (_wasDeselected)
            {
                InitializeData();
            }

            _wasDeselected = false;
        }

        #endregion

        #region Private Event Handlers

        private void OnFormLoad(object sender, EventArgs e)
        {
            InitializeData();
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void OnFormShown(object sender, EventArgs e)
        {
        }

        private void OnGeneralSettingsChanged(object sender, EventArgs e)
        {
            if (_mainForm == null || _optionSettings == null)
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
                _optionSettings.HidePathsRoot = chkHidePathsRoot.Checked;

                if (chkHidePathsRoot.Checked)
                {
                    txtSvgSuitePath.ReadOnly = true;
                }
                else
                {
                    txtSvgSuitePath.ReadOnly = false;
                }

                txtSvgSuitePath.Text = _optionSettings.GetPath(_optionSettings.LocalSuitePath);
            }

            _isInitialising = false;
        }

        private void OnSelectedThemeChanged(object sender, EventArgs e)
        {
            if (_mainForm == null || _optionSettings == null)
            {
                return;
            }
            if (_isInitialising)
            {
                return;
            }

            _isInitialising = true;

            _isGeneralModified = true;

            int selectedIndex = cmbTheme.SelectedIndex;
            if (selectedIndex != -1)
            {
                _optionSettings.Theme = (DockingTheme)selectedIndex;
            }
            //TODO: Dynamic theme will require saving the windows, closing all and recreating...
            //if (_mainForm != null)
            //{
            //    _mainForm.ApplyTheme(_optionSettings.Theme);
            //}

            _isInitialising = false;
        }

        private void OnBrowseForSvgSuitePath(object sender, EventArgs e)
        {
            string selectedDirectory = FolderBrowserDialog.ShowDialog(IntPtr.Zero,
                "Select the location of the W3C SVG 1.1 Full Test Suite", null);
            if (selectedDirectory != null)
            {
                txtSvgSuitePath.Text = selectedDirectory;
            }
        }

        private void OnOpenSvgSuitePath(object sender, EventArgs e)
        {
            var filePath = txtSvgSuitePath.Text;
            if (string.IsNullOrWhiteSpace(filePath) || Directory.Exists(filePath) == false)
            {
                return;
            }

            OptionSettings.OpenFolderAndSelectItem(filePath, null);
        }

        private void OnSvgSuitePathTextChanged(object sender, EventArgs e)
        {
            string selectePath = txtSvgSuitePath.Text;
            if (selectePath != null)
            {
                selectePath = selectePath.Trim();
            }
            if (string.IsNullOrWhiteSpace(selectePath) || !Directory.Exists(selectePath))
            {
                btnPathLocate.Enabled = false;

                return;
            }

            btnPathLocate.Enabled = true;

            if (OptionSettings.IsTestSuiteAvailable(selectePath))
            {
                _isGeneralModified = true;
                _optionSettings.LocalSuitePath = selectePath;
            }
        }

        private void OnTestSuitesSelectionChanged(object sender, EventArgs e)
        {
            if (_mainForm == null || _isInitialising || _optionSettings == null)
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
                dlg.Owner = _mainForm;
                dlg.OptionSettings = _optionSettings;

                var dialogResult = dlg.ShowDialog();

                if (dialogResult == DialogResult.Cancel)
                {
                    return;
                }
            }

            _isInitialising = true;

            txtSvgSuitePath.Text    = _optionSettings.GetPath(testSuite.LocalSuitePath);
            txtSvgSuitePathWeb.Text = testSuite.WebSuitePath;

            _optionSettings.LocalSuitePath = testSuite.LocalSuitePath;
            _optionSettings.WebSuitePath = testSuite.WebSuitePath;

            testSuite.SetSelected(_optionSettings.TestSuites);

            _isGeneralModified = true;

            _isInitialising = false;
        }

        private void InitializeData()
        {
            if (_mainForm == null || _mainForm.OptionSettings == null)
            {
                return;
            }
            _optionSettings = _mainForm.OptionSettings;

            _isInitialising = true;

            txtSvgSuitePath.Text = _optionSettings.GetPath(_optionSettings.LocalSuitePath);
            txtSvgSuitePathWeb.Text = _optionSettings.WebSuitePath;

            txtSvgSuitePath.ReadOnly = _optionSettings.HidePathsRoot;
            chkHidePathsRoot.Checked = _optionSettings.HidePathsRoot;

            var testSuites = _optionSettings.TestSuites;
            if (testSuites != null && testSuites.Count != 0)
            {
                cboTestSuites.Items.Clear();

                int selectedIndex = 0;

                for (int i = 0; i < testSuites.Count; i++)
                {
                    var testSuite = testSuites[i];
                    ComboBoxItem comboxItem = new ComboBoxItem();
                    comboxItem.Value = testSuite.Description;
                    comboxItem.Tag   = testSuite;

                    cboTestSuites.Items.Add(comboxItem);

                    if (testSuite.IsSelected)
                    {
                        selectedIndex = i;
                    }
                }

                cboTestSuites.SelectedIndex = selectedIndex;
            }

            _isConversionModified = false;

            cmbTheme.Items.Clear();
            for (int i = 0; i < 3; i++)
            {
                DockingTheme theme = (DockingTheme)i;
                cmbTheme.Items.Add(theme.ToString().Replace("Theme", string.Empty));
            }
            cmbTheme.SelectedIndex = (int)_optionSettings.Theme;

            _isInitialising = false;
        }

        #endregion
    }
}
