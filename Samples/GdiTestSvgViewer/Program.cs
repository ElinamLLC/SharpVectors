using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Viewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] cmdLineArgs)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Trace.WriteLine("Starting application", "DEBUG");

            Form svgViewerForm;

            if (cmdLineArgs.Length > 0)
            {
                Trace.WriteLine("Getting initial file from command line: " + cmdLineArgs[0], "DEBUG");
                svgViewerForm = new SvgViewerForm(cmdLineArgs[0]);
            }
            else
            {
                svgViewerForm = new SvgViewerForm();
            }

            Application.Run(svgViewerForm);
        }

    }
}
