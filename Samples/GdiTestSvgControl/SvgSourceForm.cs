using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TestSvgControl
{
    public partial class SvgSourceForm : Form
    {
        private string _svgSource;

        private IList<WikipediaSource> _wikiSources;

        public SvgSourceForm()
        {
            InitializeComponent();

            _wikiSources = new List<WikipediaSource>()
            {
                new WikipediaSource("Select Web Sources...", 
                    ""),
                new WikipediaSource("Valid SVG created with Adobe Illustrator", 
                    "https://upload.wikimedia.org/wikipedia/commons/a/a2/2001_ROCLY_cartogram-de.svg"),
                new WikipediaSource("Valid SVG created with Affinity Designer",
                    "https://upload.wikimedia.org/wikipedia/commons/0/04/WikiLovesTVRadio_3.svg"),
                new WikipediaSource("Valid SVG created with ArcMap",
                    "https://upload.wikimedia.org/wikipedia/commons/0/03/Abies_alba_range.svg"),
                new WikipediaSource("Valid SVG created with Asymptote",
                    "https://upload.wikimedia.org/wikipedia/commons/7/79/Fractalgrids.svg"),
                new WikipediaSource("Valid SVG created with Batik:Structural formulas",
                    "https://upload.wikimedia.org/wikipedia/commons/d/d8/Yttrium-90_edotreotide.svg"),
                new WikipediaSource("Valid SVG created with ChemDraw",
                    "https://upload.wikimedia.org/wikipedia/commons/9/97/Sucrose_condensation.svg"),
                new WikipediaSource("Valid SVG created with CorelDRAW",
                    "https://upload.wikimedia.org/wikipedia/commons/3/37/Compass_Card.svg"),
                new WikipediaSource("Valid SVG created with Dia",
                    "https://upload.wikimedia.org/wikipedia/commons/0/08/Dewp-Hauptseite-Interwiki-Links.svg"),
                new WikipediaSource("Valid SVG created with DrawPlus:Emblems",
                    "https://upload.wikimedia.org/wikipedia/commons/2/2f/Great_Seal_of_the_Navajo_Nation.svg"),
                new WikipediaSource("Valid SVG created with electrical symbols library",
                    "https://upload.wikimedia.org/wikipedia/commons/b/be/Ttl_demultiplexer_3x_2bit.svg"),
                new WikipediaSource("Valid SVG created with ELKI",
                    "https://upload.wikimedia.org/wikipedia/commons/d/d1/KMeans-density-data.svg"),
                new WikipediaSource("Valid SVG created with Freemind",
                    "https://upload.wikimedia.org/wikipedia/commons/7/79/Maslow_tr.svg"),
                new WikipediaSource("Valid SVG created with GChemPaint",
                    "https://upload.wikimedia.org/wikipedia/commons/9/98/Bakelit_Struktur.svg"),
                new WikipediaSource("Valid SVG created with GeoGebra",
                    "https://upload.wikimedia.org/wikipedia/commons/5/5a/Gauss_bodenmiller_in_complete_quadrilateral.svg"),
                new WikipediaSource("Valid SVG created with GIMP",
                    "https://upload.wikimedia.org/wikipedia/commons/5/50/DEU_Legden_COA.svg"),
                new WikipediaSource("Valid SVG created with Gnuplot",
                    "https://upload.wikimedia.org/wikipedia/commons/0/02/Helicoid.svg"),
                new WikipediaSource("Valid SVG created with Google Drive",
                    "https://upload.wikimedia.org/wikipedia/commons/1/14/Oslo_T-bane_linjekart.svg"),
                new WikipediaSource("Valid SVG created with Graphviz",
                    "https://upload.wikimedia.org/wikipedia/commons/4/48/Contiguite_Departements_de_France.svg"),
                new WikipediaSource("Valid SVG created with Inkscape",
                    "https://upload.wikimedia.org/wikipedia/commons/b/b8/00-05-2014_Olivio_Dutra.svg"),
                new WikipediaSource("Valid SVG created with LaTeX",
                    "https://upload.wikimedia.org/wikipedia/commons/9/99/Electrical_Telegraph_Schematic.svg"),
                new WikipediaSource("Valid SVG created with LibreOffice",
                    "https://upload.wikimedia.org/wikipedia/commons/8/8e/Chopin_Prelude_7.svg"),
                new WikipediaSource("Valid SVG created with Mathematica",
                    "https://upload.wikimedia.org/wikipedia/commons/d/d9/FreeBeamVibrationPlot.svg"),
                new WikipediaSource("Valid SVG created with MATLAB",
                    "https://upload.wikimedia.org/wikipedia/commons/1/1c/Kruskal_diagram_of_Schwarzschild_chart.svg"),
                new WikipediaSource("Valid SVG created with Matplotlib",
                    "https://upload.wikimedia.org/wikipedia/commons/a/a8/Cartes_couleur_sinFoisSin_python_matplotlib.svg"),
                new WikipediaSource("Valid SVG created with OpenOffice.org",
                    "https://upload.wikimedia.org/wikipedia/commons/0/02/DielessDrawing.svg"),
                new WikipediaSource("Valid SVG created with Potrace",
                    "https://upload.wikimedia.org/wikipedia/commons/4/4e/Polydactyly_01_Lhand_AP-vectorized-12color.svg"),
                new WikipediaSource("Valid SVG created with QGIS",
                    "https://upload.wikimedia.org/wikipedia/commons/6/6a/102nd_Intelligence_Wing_emblem.svg"),
                new WikipediaSource("Valid SVG created with R",
                    "https://upload.wikimedia.org/wikipedia/commons/2/2e/Sealevel-rise_1870-2009_de.svg"),
                new WikipediaSource("Valid SVG created with Sodipodi",
                    "https://upload.wikimedia.org/wikipedia/commons/b/b0/NewTux.svg"),
                new WikipediaSource("Valid SVG created with SVG-edit",
                    "https://upload.wikimedia.org/wikipedia/commons/5/53/Arutz_20.svg"),
                new WikipediaSource("Valid SVG created with Visio", 
                "https://upload.wikimedia.org/wikipedia/commons/c/cb/ShanghaiMetro141228.svg")
            };

            this.DialogResult = DialogResult.Cancel;
        }

        public string SvgSource
        {
            get {
                return _svgSource;
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            int selectedIndex = cbxWiki.SelectedIndex;
            if (selectedIndex >= 0 && cbxWiki.Items.Count != 0)
            {
                var wikiSource = _wikiSources[selectedIndex];

                txtSource.Text = wikiSource.SourceUri; // Selected
            }
        }

        private void OnSourceClick(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title      = "Select An SVG File";
            dlg.DefaultExt = "*.svg";
            dlg.Filter     = "All SVG Files (*.svg,*.svgz)|*.svg;*.svgz"
                                + "|Svg Uncompressed Files (*.svg)|*.svg"
                                + "|SVG Compressed Files (*.svgz)|*.svgz";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (cbxWiki.Items.Count != 0)
                {
                    cbxWiki.SelectedIndex = 0; // Empty
                }
                txtSource.Text = dlg.FileName;
            }
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void OnOKClick(object sender, EventArgs e)
        {
            var svgSource = txtSource.Text.Trim();
            if (string.IsNullOrWhiteSpace(svgSource))
            {
                MessageBox.Show("Please enter a valid file path or select a web uri.",
                    "SharpVectors: GDI+ Sample", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (File.Exists(svgSource))
            {
                _svgSource = svgSource;
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            try
            {
                Uri uri = new Uri(svgSource);
                if (!uri.IsAbsoluteUri)
                {
                    MessageBox.Show("Please enter a valid file path or select a web uri.",
                        "SharpVectors: GDI+ Sample", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                _svgSource = svgSource;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (UriFormatException)
            {
                MessageBox.Show("Please enter a valid file path or select a web uri.",
                    "SharpVectors: GDI+ Sample", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            if (_wikiSources != null && _wikiSources.Count != 0)
            {
                cbxWiki.BeginUpdate();
                foreach (var wikiSource in _wikiSources)
                {
                    cbxWiki.Items.Add(wikiSource.SourceName);
                }
                cbxWiki.EndUpdate();
            }
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            if (cbxWiki.Items.Count != 0)
            {
                cbxWiki.SelectedIndex = cbxWiki.Items.Count - 1; // Visio
            }
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void OnLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://commons.wikimedia.org/wiki/Category:Valid_SVG_created_with");
        }

        private sealed class WikipediaSource
        {
            public WikipediaSource(string sourceName, string sourceUri)
            {
                this.SourceName = sourceName;
                this.SourceUri  = sourceUri;
            }

            public string SourceName { get; set; }

            public string SourceUri { get; set; }
        }
    }
}
