using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GdiW3cSvgTestSuite
{
    public partial class ReplaceForm : SearchReplaceForm
    {
        private bool _isUpdating;

        public ReplaceForm()
        {
            InitializeComponent();
        }

        public override TextBox SearchBox
        {
            get {
                return txtSearch;
            }
        }

        protected override void UpdateSelectionState()
        {
            if (_isUpdating)
            {
                return;
            }
            _isUpdating = true;
            try
            {
                if (_searchHandler.HasScanRegion)
                {
                    chkSelection.Checked = true;
                }
                else
                {
                    chkSelection.Checked = false;
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }

        protected override void InitializeOptions()
        {
            chkMatchCase.Checked      = _isMatchCase;
            chkMatchWholeWord.Checked = _isMatchWholeWord;

            _isUpdating = false;

            base.InitializeOptions();
        }

        private void OnClickNext(object sender, EventArgs e)
        {
            FindNext(false, false, SearchGlobals.InfoTextNotFound);
        }

        private void OnClickPrevious(object sender, EventArgs e)
        {
            FindNext(false, true, SearchGlobals.InfoTextNotFound);
        }

        private void OnClickReplace(object sender, EventArgs e)
        {
            var sm = _editor.ActiveTextAreaControl.SelectionManager;
            if (string.Equals(sm.SelectedText, txtSearch.Text, StringComparison.OrdinalIgnoreCase))
            {
                InsertText(txtReplace.Text);
            }
            FindNext(false, _lastSearchWasBackward, SearchGlobals.InfoTextNotFound);
        }

        private async void OnClickReplaceAll(object sender, EventArgs e)
        {
            int count = 0;
            // BUG FIX: if the replacement string contains the original search string
            // (e.g. replace "red" with "very red") we must avoid looping around and
            // replacing forever! To fix, start replacing at beginning of region (by 
            // moving the caret) and stop as soon as we loop around.
            _editor.ActiveTextAreaControl.Caret.Position = _editor.Document.OffsetToPosition(_searchHandler.BeginOffset);

            _editor.Document.UndoStack.StartUndoGroup();
            try
            {
                while (FindNext(false, false, null) != null)
                {
                    if (_lastSearchLoopedAround)
                    {
                        break;
                    }

                    // Replace
                    count++;
                    this.InsertText(txtReplace.Text);
                }
            }
            finally
            {
                _editor.Document.UndoStack.EndUndoGroup();
            }
            if (count == 0)
            {
                this.ShowMessage(SearchGlobals.TargetNotFound);
            }
            else
            {
                this.ShowMessage(string.Format(SearchGlobals.TargetFound, count));

#if DOTNET40
                await TaskEx.Delay(5000).ContinueWith(t =>
                {
                    this.Close();
                });
#else
                await Task.Delay(5000).ContinueWith(t =>
                {
                    this.Close();
                });
#endif
            }
        }

        private void OnMatchCaseChanged(object sender, EventArgs e)
        {
            _isMatchCase = chkMatchCase.Checked;
        }

        private void OnMatchWholeWordChanged(object sender, EventArgs e)
        {
            _isMatchWholeWord = chkMatchWholeWord.Checked;
        }

        private void OnSearchTextChanged(object sender, EventArgs e)
        {
            if (_isInitialized)
            {
                _searchCount = 0;

                if (_highlightGroups.Count != 0 && _highlightGroups.ContainsKey(_editor))
                {
                    HighlightGroup group = _highlightGroups[_editor];
                    if (group.HasMarkers)
                    {
                        // Clear highlights
                        group.ClearMarkers();
                        _editor.Refresh();
                    }
                }
            }
        }

        private void OnClickClose(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnSelectionOnlyChanged(object sender, EventArgs e)
        {
            if (_isUpdating)
            {
                return;
            }

            if (chkSelection.Checked)
            {
                if (_searchHandler.HasScanRegion == false)
                {
                    var sm = _editor.ActiveTextAreaControl.SelectionManager;
                    if (sm.HasSomethingSelected && sm.SelectionCollection.Count == 1)
                    {
                        var sel = sm.SelectionCollection[0];
                        if (sel.StartPosition.Line != sel.EndPosition.Line)
                        {
                            _searchHandler.SetScanRegion(sel);
                        }
                    }

                    if (_searchHandler.HasScanRegion == false)
                    {
                        try
                        {
                            _isUpdating = true;
                            chkSelection.Checked = false;
                        }
                        finally
                        {
                            _isUpdating = false;
                        }
                    }
                }
            }
            else
            {
                _searchHandler.ClearScanRegion();
            }
        }
    }
}
