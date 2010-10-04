using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using SharpVectors.Dom.Svg;
using SharpVectors.Renderers.Gdi;
using SharpVectors.Net;
using SharpVectors.Dom.Events;

using SharpVectors.Renderers.Forms;

namespace Viewer
{
    /// <summary>
    /// The main form of the Svg Viewer application.
    /// </summary>
    public partial class SvgViewerForm : System.Windows.Forms.Form
    {
        private string title = ".NET SVG Viewer";

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="SvgViewerForm">SvgViewerForm</see> class.
        /// </summary>
        /// <param name="filePath">
        /// The URL the <see cref="SvgViewerForm">SvgViewerForm</see> object
        /// will open.
        /// </param>
        public SvgViewerForm(string filePath)
            : this()
        {
            this.svgPictureBox.SourceURL = filePath;
        }

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="SvgViewerForm">SvgViewerForm</see> class.
        /// </summary>
        public SvgViewerForm()
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            // set up the cache manager so that files are cached. Using the
            // default location (/cache)
            ExtendedHttpWebRequest.CacheManager = new SvgCacheManager();

            //// need the following three lines to get outside the firewall.
            //WebProxy proxyObject = WebProxy.GetDefaultProxy();
            //proxyObject.Credentials = CredentialCache.DefaultCredentials;

            //// Now actually take over the global proxy settings with our new
            //// settings, all new requests use this proxy info
            //GlobalProxySelection.Select = proxyObject;
        }

        #endregion

