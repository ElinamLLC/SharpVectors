using System;
using System.Windows.Forms;

namespace TestSvgControl
{
    public partial class HitTestForm : Form
    {
        private const string FormTitle = "Hit Test";

        public HitTestForm()
        {
            InitializeComponent();

            richTextBox.ZoomFactor = 1.5f;
        }

        public void SetTexts(string title, string content)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                this.Text = FormTitle;
            }
            else
            {
                this.Text = FormTitle + ": " + title;
            }
            richTextBox.Text = string.IsNullOrWhiteSpace(content) ? string.Empty : content;
        }
    }
}
