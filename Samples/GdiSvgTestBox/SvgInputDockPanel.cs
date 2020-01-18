using System;
using System.IO;
using System.Xml;
using System.IO.Compression;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
using ICSharpCode.TextEditor.Document;

using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Events;

namespace GdiSvgTestBox
{
    public partial class SvgInputDockPanel : DockPanelContent, ITestPagePanel
    {
        #region Private Fields

        private const string ValidSVG = "<svg xmlns=\"http://www.w3.org/2000/svg\"></svg>";

        private const string SvgFileName  = "TextBoxTestFile.svg";
        private const string BackFileName = "TextBoxTestFile.bak";

        private readonly ToolStripRenderer _toolStripProfessionalRenderer = new ToolStripProfessionalRenderer();

        private VisualStudioToolStripExtender _vsToolStripExtender;

        private bool _isDocumentChanged;
        private string _currentFileName;
        private string _svgFilePath;
        private string _backFilePath;

        private SizeModeForm _sizeModePanel;
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

            this._vsToolStripExtender = new VisualStudioToolStripExtender(this.components);
            _vsToolStripExtender.DefaultRenderer = _toolStripProfessionalRenderer;

            //string workingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string workingDir = Path.GetFullPath("..\\");

            _svgFilePath  = Path.Combine(workingDir, SvgFileName);
            _backFilePath = Path.Combine(workingDir, BackFileName);

            HighlightingManager.Manager.AddSyntaxModeFileProvider(new ResourceSyntaxModeProvider());

            textEditor.ConvertTabsToSpaces = true;
            textEditor.IsReadOnly          = false;
            textEditor.LineViewerStyle     = LineViewerStyle.FullRow;
            textEditor.ShowEOLMarkers      = false;
            textEditor.ShowHRuler          = false;
            textEditor.ShowSpaces          = false;
            textEditor.ShowTabs            = false;
            textEditor.TabIndex            = 0;
//            textEditor.TextRenderingHint   = TextRenderingHint.ClearTypeGridFit;
            textEditor.Encoding            = Encoding.UTF8;
            textEditor.ShowMatchingBracket = true;
            textEditor.EnableFolding       = true;
            textEditor.IsIconBarVisible    = true;

            textEditor.TextEditorProperties.Font = new Font("Consolas", 12);

            textEditor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("XML");
            textEditor.Document.FoldingManager.FoldingStrategy = new XmlFoldingStrategy();
            textEditor.Document.FormattingStrategy = new XmlFormattingStrategy();
            textEditor.Document.FoldingManager.UpdateFoldings(string.Empty, null);

            textEditor.Document.DocumentChanged += OnDocumentChanged;
        }

        private void OnDocumentChanged(object sender, DocumentEventArgs e)
        {
            _isDocumentChanged = true;
            textEditor.Document.FoldingManager.UpdateFoldings(string.Empty, null);

            var textArea = textEditor.ActiveTextAreaControl.TextArea;

            textArea.Refresh(textArea.FoldMargin);
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

            var selectManager  = textEditor.ActiveTextAreaControl.SelectionManager;
            var isTextSelected = selectManager.HasSomethingSelected;

            tbbCut.Enabled    = isTextSelected && !textEditor.IsReadOnly;
            tbbCopy.Enabled   = isTextSelected && !textEditor.IsReadOnly;
            tbbDelete.Enabled = isTextSelected && !textEditor.IsReadOnly;

            var fileExist = !string.IsNullOrWhiteSpace(_currentFileName) && File.Exists(_currentFileName);

            var validLen = ValidSVG.Length;            

            tbbSave.Enabled    = _isDocumentChanged && document.TextLength > validLen && fileExist;
            tbbSaveAs.Enabled  = document.TextLength > validLen;
            tbbFormat.Enabled  = document.TextLength > validLen;
            tbbConvert.Enabled = document.TextLength > validLen;

            tbbShowLineNumber.Checked = (textEditor.ShowLineNumbers == true);

            tbbFind.Enabled = (document.TextLength != 0);
            tbbTextBoxFind.Enabled = (document.TextLength != 0);
            tbbSearchReplace.Enabled = !textEditor.IsReadOnly && (document.TextLength != 0);
        }

        public override void OnPageDeselected(EventArgs e)
        {
            base.OnPageDeselected(e);

            if (_sizeModePanel != null && _sizeModePanel.IsDisposed == false)
            {
                _sizeModePanel.Hide();
            }

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

            if (_sizeModePanel != null && _sizeModePanel.IsDisposed == false)
            {
                _sizeModePanel.Show();
            }
        }

        #endregion

        #region ITestPagePanel Members

        public async Task<bool> LoadDocument(string documentFilePath)
        {
            if (string.IsNullOrWhiteSpace(documentFilePath) || File.Exists(documentFilePath) == false)
            {
                return false;
            }

            await this.LoadFile(documentFilePath);

            return true;
        }

