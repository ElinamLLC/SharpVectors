using System;
using System.Windows.Forms;

namespace GdiSvgTestBox
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Idle += OnApplicationIdle;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void OnApplicationIdle(object sender, EventArgs e)
        {
            var openForms = Application.OpenForms;
            if (openForms != null && openForms.Count != 0)
            {
                foreach (var openForm in openForms)
                {
                    var mainForm = openForm as MainForm;
                    if (mainForm != null)
                    {
                        mainForm.IdleUpdate();
                        break;
                    }
                }
            }
        }
    }
}
