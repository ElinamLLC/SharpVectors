using System;
using System.Drawing;
using System.Windows.Forms;

using SharpVectors.Renderers.Forms;

namespace GdiSvgTestBox
{
    public partial class SizeModeForm : Form
    {
        public SizeModeForm()
        {
            InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font(DockPanelContent.PanelDefaultFont, 14F, FontStyle.Regular, GraphicsUnit.World);

            this.StartPosition = FormStartPosition.Manual;

            this.BackColor = Color.FromArgb(0, 122, 204);

            this.Opacity = SearchGlobals.FadeOpacity;
        }

        public SvgPictureBox TargetControl { get; set; }
        public Control TargetForm { get; set; }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var targetControl = this.TargetControl;
            var selectedIndex = cbbSizeMode.SelectedIndex;
            if (selectedIndex == -1 || targetControl == null)
            {
                return;
            }

            PictureBoxSizeMode selectedMode = (PictureBoxSizeMode)selectedIndex;
            if (selectedMode != targetControl.SizeMode)
            {
                targetControl.Focus();
                targetControl.SizeMode = selectedMode;
            }
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            cbbSizeMode.Items.Add("Normal");
            cbbSizeMode.Items.Add("Stretch Image");
            cbbSizeMode.Items.Add("Auto-Size");
            cbbSizeMode.Items.Add("Center Image");
            cbbSizeMode.Items.Add("Zoom");

            Control[] controls = {
                lblSize,
                cbbSizeMode,
                panelContainer
            };
            foreach (var control in controls)
            {
                control.MouseMove += OnMouseMove;
            }

            this.MouseMove += OnMouseMove;
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            var targetControl = this.TargetControl;
            var targetForm    = this.TargetForm;

            if (targetControl != null)
            {
                var posRect = targetControl.RectangleToScreen(targetControl.ClientRectangle);
                var winSize = this.Size;

                this.Left = posRect.Right - winSize.Width - SearchGlobals.Offset;
                this.Top = posRect.Top + SearchGlobals.Offset;

                cbbSizeMode.SelectedIndex = (int)targetControl.SizeMode;

                targetControl.Focus();
            }
            if (targetForm != null)
            {
                targetForm.LocationChanged += OnTargetWinMove;
                targetForm.Resize          += OnTargetWinResize;
            }

            timerMouse.Enabled = true;
        }

        private void OnTargetWinResize(object sender, EventArgs e)
        {
            this.Reposition();
        }

        private void OnTargetWinMove(object sender, EventArgs e)
        {
            this.Reposition();
        }

        private void Reposition()
        {
            var targetControl = this.TargetControl;

            if (targetControl == null)
            {
                return;
            }

            var posLoc = targetControl.PointToScreen(targetControl.Location);

            var posRect = targetControl.RectangleToScreen(targetControl.ClientRectangle);
            var winSize = this.Size;

            this.Left = posRect.Right - winSize.Width - SearchGlobals.Offset;
            this.Top = posRect.Top + SearchGlobals.Offset;
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {

        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == SearchGlobals.WM_NCHITTEST)
            {
                m.Result = (IntPtr)(SearchGlobals.HT_CAPTION);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Control control = sender as Control;
            if (control == null)
            {
                this.Opacity = 1.0;
                timerMouse.Enabled = true;
                return;
            }
            if (control.ClientRectangle.Contains(new Point(e.X, e.Y)))
            {
                this.Opacity = 1.0;
                timerMouse.Enabled = true;
                return;
            }
            
            var rect = this.RectangleToScreen(this.ClientRectangle);
            if (rect.Contains(e.Location))
            {
                this.Opacity = SearchGlobals.FadeOpacity;
            }
            else
            {
                this.Opacity = 1.0;
                timerMouse.Enabled = true;
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            var rect = this.RectangleToScreen(this.ClientRectangle);
            if (rect.Contains(MousePosition))
            {
                this.Opacity = 1.0;
            }
            else
            {
                this.Opacity = SearchGlobals.FadeOpacity;
                timerMouse.Enabled = false;
            }
        }
    }
}
