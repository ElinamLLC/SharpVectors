using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using SharpVectors.Renderers.Gdi;

using WeifenLuo.WinFormsUI.Docking;

namespace GdiW3cSvgTestSuite
{
    public partial class TestDockPanel : DockPanelContent, ITestPagePanel
    {
        #region Private Fields

        private const string AppTitle        = "Svg Test Suite";
        private const string AppErrorTitle   = "Svg Test Suite - Error";

        #endregion

        #region Constructors and Destructor

        public TestDockPanel()
        {
            InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.DockAreas     = DockAreas.Document | DockAreas.Float;

            this.CloseButton   = false;

            labelConverted.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.World, 0);
            labelExpected.Font  = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.World, 0);
        }

        #endregion

        #region ITestPagePanel Members

        public bool LoadDocument(string documentFilePath, SvgTestInfo testInfo, object extraInfo)
        {
            this.UnloadDocument();

            if (extraInfo == null)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(documentFilePath) || File.Exists(documentFilePath) == false)
            {
                return false;
            }

            var pngFilePath = extraInfo.ToString();
            if (string.IsNullOrWhiteSpace(pngFilePath) || File.Exists(pngFilePath) == false)
            {
                return false;
            }

            Image pngImage = null;
            try
            {
                pngImage = Image.FromFile(pngFilePath);
                if (pngImage == null)
                {
                    return false;
                }
                viewerExpected.Image  = pngImage;
                viewerExpected.Width  = pngImage.Width + 10;
                viewerExpected.Height = pngImage.Height + 10;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());

                return false;
            }

            try
            {
                using (var svgRenderer = new GdiGraphicsRenderer(pngImage.Width, pngImage.Height))
                {
                    var svgWindow = svgRenderer.Window;

                    svgWindow.Source = documentFilePath;
                    svgRenderer.Render(svgWindow.Document);

                    var svgImage = svgRenderer.RasterImage;

                    viewerConverted.Image  = svgImage;
                    viewerConverted.Width  = svgImage.Width + 10;
                    viewerConverted.Height = svgImage.Height + 10;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());

                return false;
            }

            return true;
        }

        public void UnloadDocument()
        {
            viewerConverted.Image = null;
            viewerExpected.Image  = null;

            viewerConverted.Refresh();
            viewerExpected.Refresh();
        }

        #endregion
    }
}
