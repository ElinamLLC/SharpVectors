using System;
using System.IO;
using System.Windows.Forms;

namespace TestSvgControl
{
    public partial class QuickHelpForm : Form
    {
        public QuickHelpForm()
        {
            InitializeComponent();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            string quickHelpFile = Path.GetFullPath(Path.Combine("..\\", "QuickHelp.rtf"));
            if (!string.IsNullOrWhiteSpace(quickHelpFile) && File.Exists(quickHelpFile))
            {
                richTextBox.LoadFile(quickHelpFile);
            }
        }
    }
}
