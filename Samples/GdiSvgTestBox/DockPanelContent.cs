using System;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace GdiSvgTestBox
{
    public partial class DockPanelContent : DockContent
    {
        /// <summary>
        /// Default font used for the generic 'serif' family
        /// </summary>
        public const string PanelDefaultFont = "Segoe UI";

        protected bool _isActive;

        protected ThemeBase _theme;
        protected MainForm _mainForm;
        protected OptionSettings _optionSettings;

        public event EventHandler PageSelected;
        public event EventHandler PageDeselected;

        public DockPanelContent()
        {
            InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.Dpi;
        }

        public void InitializePanel(MainForm mainForm, OptionSettings optionSettings, ThemeBase theme)
        {
            _mainForm       = mainForm;
            _optionSettings = optionSettings;
            _theme          = theme;
        }

        public virtual ThemeBase Theme
        {
            get {
                return _theme;
            }
            set {
                if (value != null)
                {
                    _theme = value;
                }
            }
        }

        public virtual void IdleUpdate()
        {
        }

        public virtual void OnPageSelected(EventArgs e)
        {
            this.PageSelected?.Invoke(this, e);
        }

        public virtual void OnPageDeselected(EventArgs e)
        {
            this.PageDeselected?.Invoke(this, e);
        }
    }
}
