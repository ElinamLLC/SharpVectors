using System;
using System.IO;
using System.IO.Compression;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
using ICSharpCode.TextEditor.Document;

namespace GdiW3cSvgTestSuite
{
    public partial class SvgInputDockPanel : DockPanelContent, ITestPagePanel
    {
        #region Private Fields

        private const string ValidSVG = "<svg xmlns=\"http://www.w3.org/2000/svg\"></svg>";

        private readonly ToolStripRenderer _toolStripProfessionalRenderer = new ToolStripProfessionalRenderer();

        private VisualStudioToolStripExtender _vsToolStripExtender;

        private string _fullFilePath;

        private bool _isDocumentChanged;
        private string _currentFileName;

        private SearchForm _searchPanel;
        private ReplaceForm _replacePanel;

        private bool _isMatchCase;
        private bool _isMatchWholeWord;

        #endregion

        #region Constructors and Destructor

        public SvgInputDockPanel()
        {
            InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.DockAreas     = DockAreas.Document | DockAreas.Float;

            this.CloseButton   = false;

            if (this.components == null)
            {
                this.components = new System.ComponentModel.Container();
            }

            this._vsToolStripExtender = new VisualStudioToolStripExtender(this.components);
            _vsToolStripExtender.DefaultRenderer = _toolStripProfessionalRenderer;

            HighlightingManager.Manager.AddSyntaxModeFileProvider(new ResourceSyntaxModeProvider());

            textEditor.ConvertTabsToSpaces = true;
            textEditor.IsReadOnly          = false;
            textEditor.LineViewerStyle     = LineViewerStyle.FullRow;
            textEditor.ShowEOLMarkers      = false;
            textEditor.ShowHRuler          = false;
            textEditor.ShowSpaces          = false;
            textEditor.ShowTabs            = false;
            textEditor.TabIndex            = 0;
            textEditor.Encoding            = Encoding.UTF8;
            textEditor.ShowMatchingBracket = true;
            textEditor.EnableFolding       = true;
            textEditor.IsIconBarVisible    = true;
//            textEditor.TextRenderingHint   = TextRenderingHint.ClearTypeGridFit;

            textEditor.TextEditorProperties.Font = new Font("Consolas", 12);

            textEditor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("XML");
            textEditor.Document.FoldingManager.FoldingStrategy = new XmlFoldingStrategy();
            textEditor.Document.FormattingStrategy = new XmlFormattingStrategy();
            textEditor.Document.FoldingManager.UpdateFoldings(string.Empty, null);

            textEditor.Document.DocumentChanged += OnDocumentChanged;
        }

        #endregion

        #region Public Properties

        public override ThemeBase Theme
        {
            get {
                return base.Theme;
            }
            set {
                if (value != null && value != _theme)
                {
                    base.Theme = value;
                    _vsToolStripExtender.SetStyle(toolBar, VisualStudioToolStripExtender.VsVersion.Vs2015, value);
                }
            }
        }

        #endregion

        #region Public Methods

        public override void IdleUpdate()
        {
            if (textEditor == null)
            {
                return;
            }

            var document = textEditor.Document;

            tbbUndo.Enabled = textEditor.EnableUndo;
            tbbRedo.Enabled = textEditor.EnableRedo;

            var selectManager = textEditor.ActiveTextAreaControl.SelectionManager;
            var isTextSelected = selectManager.HasSomethingSelected;

            tbbCut.Enabled = isTextSelected && !textEditor.IsReadOnly;
            tbbCopy.Enabled = isTextSelected && !textEditor.IsReadOnly;
            tbbDelete.Enabled = isTextSelected && !textEditor.IsReadOnly;

            var fileExist = !string.IsNullOrWhiteSpace(_currentFileName) && File.Exists(_currentFileName);

            var validLen = ValidSVG.Length;

            tbbSave.Enabled = _isDocumentChanged && document.TextLength > validLen && fileExist;
            tbbSaveAs.Enabled = document.TextLength > validLen;
            //tbbFormat.Enabled = document.TextLength > validLen;
            //tbbConvert.Enabled = document.TextLength > validLen;

            tbbShowLineNumber.Checked = (textEditor.ShowLineNumbers == true);

            tbbFind.Enabled = (document.TextLength != 0);
            tbbTextBoxFind.Enabled = (document.TextLength != 0);
            tbbSearchReplace.Enabled = !textEditor.IsReadOnly && (document.TextLength != 0);
        }

        public override void OnPageDeselected(EventArgs e)
        {
            base.OnPageDeselected(e);

            if (_searchPanel != null && _searchPanel.IsDisposed == false)
            {
                _searchPanel.Close();
                _searchPanel = null;
            }

            if (_replacePanel != null && _replacePanel.IsDisposed == false)
            {
                _replacePanel.Close();
                _replacePanel = null;
            }
        }

        public override void OnPageSelected(EventArgs e)
        {
            base.OnPageSelected(e);
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
            if (textEditor != null)
            {
                textEditor.Text = string.Empty;
            }
            _currentFileName = null;
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
            //if (textEditor.Document.TextLength != 0)
            //{
            //    var dlgResult = MessageBox.Show("This will clear the current text in the document. Do you want to continue?",
            //        MainForm.AppTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            //    if (dlgResult == DialogResult.Yes)
            //    {
            //        textEditor.Text = "";
            //    }
            //}

            //_isDocumentChanged = false;
            //textEditor.Document.UndoStack.ClearAll();

            //textEditor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("XML");
            //textEditor.Document.FoldingManager.FoldingStrategy = new XmlFoldingStrategy();
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
            if (textEditor != null)
            {
                var clipboardHander = textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler;

                if (clipboardHander.EnableCut)
                {
                    clipboardHander.Cut(null, null);
                }
            }
        }

