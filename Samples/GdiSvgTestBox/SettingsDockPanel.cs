using System;
using System.Drawing;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace GdiSvgTestBox
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

            panelGeneral.Font     = new Font(PanelDefaultFont, 14F, FontStyle.Regular, GraphicsUnit.World);

            chkHidePathsRoot.Font = new Font(chkHidePathsRoot.Font, FontStyle.Bold);
//            labelTheme.Font       = new Font(labelTheme.Font, FontStyle.Bold);
            labelThemeNote.Font   = new Font(PanelDefaultFont, 14F, FontStyle.Regular, GraphicsUnit.World);
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

        private void InitializeData()
        {
            if (_mainForm == null || _mainForm.OptionSettings == null)
            {
                return;
            }
            _optionSettings = _mainForm.OptionSettings;

            _isInitialising = true;

            cmbTheme.Items.Clear();
            for (int i = 0; i < 3; i++)
            {
                DockingTheme theme = (DockingTheme)i;
                cmbTheme.Items.Add(theme.ToString().Replace("Theme", string.Empty));
            }
            cmbTheme.SelectedIndex = (int)_optionSettings.Theme;

            _isConversionModified = false;

            _isInitialising = false;
        }

        #endregion
    }
}
