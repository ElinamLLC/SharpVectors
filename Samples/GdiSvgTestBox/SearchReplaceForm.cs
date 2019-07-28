using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace GdiSvgTestBox
{
    public partial class SearchReplaceForm : Form
    {
        #region Private Fields

        private PopupMessageContainer _popupHandler;

        #endregion

        #region Protected Fields

        protected int _searchCount;

        protected bool _isInitialized;

        protected bool _panelIsHidden;

        protected MainForm _mainForm;
        protected TextEditorSearcher _searchHandler;
        protected TextEditorControl _editor;

        protected bool _replaceMode;
        protected bool _isMatchCase;
        protected bool _isMatchWholeWord;

        protected bool _lastSearchWasBackward;
        protected bool _lastSearchLoopedAround;

        protected Action<string, bool, bool> _searchOptonsUpdate;

        protected Dictionary<TextEditorControl, HighlightGroup> _highlightGroups;

        #endregion

        #region Constructors and Destructor

        public SearchReplaceForm()
        {
            InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font(DockPanelContent.PanelDefaultFont, 14F, FontStyle.Regular, GraphicsUnit.World);

            this.StartPosition = FormStartPosition.Manual;

            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.UpdateStyles();

            this.BackColor = Color.FromArgb((int)(255 * SearchGlobals.FadeOpacity), 0, 122, 204);

            if (!this.DesignMode)
            {
                this.FormClosing += this.OnFormClosing;
                this.FormClosed  += this.OnFormClosed;
                this.Load        += this.OnFormLoad;
                this.Shown       += this.OnFormShown;
            }

            _panelIsHidden = false;

            _searchHandler   = new TextEditorSearcher();
            _highlightGroups = new Dictionary<TextEditorControl, HighlightGroup>();
        }

        #endregion

        #region Public Properties

        public TextEditorControl Editor
        {
            get { return _editor; }
            set {
                _editor = value;
                _searchHandler.Document = _editor.Document;
                UpdateSelectionState();
            }
        }

        public MainForm MainForm
        {
            get {
                return _mainForm;
            }
            set {
                _mainForm = value;
            }
        }

        public bool IsMatchCase
        {
            get {
                return _isMatchCase;
            }
            set {
                _isMatchCase = value;
            }
        }

        public bool IsMatchWholeWord
        {
            get {
                return _isMatchWholeWord;
            }
            set {
                _isMatchWholeWord = value;
            }
        }

        public string SearchText
        {
            get {
                var searchBox = this.SearchBox;
                if (searchBox != null)
                {
                    return searchBox.Text;
                }
                return string.Empty;
            }
            set {
                var searchBox = this.SearchBox;
                if (searchBox != null)
                {
                    searchBox.Text = value;
                }
                _searchOptonsUpdate?.Invoke(value, _isMatchCase, _isMatchWholeWord);
            }
        }

        public virtual TextBox SearchBox
        {
            get {
                return null;
            }
        }

        #endregion

        #region Public Methods

        public void Show(MainForm form, TextEditorControl editor, 
            Action<string, bool, bool> searchOptionsUpdate, bool replaceMode)
        {
            _replaceMode        = replaceMode;
            _searchOptonsUpdate = searchOptionsUpdate;

            this.Editor         = editor;
            this.MainForm       = form;

            _searchHandler.ClearScanRegion();

            string selectedWord = null;

            var sm = editor.ActiveTextAreaControl.SelectionManager;
            if (sm.HasSomethingSelected && sm.SelectionCollection.Count == 1)
            {
                var sel = sm.SelectionCollection[0];
                if (sel.StartPosition.Line == sel.EndPosition.Line)
                {
                    selectedWord = sm.SelectedText;
                }
                else
                {
                    _searchHandler.SetScanRegion(sel);
                }
            }
            else
            {
                // Get the current word that the caret is on
                Caret caret  = editor.ActiveTextAreaControl.Caret;
                int start    = TextUtilities.FindWordStart(editor.Document, caret.Offset);
                int endAt    = TextUtilities.FindWordEnd(editor.Document, caret.Offset);
                selectedWord = editor.Document.GetText(start, endAt - start);
            }

            if (!string.IsNullOrWhiteSpace(selectedWord))
            {
                this.SearchText = selectedWord;
            }

            this.InitializeOptions();

            this.UpdateSelectionState();

            if (this.Owner == null)
            {
                this.Owner = form;
            }

            this.Show();
        }

        #endregion

        #region Protected Methods

        protected TextRange FindNext(bool viaF3, bool searchBackward, string messageIfNotFound)
        {
            if (string.IsNullOrEmpty(this.SearchText))
            {
                this.ShowMessage("Please enter the search text!");
                return null;
            }
            _lastSearchWasBackward     = searchBackward;
            _searchHandler.SearchText            = this.SearchText;
            _searchHandler.MatchCase          = _isMatchCase;
            _searchHandler.MatchWholeWordOnly = _isMatchWholeWord;

            var caret = _editor.ActiveTextAreaControl.Caret;
            if (viaF3 && _searchHandler.HasScanRegion && !caret.Offset.IsInRange(_searchHandler.BeginOffset, _searchHandler.EndOffset))
            {
                // user moved outside of the originally selected region
                _searchHandler.ClearScanRegion();
                UpdateSelectionState();
            }

            int startFrom = caret.Offset - (searchBackward ? 1 : 0);
            TextRange range = _searchHandler.FindNext(startFrom, searchBackward, out _lastSearchLoopedAround);
            if (range != null)
            {
                _searchCount++;

                SelectResult(range);
                if (_searchCount == 1)
                {
                    this.HighlightAll();
                }
            }
            else if (messageIfNotFound != null)
            {
                this.ShowMessage(messageIfNotFound);
            }
            return range;
        }

        protected void ShowMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }
            if (_popupHandler == null || _popupHandler.IsDisposed)
            {
                _popupHandler = new PopupMessageContainer();
            }

            _popupHandler.Message = message;
            Point pt = this.Location;
            pt.Offset(0, this.Height + 1);
            _popupHandler.Show(pt);
        }

        protected virtual void InitializeOptions()
        {
            if (_panelIsHidden)
            {
                var searchBox = this.SearchBox;
                if (searchBox != null)
                {
                    searchBox.SelectAll();
                    searchBox.Focus();
                }
                else
                {
                    if (_editor != null)
                    {
                        _editor.Focus();
                    }
                }

                this.HighlightAll();
            }

            _isInitialized = true;
            _searchCount   = 0;
        }

        protected void InsertText(string text)
        {
            var textArea = _editor.ActiveTextAreaControl.TextArea;
            textArea.Document.UndoStack.StartUndoGroup();
            try
            {
                if (textArea.SelectionManager.HasSomethingSelected)
                {
                    textArea.Caret.Position = textArea.SelectionManager.SelectionCollection[0].StartPosition;
                    textArea.SelectionManager.RemoveSelectedText();
                }
                textArea.InsertString(text);
            }
            finally
            {
                textArea.Document.UndoStack.EndUndoGroup();
            }
        }

        protected virtual void UpdateSelectionState()
        {
        }

        #endregion

        #region Protected Events-Related Methods

        protected virtual void OnFormLoad(object sender, EventArgs e)
        {
            var targetForm = this.MainForm;
            if (targetForm != null)
            {
                targetForm.LocationChanged += OnTargetWinMove;
                targetForm.Resize += OnTargetWinResize;
            }
        }

        protected virtual void OnFormShown(object sender, EventArgs e)
        {
            if (_editor != null)
            {
                var posRect = _editor.RectangleToScreen(_editor.ClientRectangle);
                var winSize = this.Size;

                this.Left = posRect.Right - winSize.Width - SearchGlobals.Offset * 4;
                this.Top = posRect.Top + SearchGlobals.Offset;
            }

            var searchBox = this.SearchBox;
            if (searchBox != null)
            {
                searchBox.SelectAll();
                searchBox.Focus();
            }
            else
            {
                if (_editor != null)
                {
                    _editor.Focus();
                }
            }

            this.HighlightAll();

            _isInitialized = true;
        }

        protected virtual void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            _isInitialized = false;
        }

        protected virtual void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            _searchOptonsUpdate?.Invoke(this.SearchText, _isMatchCase, _isMatchWholeWord);

            if (_highlightGroups.ContainsKey(_editor))
            {
                HighlightGroup group = _highlightGroups[_editor];
                if (group.HasMarkers)
                {
                    // Clear highlights
                    group.ClearMarkers();
                    _editor.Refresh();
                }
            }

            // Prevent dispose, as this form can be re-used
            if (e.CloseReason != CloseReason.FormOwnerClosing)
            {
                if (this.Owner != null)
                    this.Owner.Select(); // prevent another app from being activated instead

                _panelIsHidden = true;

                e.Cancel = true;
                this.Hide();

                // Discard search region
                _searchHandler.ClearScanRegion();
                _editor.Refresh(); // must repaint manually
            }
        }

        protected void OnTargetWinResize(object sender, EventArgs e)
        {
            this.Reposition();
        }

        protected void OnTargetWinMove(object sender, EventArgs e)
        {
            this.Reposition();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == SearchGlobals.WM_NCHITTEST)
            {
                m.Result = (IntPtr)(SearchGlobals.HT_CAPTION);
            }
        }

        #endregion

        #region Private Methods

        private void HighlightAll()
        {
            if (this.DesignMode)
            {
                return;
            }

            if (!_highlightGroups.ContainsKey(_editor))
                _highlightGroups[_editor] = new HighlightGroup(_editor);

            HighlightGroup group = _highlightGroups[_editor];

            if (!string.IsNullOrWhiteSpace(this.SearchText))
            {
                _searchHandler.SearchText            = this.SearchText;
                _searchHandler.MatchCase          = _isMatchCase;
                _searchHandler.MatchWholeWordOnly = _isMatchWholeWord;

                bool looped = false;
                int offset = 0, count = 0;
                for (; ; )
                {
                    TextRange range = _searchHandler.FindNext(offset, false, out looped);
                    if (range == null || looped)
                        break;
                    offset = range.Offset + range.Length;
                    count++;

                    var m = new TextMarker(range.Offset, range.Length,
                            TextMarkerType.SolidBlock, Color.LightGoldenrodYellow, Color.Black);
                    group.AddMarker(m);
                }

                if (group.HasMarkers)
                {
                    _editor.Refresh();
                }
            }
            else
            {
                if (_searchHandler.HasScanRegion || group.HasMarkers)
                {
                    // Clear highlights
                    group.ClearMarkers();
                }
            }
        }

        private void Reposition()
        {
            var targetControl = this.Editor;

            if (targetControl == null)
            {
                return;
            }

            var posLoc = targetControl.PointToScreen(targetControl.Location);

            var posRect = targetControl.RectangleToScreen(targetControl.ClientRectangle);
            var winSize = this.Size;

            this.Left = posRect.Right - winSize.Width - SearchGlobals.Offset * 4;
            this.Top = posRect.Top + SearchGlobals.Offset;
        }

        private void SelectResult(TextRange range)
        {
            TextLocation p1 = _editor.Document.OffsetToPosition(range.Offset);
            TextLocation p2 = _editor.Document.OffsetToPosition(range.Offset + range.Length);
            _editor.ActiveTextAreaControl.SelectionManager.SetSelection(p1, p2);
            _editor.ActiveTextAreaControl.ScrollTo(p1.Line, p1.Column);
            // Also move the caret to the end of the selection, because when the user 
            // presses F3, the caret is where we start searching next time.
            _editor.ActiveTextAreaControl.Caret.Position =
                _editor.Document.OffsetToPosition(range.Offset + range.Length);
        }

        #endregion
    }

    public sealed class TextRange : AbstractSegment
    {
        private IDocument _document;

        public TextRange(IDocument document, int offset, int length)
        {
            _document = document;
            this.offset = offset;
            this.length = length;
        }

        public IDocument Document
        {
            get {
                return _document;
            }
        }
    }

    /// <summary>This class finds occurrances of a search string in a text 
    /// editor's IDocument... it's like Find box without a GUI.</summary>
    public sealed class TextEditorSearcher : IDisposable
    {
        private bool _isDisposed;
        // I would have used the TextAnchor class to represent the beginning and 
        // end of the region to scan while automatically adjusting to changes in 
        // the document--but for some reason it is sealed and its constructor is 
        // internal. Instead I use a TextMarker, which is perhaps even better as 
        // it gives me the opportunity to highlight the region. Note that all the 
        // markers and coloring information is associated with the text document, 
        // not the editor control, so TextEditorSearcher doesn't need a reference 
        // to the TextEditorControl. After adding the marker to the document, we
        // must remember to remove it when it is no longer needed.
        private TextMarker _region;
        private IDocument _document;

        private string _lookFor;
        private string _lookFor2; // uppercase in case-insensitive mode

        public TextEditorSearcher()
        {
        }

        ~TextEditorSearcher()
        {
            this.Dispose(false);
        }

        public bool IsDisposed
        {
            get {
                return _isDisposed;
            }
        }

        public IDocument Document
        {
            get { return _document; }
            set {
                if (_document != value)
                {
                    ClearScanRegion();
                    _document = value;
                }
            }
        }

        public bool HasScanRegion
        {
            get {
                return _region != null;
            }
        }

        /// <summary>Begins the start offset for searching</summary>
        public int BeginOffset
        {
            get {
                if (_region != null)
                    return _region.Offset;

                return 0;
            }
        }

        /// <summary>Begins the end offset for searching</summary>
        public int EndOffset
        {
            get {
                if (_region != null)
                    return _region.EndOffset;

                return _document.TextLength;
            }
        }

        public bool MatchCase { get; set; }

        public bool MatchWholeWordOnly { get; set; }

        public string SearchText
        {
            get { return _lookFor; }
            set { _lookFor = value; }
        }

        /// <summary>Sets the region to search. The region is updated 
        /// automatically as the document changes.</summary>
        public void SetScanRegion(ISelection sel)
        {
            SetScanRegion(sel.Offset, sel.Length);
        }

        /// <summary>Sets the region to search. The region is updated 
        /// automatically as the document changes.</summary>
        public void SetScanRegion(int offset, int length)
        {
            var bkgColor = _document.HighlightingStrategy.GetColorFor("Default").BackgroundColor;
            _region = new TextMarker(offset, length, TextMarkerType.SolidBlock,
                bkgColor.HalfMix(Color.FromArgb(160, 160, 160)));

            _document.MarkerStrategy.AddMarker(_region);
        }

        public void ClearScanRegion()
        {
            if (_region != null)
            {
                _document.MarkerStrategy.RemoveMarker(_region);
                _region = null;
            }
        }

        /// <summary>Finds next instance of LookFor, according to the search rules 
        /// (MatchCase, MatchWholeWordOnly).</summary>
        /// <param name="beginAtOffset">Offset in Document at which to begin the search</param>
        /// <remarks>If there is a match at beginAtOffset precisely, it will be returned.</remarks>
        /// <returns>Region of document that matches the search string</returns>
        public TextRange FindNext(int beginAtOffset, bool searchBackward, out bool loopedAround)
        {
            Debug.Assert(!string.IsNullOrEmpty(_lookFor));
            loopedAround = false;

            int startAt = BeginOffset, endAt = EndOffset;
            int curOffs = beginAtOffset.InRange(startAt, endAt);

            _lookFor2 = MatchCase ? _lookFor : _lookFor.ToUpperInvariant();

            TextRange result;
            if (searchBackward)
            {
                result = FindNextIn(startAt, curOffs, true);
                if (result == null)
                {
                    loopedAround = true;
                    result = FindNextIn(curOffs, endAt, true);
                }
            }
            else
            {
                result = FindNextIn(curOffs, endAt, false);
                if (result == null)
                {
                    loopedAround = true;
                    result = FindNextIn(startAt, curOffs, false);
                }
            }
            return result;
        }

        private TextRange FindNextIn(int offset1, int offset2, bool searchBackward)
        {
            Debug.Assert(offset2 >= offset1);
            offset2 -= _lookFor.Length;

            // Make behavior decisions before starting search loop
            Func<char, char, bool> matchFirstCh;
            Func<int, bool> matchWord;
            if (MatchCase)
                matchFirstCh = (lookFor, c) => (lookFor == c);
            else
                matchFirstCh = (lookFor, c) => (lookFor == char.ToUpperInvariant(c));
            if (MatchWholeWordOnly)
                matchWord = IsWholeWordMatch;
            else
                matchWord = IsPartWordMatch;

            // Search
            char lookForCh = _lookFor2[0];
            if (searchBackward)
            {
                for (int offset = offset2; offset >= offset1; offset--)
                {
                    if (matchFirstCh(lookForCh, _document.GetCharAt(offset))
                        && matchWord(offset))
                        return new TextRange(_document, offset, _lookFor.Length);
                }
            }
            else
            {
                for (int offset = offset1; offset <= offset2; offset++)
                {
                    if (matchFirstCh(lookForCh, _document.GetCharAt(offset))
                        && matchWord(offset))
                        return new TextRange(_document, offset, _lookFor.Length);
                }
            }
            return null;
        }

        private bool IsWholeWordMatch(int offset)
        {
            if (IsWordBoundary(offset) && IsWordBoundary(offset + _lookFor.Length))
                return IsPartWordMatch(offset);

            return false;
        }

        private bool IsWordBoundary(int offset)
        {
            return offset <= 0 || offset >= _document.TextLength ||
                !IsAlphaNumeric(offset - 1) || !IsAlphaNumeric(offset);
        }

        private bool IsAlphaNumeric(int offset)
        {
            char c = _document.GetCharAt(offset);
            return char.IsLetterOrDigit(c) || c == '_';
        }

        private bool IsPartWordMatch(int offset)
        {
            string substr = _document.GetText(offset, _lookFor.Length);
            if (!MatchCase)
                substr = substr.ToUpperInvariant();
            return substr == _lookFor2;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            ClearScanRegion();

            _isDisposed = true;
        }
    }

    /// <summary>Bundles a group of markers together so that they can be cleared 
    /// together.</summary>
    public sealed class HighlightGroup : IDisposable
    {
        private bool _isDisposed;
        private List<TextMarker> _markers;
        private TextEditorControl _editor;
        private IDocument _document;

        public HighlightGroup(TextEditorControl editor)
        {
            _markers  = new List<TextMarker>();
            _editor   = editor;
            _document = editor.Document;
        }
        ~HighlightGroup()
        {
            this.Dispose(false);
        }

        public bool IsDisposed
        {
            get {
                return _isDisposed;
            }
        }

        public bool HasMarkers
        {
            get {
                return (_markers.Count != 0);
            }
        }

        public IList<TextMarker> Markers
        {
            get {
                return _markers.AsReadOnly();
            }
        }

        public void AddMarker(TextMarker marker)
        {
            _markers.Add(marker);
            _document.MarkerStrategy.AddMarker(marker);
        }

        public void ClearMarkers()
        {
            foreach (TextMarker m in _markers)
            {
                _document.MarkerStrategy.RemoveMarker(m);
            }
            _markers.Clear();
            _editor.Refresh();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            ClearMarkers();

            if (disposing)
            {
                _markers = null;
            }

            _isDisposed = false;
        }
    }

}
