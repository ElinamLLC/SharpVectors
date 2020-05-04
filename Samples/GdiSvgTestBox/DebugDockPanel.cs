using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace GdiSvgTestBox
{
    public partial class DebugDockPanel : DockPanelContent
    {
        #region Private Fields

        private TextBoxTraceListener _textBoxListener;

        #endregion

        #region Constructors and Destructor

        public DebugDockPanel()
        {
            InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.DockAreas     = DockAreas.DockBottom | DockAreas.Float;
 
            this.CloseButton        = false;
            this.CloseButtonVisible = false;

            debugTextBox.BackColor = Color.White;
            debugTextBox.Font      = new Font(debugTextBox.Font.FontFamily, debugTextBox.Font.Size + 4, 
                FontStyle.Regular, debugTextBox.Font.Unit);

            _textBoxListener = new TextBoxTraceListener(debugTextBox, this.Icon);
            Trace.Listeners.Add(_textBoxListener);
        }

        #endregion

        #region Private Event Handlers

        private void OnFormLoad(object sender, EventArgs e)
        {

        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            if (_textBoxListener != null)
            {
                _textBoxListener.Dispose();
                _textBoxListener = null;
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            Trace.Listeners.Remove(_textBoxListener);
        }

        private void OnFormShown(object sender, EventArgs e)
        {
        }

        #endregion
    }
}
