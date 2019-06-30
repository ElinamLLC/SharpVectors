using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using FastColoredTextBoxNS;
using WeifenLuo.WinFormsUI.Docking;

namespace GdiW3cSvgTestSuite
{
    public partial class SvgInputDockPanel : DockPanelContent, ITestPagePanel
    {
        #region Private Fields

        private readonly ToolStripRenderer _toolStripProfessionalRenderer = new ToolStripProfessionalRenderer();

        private bool _firstSearch = true;
        private Place _startPlace;

        private bool _isMatchCase = true;
        private bool _isRegex = true;
        private bool _isWholeWord = true;

        private Style _invisibleCharsStyle = new InvisibleCharsRenderer(Pens.Gray);

        private VisualStudioToolStripExtender _vsToolStripExtender;

        private string _fullFilePath;

        #endregion

        #region Constructors and Destructor

        public SvgInputDockPanel()
        {
            InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.DockAreas     = DockAreas.Document | DockAreas.Float;

            this.CloseButton   = false;

            this._vsToolStripExtender = new VisualStudioToolStripExtender(this.components);
            _vsToolStripExtender.DefaultRenderer = _toolStripProfessionalRenderer;

            textEditor.DelayedTextChangedInterval = 1000;
            textEditor.DelayedEventsInterval = 500;
            textEditor.TextChangedDelayed += OnTextChangedDelayed;
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        public override void IdleUpdate()
        {
            if (textEditor == null)
            {
                return;
            }

            tbbUndo.Enabled = textEditor.UndoEnabled;
            tbbRedo.Enabled = textEditor.RedoEnabled;

            var isTextSelected = (textEditor.Selection.IsEmpty == false);

            tbbCut.Enabled    = isTextSelected && !textEditor.ReadOnly;
            tbbCopy.Enabled   = isTextSelected && !textEditor.ReadOnly;
            tbbDelete.Enabled = isTextSelected && !textEditor.ReadOnly;

            var fileExist = !string.IsNullOrWhiteSpace(_fullFilePath) && File.Exists(_fullFilePath);

            tbbSave.Enabled   = textEditor.IsChanged && textEditor.TextLength != 0 && fileExist;
            tbbSaveAs.Enabled = textEditor.TextLength != 0;

            tbbWordWrap.Checked = (textEditor.WordWrap == true);
            tbbShowLineNumber.Checked = (textEditor.ShowLineNumbers == true);

            tbbFind.Enabled = (textEditor.TextLength != 0);
            tbbTextBoxFind.Enabled = (textEditor.TextLength != 0);
            tbbSearchReplace.Enabled = !textEditor.ReadOnly && (textEditor.TextLength != 0);
        }

        #endregion

        #region ITestPagePanel Members

        public bool LoadDocument(string documentFilePath, SvgTestInfo testInfo, object extraInfo)
        {
            if (string.IsNullOrWhiteSpace(documentFilePath) || File.Exists(documentFilePath) == false)
            {
                return false;
            }

            this.LoadFile(documentFilePath);

            return true;
        }

        public void UnloadDocument()
        {
            textEditor.Clear();
            _fullFilePath = null;
        }

        #endregion

        #region Forms Event Handlers

        private void OnFormLoad(object sender, EventArgs e)
        {
            tbbComboBoxSyntax.ComboBox.SelectedIndex = 3;
            if (_theme != null)
            {
                _vsToolStripExtender.SetStyle(toolBar, VisualStudioToolStripExtender.VsVersion.Vs2015, _theme);
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void OnFormShown(object sender, EventArgs e)
        {

        }

        #endregion

        #region Toolbar Event Handlers

        private void OnClickFileNew(object sender, EventArgs e)
        {

        }

        private void OnClickFileOpen(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Title       = "Select An SVG File";
            openDialog.Filter      = "Svg Files (*.svg)|*.svg" + "|" + "SVG Compressed Files (*.svgz)|*.svgz";
            openDialog.Multiselect = false;
            openDialog.DefaultExt  = "*.svg";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openDialog.FileName;

                var fileExt = Path.GetExtension(filePath);
                if (string.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
                {
                    this.LoadFile(filePath);
                }
                else
                {
                    MessageBox.Show("Select file extension must be .svg or .svgz", 
                        MainForm.AppErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void OnClickCut(object sender, EventArgs e)
        {
            if (textEditor != null && !textEditor.Selection.IsEmpty)
            {
                textEditor.Cut();
            }
        }

        private void OnClickCopy(object sender, EventArgs e)
        {
            if (textEditor != null && !textEditor.Selection.IsEmpty)
            {
                textEditor.Copy();
            }
        }

        private void OnClickPaste(object sender, EventArgs e)
        {
            if (textEditor != null)
            {
                textEditor.Paste();
            }
        }

        private void OnClickDelete(object sender, EventArgs e)
        {
            if (textEditor != null && !textEditor.Selection.IsEmpty)
            {
                textEditor.ClearSelected();
            }
        }

        private void OnClickSave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_fullFilePath) || File.Exists(_fullFilePath) == false)
            {
                this.OnClickSaveAs(sender, e);
                return;
            }

            this.SaveFile(_fullFilePath, false);
        }

        private void OnClickSaveAs(object sender, EventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Title      = "Save An SVG File";
            saveDialog.Filter     = "Svg Files (*.svg)|*.svg" + "|" + "SVG Compressed Files (*.svgz)|*.svgz";
            saveDialog.DefaultExt = "*.svg";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveDialog.FileName;

                var fileExt = Path.GetExtension(filePath);
                if (string.IsNullOrWhiteSpace(fileExt) || 
                    string.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(fileExt))
                    {
                        filePath = filePath + ".svg";
                    }

                    this.SaveFile(filePath, true);
                }
                else
                {
                    MessageBox.Show("The file extension must be .svg or .svgz",
                        MainForm.AppErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void OnClickUndo(object sender, EventArgs e)
        {
            if (textEditor != null && textEditor.UndoEnabled)
            {
                textEditor.Undo();
            }
        }

        private void OnClickRedo(object sender, EventArgs e)
        {
            if (textEditor != null && textEditor.RedoEnabled)
            {
                textEditor.Redo();
            }
        }

        private void OnClickWordWrap(object sender, EventArgs e)
        {
            if (textEditor != null)
            {
                textEditor.WordWrap = !textEditor.WordWrap;
            }
        }

        private void OnClickLineNumber(object sender, EventArgs e)
        {
            if (textEditor != null)
            {
                textEditor.ShowLineNumbers = !textEditor.ShowLineNumbers;
            }
        }

        private void OnClickWhitespace(object sender, EventArgs e)
        {
            if (textEditor != null)
            {
                tbbShowWhitespace.Checked = !tbbShowWhitespace.Checked;

                HighlightInvisibleChars(textEditor.Range);
                if (textEditor != null)
                {
                    textEditor.Invalidate();
                }
            }
        }

        private void OnClickFind(object sender, EventArgs e)
        {
            textEditor.ShowFindDialog(tbbTextBoxFind.Text);
        }

        private void OnClickSearchReplace(object sender, EventArgs e)
        {
            textEditor.ShowReplaceDialog(tbbTextBoxFind.Text);
        }

        private void OnKeyPressSearch(object sender, KeyPressEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbbTextBoxFind.Text))
            {
                return;
            }

            this.FindNext(tbbTextBoxFind.Text);
        }

        private void OnTextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            // Show invisible chars
            HighlightInvisibleChars(e.ChangedRange);
        }

        #endregion

        #region Private Methods

        private void LoadFile(string documentFilePath)
        {
            this.UnloadDocument();

            string fileExt    = Path.GetExtension(documentFilePath);
            bool isCompressed = !string.IsNullOrWhiteSpace(fileExt) &&
                string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase);

            if (isCompressed)
            {
                using (var stream = File.OpenRead(documentFilePath))
                {
                    using (var zipStream = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        textEditor.OpenStream(zipStream);
                    }
                }
            }
            else
            {
                textEditor.OpenFile(documentFilePath, Encoding.UTF8);
            }

            _fullFilePath = string.Copy(documentFilePath);
        }

        private void SaveFile(string documentFilePath, bool isSaveAs)
        {
            string fileExt    = Path.GetExtension(documentFilePath);
            bool isCompressed = !string.IsNullOrWhiteSpace(fileExt) &&
                string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase);

            if (isCompressed)
            {
                using (var stream = File.Create(documentFilePath))
                {
                    using (var zipStream = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        textEditor.SaveToStream(zipStream);
                    }
                }
            }
            else
            {
                textEditor.SaveToFile(documentFilePath, Encoding.UTF8);
            }

            if (!isSaveAs)
            {
                _fullFilePath = string.Copy(documentFilePath);
            }
        }

        private void FindNext(string pattern)
        {
            try
            {
                RegexOptions opt = _isMatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
                if (!_isRegex)
                    pattern = Regex.Escape(pattern);
                if (_isWholeWord)
                    pattern = "\\b" + pattern + "\\b";
                //
                Range range = textEditor.Selection.Clone();
                range.Normalize();
                //
                if (_firstSearch)
                {
                    _startPlace = range.Start;
                    _firstSearch = false;
                }
                //
                range.Start = range.End;
                if (range.Start >= _startPlace)
                    range.End = new Place(textEditor.GetLineLength(textEditor.LinesCount - 1), textEditor.LinesCount - 1);
                else
                    range.End = _startPlace;
                //
                foreach (var r in range.GetRangesByLines(pattern, opt))
                {
                    textEditor.Selection = r;
                    textEditor.DoSelectionVisible();
                    textEditor.Invalidate();
                    return;
                }
                //
                if (range.Start >= _startPlace && _startPlace > Place.Empty)
                {
                    textEditor.Selection.Start = new Place(0, 0);
                    FindNext(pattern);
                    return;
                }
                MessageBox.Show("Not found");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void HighlightInvisibleChars(Range range)
        {
            range.ClearStyle(_invisibleCharsStyle);

            if (tbbShowWhitespace.Checked)
            {
                range.SetStyle(_invisibleCharsStyle, @".$|.\r\n|\s");
            }
        }

        #endregion

        #region Private Classes

        public sealed class InvisibleCharsRenderer : Style
        {
            private Pen _pen;

            public InvisibleCharsRenderer(Pen pen)
            {
                _pen = pen;
            }

            public override void Draw(Graphics gr, Point position, Range range)
            {
                var tb = range.tb;
                using (Brush brush = new SolidBrush(_pen.Color))
                    foreach (var place in range)
                    {
                        switch (tb[place].c)
                        {
                            case ' ':
                                var point = tb.PlaceToPoint(place);
                                point.Offset(tb.CharWidth / 2, tb.CharHeight / 2);
                                gr.DrawLine(_pen, point.X, point.Y, point.X + 1, point.Y);
                                break;
                        }

                        if (tb[place.iLine].Count - 1 == place.iChar)
                        {
                            var point = tb.PlaceToPoint(place);
                            point.Offset(tb.CharWidth, 0);
                            gr.DrawString("¶", tb.Font, brush, point);
                        }
                    }
            }
        }


        #endregion
    }
}