        #region Entry Point

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(
            string[] cmdLineArgs)
        {
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

        #endregion

        #region Event handlers

        private void btnGo_Click(object sender, EventArgs e)
        {
            string url = null;

            if (svgUrlCombo.SelectedItem is FileInfo)
            {
                FileInfo f = (FileInfo)svgUrlCombo.SelectedItem;
                url = f.FullName;
            }
            else
            {
                url = svgUrlCombo.Text;
            }

            if (url != null && url.Length > 0)
            {
                loadUrl(url);
            }
            else
            {
                this.Text = this.title;
                MessageBox.Show("File not found");
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            svgUrlCombo.Text = openFileDialog1.FileName;
            btnGo.PerformClick();
        }

        private void miOpenLocal_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog(this);
        }

        /// <summary>
        /// The following code will load the SVGDOM, render to a emf, and
        /// place the EMF on the clipboard. Kt seems more complicated than it should
        /// see http://www.dotnet247.com/247reference/msgs/23/118514.aspx
        /// for why the straight forward solution does not work.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miCopy_Click(object sender, EventArgs e)
        {
            if (svgUrlCombo.Text.Length > 0)
            {
                int width = 500;
                int height = 500;

                // make SVG document with associated window so we can render it.
                SvgWindow window = new SvgPictureBoxWindow(width, height, null);
                GdiGraphicsRenderer renderer = new GdiGraphicsRenderer();
                renderer.Window = window;
                window.Renderer = renderer;
                window.Source = svgUrlCombo.Text;

                // copy and paste code taken from
                // http://www.dotnet247.com/247reference/msgs/23/117611.aspx
                // putting a plain MetaFile on the clipboard does not work.
                // .Net's metafile format is not recognised by most programs.
                Graphics g = CreateGraphics();
                IntPtr hdc = g.GetHdc();
                Metafile m = new Metafile(hdc, EmfType.EmfOnly);
                g.ReleaseHdc(hdc);
                g.Dispose();
                g = Graphics.FromImage(m);

                // draw the SVG here
                // NOTE: the graphics object is automatically Disposed by the
                // GdiRenderer.
                renderer.Graphics = g;
                renderer.Render(window.Document as SvgDocument);
                //window.Render();

                // put meta file on the clipboard.
                IntPtr hwnd = this.Handle;
                if (Win32.OpenClipboard(hwnd))
                {
                    Win32.EmptyClipboard();
                    IntPtr hemf = m.GetHenhmetafile();
                    Win32.SetClipboardData(14, hemf); //CF_ENHMETAFILE=14
                    Win32.CloseClipboard();
                }
            }
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void miAbout_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.Owner = this;

            aboutForm.Show();
        }

        private void SvgViewerForm_Load(object sender, EventArgs e)
        {
            addSvgTestSuiteMenu();
        }

        private void miClearCache_Click(object sender, EventArgs e)
        {
            SvgCacheManager cm = ExtendedHttpWebRequest.CacheManager as SvgCacheManager;

            if (cm != null)
            {
                long size = (long)(cm.Size / 1000);
                DialogResult result = MessageBox.Show(
                    this,
                    "The cache is " + size + "kb. Do you want to delete all content from it?",
                    "Clear cache", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    cm.Clear();
                }
            }
        }

        #endregion

        #region SVG Test suite handling

        private void loadSvgtestSuiteFile(object sender, EventArgs e)
        {
            string file = ((MenuItem)sender).Text;

            string localDir = @"E:\SU_EWD\SVG\W3C_SVG_11_FullTestSuite";
            if (!Directory.Exists(localDir))
            {
                string urlBase = "http://www.w3.org/Graphics/SVG/Test/20021112/";

                svgUrlCombo.Text = urlBase + "svggen/" + file + ".svg";
                btnGo.PerformClick();

                if (miTestShowRefImage.Checked)
                {
                    string refImageUrl = urlBase + "png/full-" + file + ".png";
                    WebRequest req = WebRequest.Create(refImageUrl);
                    WebResponse res = req.GetResponse();
                    pbRefImage.Image = Bitmap.FromStream(res.GetResponseStream());
                }
            }
            else
            {
                svgUrlCombo.Text = Path.Combine(localDir, @"svg\" + file + ".svg");
                btnGo.PerformClick();

                if (miTestShowRefImage.Checked)
                {
                    string refImageUrl = Path.Combine(localDir, @"png\full-" + file + ".png");
                    if (File.Exists(refImageUrl))
                    {
                        pbRefImage.Visible = true;
                        pbRefImage.Image = new Bitmap(refImageUrl);
                    }
                }
            }
        }

        /// <summary>
        /// Add a new Svg test suite menu item.
        /// </summary>
        /// <param name="testCategory">
        /// The test category of the test suite menu item. The test category groups
        /// the menu into a submenu.
        /// </param>
        /// <param name="testName">
        /// The name of the test suite menu item.
        /// </param>
        private void addSvgTestSuiteMenuItem(
            string testCategory,
            string testName)
        {
            foreach (MenuItem menuItem in miTestSuite.MenuItems)
            {
                if (menuItem.Text == testCategory)
                {
                    menuItem.MenuItems.Add(
                        new MenuItem(
                        testName,
                        new System.EventHandler(loadSvgtestSuiteFile)));

                    return;
                }
            }

            MenuItem newMenuItem = new MenuItem(testCategory);

            miTestSuite.MenuItems.Add(newMenuItem);
            newMenuItem.MenuItems.Add(
                new MenuItem(
                testName,
                new System.EventHandler(loadSvgtestSuiteFile)));
        }

        /// <summary>
        /// Adds the Svg test suite menu items to the form's menu.
        /// </summary>
        private void addSvgTestSuiteMenu()
        {
            addSvgTestSuiteMenuItem("animate", "animate-elem-02-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-03-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-04-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-05-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-06-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-07-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-08-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-09-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-10-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-11-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-12-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-13-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-14-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-15-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-16-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-17-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-18-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-19-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-20-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-21-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-22-b");
            addSvgTestSuiteMenuItem("animate", "animate-elem-23-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-24-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-25-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-26-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-27-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-28-t");
            addSvgTestSuiteMenuItem("animate", "animate-elem-29-b");
            addSvgTestSuiteMenuItem("color", "color-prof-01-f");
            addSvgTestSuiteMenuItem("color", "color-prop-01-b");
            addSvgTestSuiteMenuItem("color", "color-prop-02-f");
            addSvgTestSuiteMenuItem("color", "color-prop-03-t");
            addSvgTestSuiteMenuItem("coords", "coords-trans-01-b");
            addSvgTestSuiteMenuItem("coords", "coords-trans-02-t");
            addSvgTestSuiteMenuItem("coords", "coords-trans-03-t");
            addSvgTestSuiteMenuItem("coords", "coords-trans-04-t");
            addSvgTestSuiteMenuItem("coords", "coords-trans-05-t");
            addSvgTestSuiteMenuItem("coords", "coords-trans-06-t");
            addSvgTestSuiteMenuItem("coords", "coords-units-01-b");
            addSvgTestSuiteMenuItem("coords", "coords-units-02-b");
            addSvgTestSuiteMenuItem("coords", "coords-units-03-b");
            addSvgTestSuiteMenuItem("coords", "coords-viewattr-01-b");
            addSvgTestSuiteMenuItem("extend", "extend-namespace-01-f");
            addSvgTestSuiteMenuItem("filters", "filters-blend-01-b");
            addSvgTestSuiteMenuItem("filters", "filters-color-01-b");
            addSvgTestSuiteMenuItem("filters", "filters-composite-01-b");
            addSvgTestSuiteMenuItem("filters", "filters-comptran-01-b");
            addSvgTestSuiteMenuItem("filters", "filters-conv-01-f");
            addSvgTestSuiteMenuItem("filters", "filters-diffuse-01-f");
            addSvgTestSuiteMenuItem("filters", "filters-displace-01-f");
            addSvgTestSuiteMenuItem("filters", "filters-example-01-b");
            addSvgTestSuiteMenuItem("filters", "filters-gauss-01-b");
            addSvgTestSuiteMenuItem("filters", "filters-image-01-b");
            addSvgTestSuiteMenuItem("filters", "filters-light-01-f");
            addSvgTestSuiteMenuItem("filters", "filters-morph-01-f");
            addSvgTestSuiteMenuItem("filters", "filters-offset-01-b");
            addSvgTestSuiteMenuItem("filters", "filters-specular-01-f");
            addSvgTestSuiteMenuItem("filters", "filters-tile-01-b");
            addSvgTestSuiteMenuItem("fonts", "fonts-elem-01-t");
            addSvgTestSuiteMenuItem("fonts", "fonts-elem-02-t");
            addSvgTestSuiteMenuItem("fonts", "fonts-elem-03-t");
            addSvgTestSuiteMenuItem("fonts", "fonts-elem-04-t");
            addSvgTestSuiteMenuItem("interact", "interact-cursor-01-b");
            addSvgTestSuiteMenuItem("interact", "interact-dom-01-t");
            addSvgTestSuiteMenuItem("interact", "interact-events-01-t");
            addSvgTestSuiteMenuItem("interact", "interact-order-01-t");
            addSvgTestSuiteMenuItem("interact", "interact-order-02-t");
            addSvgTestSuiteMenuItem("interact", "interact-order-03-t");
            addSvgTestSuiteMenuItem("interact", "interact-zoom-01-t");
            addSvgTestSuiteMenuItem("linking", "linking-a-01-b");
            addSvgTestSuiteMenuItem("linking", "linking-a-02-b");
            addSvgTestSuiteMenuItem("linking", "linking-a-03-t");
            addSvgTestSuiteMenuItem("linking", "linking-a-04-t");
            addSvgTestSuiteMenuItem("linking", "linking-uri-01-b");
            addSvgTestSuiteMenuItem("linking", "linking-uri-02-b");
            addSvgTestSuiteMenuItem("linking", "linking-uri-03-t");
            addSvgTestSuiteMenuItem("masking", "masking-mask-01-b");
            addSvgTestSuiteMenuItem("masking", "masking-opacity-01-b");
            addSvgTestSuiteMenuItem("masking", "masking-path-01-b");
            addSvgTestSuiteMenuItem("masking", "masking-path-02-b");
            addSvgTestSuiteMenuItem("masking", "masking-path-03-t");
            addSvgTestSuiteMenuItem("masking", "masking-path-04-b");
            addSvgTestSuiteMenuItem("masking", "masking-path-05-f");
            addSvgTestSuiteMenuItem("masking", "metadata-example-01-b");
            addSvgTestSuiteMenuItem("painting", "painting-fill-01-t");
            addSvgTestSuiteMenuItem("painting", "painting-fill-02-t");
            addSvgTestSuiteMenuItem("painting", "painting-fill-03-t");
            addSvgTestSuiteMenuItem("painting", "painting-fill-04-t");
            addSvgTestSuiteMenuItem("painting", "painting-marker-01-f");
            addSvgTestSuiteMenuItem("painting", "painting-marker-02-f");
            addSvgTestSuiteMenuItem("painting", "painting-render-01-b");
            addSvgTestSuiteMenuItem("painting", "painting-stroke-01-t");
            addSvgTestSuiteMenuItem("painting", "painting-stroke-02-t");
            addSvgTestSuiteMenuItem("painting", "painting-stroke-03-t");
            addSvgTestSuiteMenuItem("painting", "painting-stroke-04-t");
            addSvgTestSuiteMenuItem("paths", "paths-data-01-t");
            addSvgTestSuiteMenuItem("paths", "paths-data-02-t");
            addSvgTestSuiteMenuItem("paths", "paths-data-03-f");
            addSvgTestSuiteMenuItem("paths", "paths-data-04-t");
            addSvgTestSuiteMenuItem("paths", "paths-data-05-t");
            addSvgTestSuiteMenuItem("paths", "paths-data-06-t");
            addSvgTestSuiteMenuItem("paths", "paths-data-07-t");
            addSvgTestSuiteMenuItem("pservers", "pservers-grad-01-b");
            addSvgTestSuiteMenuItem("pservers", "pservers-grad-02-b");
            addSvgTestSuiteMenuItem("pservers", "pservers-grad-03-b");
            addSvgTestSuiteMenuItem("pservers", "pservers-grad-04-b");
            addSvgTestSuiteMenuItem("pservers", "pservers-grad-05-b");
            addSvgTestSuiteMenuItem("pservers", "pservers-grad-06-b");
            addSvgTestSuiteMenuItem("pservers", "pservers-grad-07-b");
            addSvgTestSuiteMenuItem("pservers", "pservers-grad-08-b");
            addSvgTestSuiteMenuItem("pservers", "pservers-grad-09-b");
            addSvgTestSuiteMenuItem("pservers", "pservers-grad-10-b");
            addSvgTestSuiteMenuItem("pservers", "pservers-grad-11-b");
            addSvgTestSuiteMenuItem("pservers", "pservers-grad-12-b");
            addSvgTestSuiteMenuItem("pservers", "pservers-pattern-01-b");
            addSvgTestSuiteMenuItem("render", "render-elems-01-t");
            addSvgTestSuiteMenuItem("render", "render-elems-02-t");
            addSvgTestSuiteMenuItem("render", "render-elems-03-t");
            addSvgTestSuiteMenuItem("render", "render-elems-06-t");
            addSvgTestSuiteMenuItem("render", "render-elems-07-t");
            addSvgTestSuiteMenuItem("render", "render-elems-08-t");
            addSvgTestSuiteMenuItem("render", "render-groups-01-b");
            addSvgTestSuiteMenuItem("render", "render-groups-03-t");
            addSvgTestSuiteMenuItem("render", "render-groups-04-b");
            addSvgTestSuiteMenuItem("script", "script-handle-01-t");
            addSvgTestSuiteMenuItem("script", "script-handle-02-t");
            addSvgTestSuiteMenuItem("script", "script-handle-03-t");
            addSvgTestSuiteMenuItem("script", "script-handle-04-t");
            addSvgTestSuiteMenuItem("shapes", "shapes-circle-01-t");
            addSvgTestSuiteMenuItem("shapes", "shapes-ellipse-01-t");
            addSvgTestSuiteMenuItem("shapes", "shapes-line-01-t");
            addSvgTestSuiteMenuItem("shapes", "shapes-polygon-01-t");
            addSvgTestSuiteMenuItem("shapes", "shapes-polyline-01-t");
            addSvgTestSuiteMenuItem("shapes", "shapes-rect-01-t");
            addSvgTestSuiteMenuItem("struct", "struct-cond-01-t");
            addSvgTestSuiteMenuItem("struct", "struct-cond-02-t");
            addSvgTestSuiteMenuItem("struct", "struct-defs-01-t");
            addSvgTestSuiteMenuItem("struct", "struct-dom-01-t");
            addSvgTestSuiteMenuItem("struct", "struct-dom-02-t");
            addSvgTestSuiteMenuItem("struct", "struct-dom-03-t");
            addSvgTestSuiteMenuItem("struct", "struct-dom-04-t");
            addSvgTestSuiteMenuItem("struct", "struct-dom-05-t");
            addSvgTestSuiteMenuItem("struct", "struct-dom-06-t");
            addSvgTestSuiteMenuItem("struct", "struct-frag-01-t");
            addSvgTestSuiteMenuItem("struct", "struct-group-01-t");
            addSvgTestSuiteMenuItem("struct", "struct-group-02-t");
            addSvgTestSuiteMenuItem("struct", "struct-image-01-t");
            addSvgTestSuiteMenuItem("struct", "struct-image-02-t");
            addSvgTestSuiteMenuItem("struct", "struct-image-03-t");
            addSvgTestSuiteMenuItem("struct", "struct-symbol-01-b");
            addSvgTestSuiteMenuItem("styling", "styling-css-01-t");
            addSvgTestSuiteMenuItem("styling", "styling-css-02-t");
            addSvgTestSuiteMenuItem("styling", "styling-css-03-t");
            addSvgTestSuiteMenuItem("styling", "styling-inherit-01-b");
            addSvgTestSuiteMenuItem("styling", "styling-pres-01-t");
            addSvgTestSuiteMenuItem("text", "text-align-01-b");
            addSvgTestSuiteMenuItem("text", "text-align-02-b");
            addSvgTestSuiteMenuItem("text", "text-align-03-b");
            addSvgTestSuiteMenuItem("text", "text-align-04-b");
            addSvgTestSuiteMenuItem("text", "text-align-05-b");
            addSvgTestSuiteMenuItem("text", "text-align-06-b");
            addSvgTestSuiteMenuItem("text", "text-altglyph-01-b");
            addSvgTestSuiteMenuItem("text", "text-deco-01-b");
            addSvgTestSuiteMenuItem("text", "text-fonts-01-t");
            addSvgTestSuiteMenuItem("text", "text-fonts-02-t");
            addSvgTestSuiteMenuItem("text", "text-intro-01-t");
            addSvgTestSuiteMenuItem("text", "text-intro-02-b");
            addSvgTestSuiteMenuItem("text", "text-intro-03-b");
            addSvgTestSuiteMenuItem("text", "text-intro-04-t");
            addSvgTestSuiteMenuItem("text", "text-path-01-b");
            addSvgTestSuiteMenuItem("text", "text-spacing-01-b");
            addSvgTestSuiteMenuItem("text", "text-text-01-b");
            addSvgTestSuiteMenuItem("text", "text-text-03-b");
            addSvgTestSuiteMenuItem("text", "text-tref-01-b");
            addSvgTestSuiteMenuItem("text", "text-tselect-01-b");
            addSvgTestSuiteMenuItem("text", "text-tspan-01-b");
            addSvgTestSuiteMenuItem("text", "text-ws-01-t");
        }

        private void miShowRefImage_Click(
            object sender,
            System.EventArgs e)
        {
            miTestShowRefImage.Checked = !miTestShowRefImage.Checked;

            if (svgUrlCombo.Text.StartsWith(
                "http://www.w3.org/Graphics/SVG/Test/20021112/svggen/"))
            {
                this.pbRefImage.Visible = miTestShowRefImage.Checked;
            }
        }

        #endregion

        #region Miscellanious

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private void loadUrl(string url)
        {
            if (url.StartsWith("http://www.w3.org/Graphics/SVG/Test/20021112/svggen/"))
            {
                // SVG test suite file
                svgPictureBox.Size = new System.Drawing.Size(480, 360);
                this.svgPictureBox.Dock = System.Windows.Forms.DockStyle.None;
                this.pbRefImage.Visible = miTestShowRefImage.Checked;
            }
            else
            {
                //this.svgPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
                this.pbRefImage.Image = null;
            }

            // JR simple timing
            DateTime started = DateTime.Now;

            svgPictureBox.SourceURL = url;

            string title = svgPictureBox.Window.Document.Title;

            if (title.Length > 0)
            {
                this.Text = this.title + " - " + title;
            }
            else
            {
                this.Text = this.title;
            }

            // JR simple timing
            TimeSpan duration = DateTime.Now - started;

            statusBar.Text = "Done (" + duration + ")";

            // JR Test event
            ISvgDocument doc = svgPictureBox.Window.Document;
            doc.RootElement.AddEventListener("click", new EventListener(ClickMe), false);
        }

        private void ClickMe(IEvent e)
        {
            SvgElement el = ((SvgElement)e.Target);
            MessageBox.Show("Clicked - " + el.LocalName);
        }

        #endregion
    }
}
