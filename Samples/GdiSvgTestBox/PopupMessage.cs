using System;
using System.Drawing;
using System.Windows.Forms;

namespace GdiSvgTestBox
{
    public partial class PopupMessage : UserControl
    {
        private ToolStripDropDown _ownerControl;
        private Timer _timerClose;

        public PopupMessage()
        {
            InitializeComponent();

            this.Font = new Font(DockPanelContent.PanelDefaultFont, 16F, FontStyle.Regular, GraphicsUnit.World);

            // timerClose
            _timerClose = new Timer(this.components);
            _timerClose.Interval = 6000;
            _timerClose.Enabled  = false;
            _timerClose.Tick    += OnCloseTimer;
        }

        public string Message
        {
            get {
                return txtMessage.Text;
            }
            set {
                txtMessage.Text = value;
            }
        }

        public ToolStripDropDown OwnerControl
        {
            get {
                return _ownerControl;
            }
            set {
                _ownerControl = value;

                if (_ownerControl != null)
                {
                    _ownerControl.Closing += OnControlClosing;
                    _ownerControl.Opened  += OnControlOpened;
                }
            }
        }

        private void OnControlOpened(object sender, EventArgs e)
        {
            if (_timerClose.Enabled == false)
            {
                _timerClose.Enabled = true;
            }
        }

        private void OnControlClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            _timerClose.Enabled = false;
        }

        private void OnCloseTimer(object sender, EventArgs e)
        {
            _timerClose.Enabled = false;
            if (_ownerControl != null)
            {
                _ownerControl.Hide();
            }
        }
    }

    public sealed class PopupMessageContainer : ToolStripDropDown
    {
        private PopupMessage _popupMsg;

        public PopupMessageContainer()
        {
            this.Font = new Font(DockPanelContent.PanelDefaultFont, 16F, FontStyle.Regular, GraphicsUnit.World);

            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.UpdateStyles();

            this.BackColor = Color.FromArgb((int)(255 * SearchGlobals.FadeOpacity), 0, 122, 204);

            _popupMsg = new PopupMessage();
            _popupMsg.OwnerControl = this;
            var host = new ToolStripControlHost(_popupMsg)
            {
                Margin   = Padding.Empty,
                Padding  = Padding.Empty,
                AutoSize = false,
                Size     = _popupMsg.Size
            };

            this.Size = _popupMsg.Size;
            this.Items.Add(host);

            this.Margin            = Padding.Empty;
            this.Padding           = new Padding(1, 1, 1, 8);
            this.CanOverflow       = false;
            this.AutoClose         = true;
            this.DropShadowEnabled = false;
        }

        public string Message
        {
            get {
                if (_popupMsg != null)
                {
                    return _popupMsg.Message;
                }
                return string.Empty;
            }
            set {
                if (_popupMsg != null)
                {
                    _popupMsg.Message = value;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (var brush = new SolidBrush(this.BackColor))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
            var rect = this.ClientRectangle;
            rect.Inflate(1, 1);
            using (var pen = new Pen(this.BackColor, 1))
            {
                e.Graphics.DrawRectangle(pen, rect);
            }
        }
    }

}
