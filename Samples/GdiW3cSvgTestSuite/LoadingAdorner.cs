using System;
using System.Drawing;
using System.Windows.Forms;

namespace GdiW3cSvgTestSuite
{
    public partial class LoadingAdorner : Form
    {
        public LoadingAdorner()
        {
            InitializeComponent();

            progressIndicator.BackColor = Color.LightGray;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            progressIndicator.Start();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            progressIndicator.Stop();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var sb = new SolidBrush(Color.FromArgb(128, Color.DarkGray));
            e.Graphics.FillRectangle(sb, this.DisplayRectangle);
        }
    }
}