        private void OnClickCopy(object sender, EventArgs e)
        {
            if (textEditor != null)
            {
                var clipboardHander = textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler;

                if (clipboardHander.EnableCopy)
                {
                    clipboardHander.Copy(null, null);
                }
            }
        }

        private void OnClickPaste(object sender, EventArgs e)
        {
            if (textEditor != null)
            {
                var clipboardHander = textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler;

                if (clipboardHander.EnablePaste)
                {
                    clipboardHander.Paste(null, null);
                }
            }
        }

        private void OnClickDelete(object sender, EventArgs e)
        {
            if (textEditor != null)
            {
                var clipboardHander = textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler;

                if (clipboardHander.EnableDelete)
                {
                    clipboardHander.Delete(null, null);
                }
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
            if (textEditor != null && textEditor.EnableUndo)
            {
                textEditor.Undo();
            }
        }

        private void OnClickRedo(object sender, EventArgs e)
        {
            if (textEditor != null && textEditor.EnableRedo)
            {
                textEditor.Redo();
            }
        }

        private void OnClickWordWrap(object sender, EventArgs e)
        {
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

                textEditor.ShowEOLMarkers = tbbShowWhitespace.Checked;
                textEditor.ShowSpaces = tbbShowWhitespace.Checked;
                textEditor.ShowTabs = tbbShowWhitespace.Checked;

                textEditor.Refresh();
            }
        }

        private void OnClickFind(object sender, EventArgs e)
        {
            if (_replacePanel != null && !_replacePanel.IsDisposed)
            {
                _replacePanel.Hide();
            }

            if (_searchPanel == null || _searchPanel.IsDisposed)
            {
                _searchPanel = new SearchForm();
                _searchPanel.Owner = this;
            }

            var posRect = textEditor.RectangleToScreen(textEditor.ClientRectangle);
            var winSize = _searchPanel.Size;

            _searchPanel.Left = posRect.Right - winSize.Width - SearchGlobals.Offset * 4;
            _searchPanel.Top = posRect.Top + SearchGlobals.Offset;


            Action<string, bool, bool> searchListener = this.SearchTextChanged;

            _searchPanel.SearchText = tbbTextBoxFind.Text;
            _searchPanel.IsMatchCase = _isMatchCase;
            _searchPanel.IsMatchWholeWord = _isMatchWholeWord;

            _searchPanel.Show(_mainForm, textEditor, searchListener, false);
        }

        private void OnClickSearchReplace(object sender, EventArgs e)
        {
            if (_searchPanel != null && !_searchPanel.IsDisposed)
            {
                _searchPanel.Hide();
            }

            if (_replacePanel == null || _replacePanel.IsDisposed)
            {
                _replacePanel = new ReplaceForm();
                _replacePanel.Owner = this;
            }

            var posRect = textEditor.RectangleToScreen(textEditor.ClientRectangle);
            var winSize = _replacePanel.Size;

            _replacePanel.Left = posRect.Right - winSize.Width - SearchGlobals.Offset * 4;
            _replacePanel.Top = posRect.Top + SearchGlobals.Offset;

            Action<string, bool, bool> searchListener = this.SearchTextChanged;

            _replacePanel.SearchText = tbbTextBoxFind.Text;
            _replacePanel.IsMatchCase = _isMatchCase;
            _replacePanel.IsMatchWholeWord = _isMatchWholeWord;

            _replacePanel.Show(_mainForm, textEditor, searchListener, true);
        }

        private void SearchTextChanged(string searchText, bool isMatchCase, bool isMatchWholeWord)
        {
            tbbTextBoxFind.Text = searchText;
            _isMatchCase = isMatchCase;
            _isMatchWholeWord = isMatchWholeWord;
        }

        private void OnKeyPressSearch(object sender, KeyPressEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbbTextBoxFind.Text))
            {
                return;
            }

            this.FindNext(tbbTextBoxFind.Text);
        }

        #endregion

        #region Private Methods

        private void OnDocumentChanged(object sender, DocumentEventArgs e)
        {
            _isDocumentChanged = true;
            textEditor.Document.FoldingManager.UpdateFoldings(string.Empty, null);

            var textArea = textEditor.ActiveTextAreaControl.TextArea;

            textArea.Refresh(textArea.FoldMargin);
        }

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
                        textEditor.LoadFile(documentFilePath, zipStream, false, false);
                    }
                }
            }
            else
            {
                textEditor.LoadFile(documentFilePath);
            }

            _fullFilePath = new string(documentFilePath.ToCharArray());
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
                        textEditor.SaveFile(zipStream);
                    }
                }
            }
            else
            {
                textEditor.SaveFile(documentFilePath);
            }

            if (!isSaveAs)
            {
                _fullFilePath = new string(documentFilePath.ToCharArray());
            }
        }

        private void FindNext(string pattern)
        {
        }

        #endregion
    }
}