        public void UnloadDocument(bool clearText = false)
        {
            if (clearText)
            {
                if (textEditor != null)
                {
                    textEditor.Text = string.Empty;
                }
            }
            _currentFileName = null;
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
            if (_sizeModePanel == null || _sizeModePanel.IsDisposed)
            {
                _sizeModePanel = new SizeModeForm();
                _sizeModePanel.Owner = this;
            }
            _sizeModePanel.TargetControl = svgPictureBox;
            _sizeModePanel.TargetForm    = _mainForm;

            var posRect = svgPictureBox.RectangleToScreen(svgPictureBox.ClientRectangle);
            var winSize = _sizeModePanel.Size;

            _sizeModePanel.Left = posRect.Right - winSize.Width - SearchGlobals.Offset;
            _sizeModePanel.Top = posRect.Top + SearchGlobals.Offset;

            _sizeModePanel.Show();

            timerUpdates.Enabled = true;
        }

        private async void OnDragDrop(object sender, DragEventArgs de)
        {
            string fileName = "";
            if (de.Data.GetDataPresent(DataFormats.Text))
            {
                fileName = (string)de.Data.GetData(DataFormats.Text);
            }
            else if (de.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileNames;
                fileNames = (string[])de.Data.GetData(DataFormats.FileDrop);
                fileName = fileNames[0];
            }

            if (!string.IsNullOrEmpty(fileName))
            {
            }
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return;
            }
            string fileExt = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(fileExt))
            {
                return;
            }
            if (!string.Equals(fileExt, ".svg", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (!string.IsNullOrWhiteSpace(_svgFilePath))
                {
                    if (File.Exists(_svgFilePath))
                    {
                        File.Delete(_svgFilePath);
                    }
                }
                if (!string.IsNullOrWhiteSpace(_backFilePath))
                {
                    if (File.Exists(_backFilePath))
                    {
                        File.Delete(_backFilePath);
                    }
                }

                await this.LoadDocument(fileName);
            }
            catch (Exception ex)
            {
                this.ReportError(ex);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void OnDragEnter(object sender, DragEventArgs de)
        {            
            if (de.Data.GetDataPresent(DataFormats.Text) ||
               de.Data.GetDataPresent(DataFormats.FileDrop))
            {
                de.Effect = DragDropEffects.Copy;
            }
            else
            {
                de.Effect = DragDropEffects.None;
            }

        }

        private void OnSplitterMoved(object sender, SplitterEventArgs e)
        {
            if (_sizeModePanel == null || _sizeModePanel.IsDisposed)
            {
                return;
            }

            var offsetLimit = SearchGlobals.Offset * 2;

            var posRect = svgPictureBox.RectangleToScreen(svgPictureBox.ClientRectangle);
            if (posRect.Height < _sizeModePanel.Height + offsetLimit || posRect.Width < _sizeModePanel.Width + offsetLimit)
            {
                _sizeModePanel.Hide();
                return;
            }
            var winSize = _sizeModePanel.Size;

            _sizeModePanel.Left = posRect.Right - winSize.Width - SearchGlobals.Offset;
            _sizeModePanel.Top = posRect.Top + SearchGlobals.Offset;

            _sizeModePanel.Show();
        }

        private async void OnTimeTick(object sender, EventArgs e)
        {
            timerUpdates.Enabled = false;

            if (!string.IsNullOrWhiteSpace(_svgFilePath) && File.Exists(_svgFilePath))
            {
                await this.LoadDocument(_svgFilePath);
            }
        }

        #endregion

        #region Toolbar Event Handlers

        private void OnClickFileNew(object sender, EventArgs e)
        {
            if (textEditor.Document.TextLength != 0)
            {
                var dlgResult = MessageBox.Show("This will clear the current text in the document. Do you want to continue?", 
                    MainForm.AppTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dlgResult == DialogResult.Yes)
                {
                    textEditor.Text = "";
                }
            }

            _isDocumentChanged = false;
            textEditor.Document.UndoStack.ClearAll();

            textEditor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("XML");
            textEditor.Document.FoldingManager.FoldingStrategy = new XmlFoldingStrategy();
        }

        private async void OnClickFileOpen(object sender, EventArgs e)
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
                    _currentFileName = new string(filePath.ToCharArray());

                    if (!string.IsNullOrWhiteSpace(_svgFilePath))
                    {
                        if (File.Exists(_svgFilePath))
                        {
                            File.Delete(_svgFilePath);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(_backFilePath))
                    {
                        if (File.Exists(_backFilePath))
                        {
                            File.Delete(_backFilePath);
                        }
                    }
                    await this.LoadFile(filePath);
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
            if (string.IsNullOrWhiteSpace(_currentFileName) || File.Exists(_currentFileName) == false)
            {
                this.OnClickSaveAs(sender, e);
                return;
            }

            this.SaveFile(_currentFileName, false);
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
                textEditor.ShowSpaces     = tbbShowWhitespace.Checked;
                textEditor.ShowTabs       = tbbShowWhitespace.Checked;

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

            _searchPanel.SearchText       = tbbTextBoxFind.Text;
            _searchPanel.IsMatchCase      = _isMatchCase;
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

            _replacePanel.SearchText       = tbbTextBoxFind.Text;
            _replacePanel.IsMatchCase      = _isMatchCase;
            _replacePanel.IsMatchWholeWord = _isMatchWholeWord;

            _replacePanel.Show(_mainForm, textEditor, searchListener, true);
        }

        private void SearchTextChanged(string searchText, bool isMatchCase, bool isMatchWholeWord)
        {
            tbbTextBoxFind.Text = searchText;
            _isMatchCase        = isMatchCase;
            _isMatchWholeWord   = isMatchWholeWord;
        }

        private void OnKeyPressSearch(object sender, KeyPressEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbbTextBoxFind.Text))
            {
                return;
            }

            this.FindNext(tbbTextBoxFind.Text);
        }

        private void OnClickFormat(object sender, EventArgs e)
        {
            if (textEditor == null)
            {
                return;
            }
            string inputText = textEditor.Text;
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return;
            }

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();
            document.XmlResolver = null;

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(inputText);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                textEditor.Text = sReader.ReadToEnd();
            }
            catch (XmlException)
            {
                // Handle the exception
            }

            mStream.Close();
            writer.Close();
        }

        private async void OnClickConvert(object sender, EventArgs e)
        {
            this.UnloadDocument();

            try
            {
                if (!string.IsNullOrWhiteSpace(_svgFilePath))
                {
                    if (File.Exists(_svgFilePath))
                    {
                        File.Delete(_svgFilePath);
                    }
                }
                if (!string.IsNullOrWhiteSpace(_backFilePath))
                {
                    if (File.Exists(_backFilePath))
                    {
                        File.Delete(_backFilePath);
                    }
                }

                if (this.SaveDocument() && File.Exists(_svgFilePath))
                {
                    await this.RenderSvgSource(_svgFilePath);
                }
            }
            catch (Exception ex)
            {
                this.ReportError(ex);
            }
        }

        #endregion

        #region Private Methods

        private bool SaveDocument()
        {
            try
            {
                string inputText = textEditor.Text;
                if (string.IsNullOrWhiteSpace(inputText))
                {
                    return false;
                }

                this.SaveFile(_backFilePath, false);

                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Parse,
                    XmlResolver = null
                };

                using (var textReader = new StreamReader(_backFilePath))
                {
                    using (var reader = XmlReader.Create(textReader, settings))
                    {
                        var document = new XmlDocument();
                        document.Load(reader);
                    }
                }

                if (File.Exists(_svgFilePath))
                {
                    File.Delete(_svgFilePath);
                }

                File.Move(_backFilePath, _svgFilePath);

                _isDocumentChanged = false;

                return true;
            }
            catch (Exception ex)
            {
                this.ReportError(ex);

                return false;
            }
        }

        private void ReportInfo(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            Trace.TraceInformation(message);

            MessageBox.Show(message, MainForm.AppErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ReportError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            Trace.TraceError(message);

            MessageBox.Show(message, MainForm.AppErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ReportError(Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            Trace.TraceError(ex.ToString());

            MessageBox.Show(ex.ToString(), MainForm.AppErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async Task LoadFile(string documentFilePath)
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
                        textEditor.LoadFile(documentFilePath, zipStream, false, true);
                    }
                }
            }
            else
            {
                textEditor.LoadFile(documentFilePath);
            }
            if (!string.Equals(documentFilePath, _svgFilePath, StringComparison.OrdinalIgnoreCase))
            {
                this.SaveDocument();
            }

            textEditor.Document.FoldingManager.UpdateFoldings(string.Empty, null);

            _currentFileName = new string(documentFilePath.ToCharArray());

            await this.RenderSvgSource(_currentFileName);
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
                _currentFileName = new string(documentFilePath.ToCharArray());
            }
        }

        private void FindNext(string pattern)
        {
        }

        private async Task RenderSvgSource(string svgSource)
        {
            if (string.IsNullOrWhiteSpace(svgSource))
            {
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                await svgPictureBox.LoadAsync(svgSource);

                // JR Test event
                ISvgDocument doc = svgPictureBox.Window.Document;
                if (doc != null && doc.RootElement != null)
                {
                    doc.RootElement.AddEventListener("click", new EventListener(OnSvgElementClicked), false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), MainForm.AppErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        public void OnSvgElementClicked(IEvent e)
        {
            SvgElement el = ((SvgElement)e.Target);

            HitTestForm _hitTestForm = new HitTestForm();

            var title = string.Format("Clicked - LocalName = {0}, ID = {1}", el.LocalName, el.Id);
            _hitTestForm.SetTexts(title, el.OuterXml);

            _hitTestForm.ShowDialog(this);
        }

        #endregion
    }
}
