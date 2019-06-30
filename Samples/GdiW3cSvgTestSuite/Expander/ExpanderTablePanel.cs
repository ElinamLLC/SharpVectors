//-------------------------------------------------------------------------------- 
// Original source: https://github.com/mkoertgen/winforms.expander
// Modified for the Windows Forms test suite application
//-------------------------------------------------------------------------------- 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsExpander
{
    public class ExpanderTablePanel : TableLayoutPanel
    {
        // initially copied from http://stackoverflow.com/a/31086890/2592915
        // then mainly reworked

        private int _sizingRow = -1;
        private int _currentRow = -1;
        private Point _mdown = Point.Empty;
        private int _oldHeight = -1;
        private bool _isNormal;
        private readonly List<RectangleF> _splitterRects = new List<RectangleF>();
        private int[] _rowHeights = new int[0];
        private int _scaleY = 1;

        [Category("Appearance"), DefaultValue(typeof(int), "6")]
        public int SplitterSize { get; set; }

        [Category("Appearance"), DefaultValue(typeof(int), "28")]
        public int MinRowHeight { get; set; }


        public ExpanderTablePanel()
        {
            MouseDown += SplitTablePanel_MouseDown;
            MouseMove += SplitTablePanel_MouseMove;
            MouseUp += SplitTablePanel_MouseUp;
            MouseLeave += SplitTablePanel_MouseLeave;
            SplitterSize = 6;
            MinRowHeight = 28;

            foreach (var expander in Controls.OfType<Expander>())
                expander.ExpandedChanged += Expander_ExpandedChanged;

            SetCanToggle();
            UpdateMinSize();
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            var expander = e.Control as Expander;
            if (expander != null)
                expander.ExpandedChanged += Expander_ExpandedChanged;
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            var expander = e.Control as Expander;
            if (expander != null)
                expander.ExpandedChanged -= Expander_ExpandedChanged;
        }

        private void Expander_ExpandedChanged(object sender, EventArgs e)
        {
            var expander = sender as Expander;
            if (expander == null) return;

            var row = GetRow(expander);
            if (row < 0) return;

            var newHeight = expander.IsExpanded ? expander.ExpandedHeight : expander.CollapsedHeight;
            SizeRow(row, newHeight);

            SetCanToggle();
            UpdateMinSize();
        }

        private void SetCanToggle()
        {
            var expander = Controls.OfType<Expander>().ToList();
            expander.ForEach(e => e.CanToggle = true);
            var expanded = expander.Where(e => e.IsExpanded).ToList();
            //if (expanded.Count == 1) expanded.Single().CanToggle = false;
        }

        private void UpdateMinSize()
        {
            var controls = Controls.OfType<Control>().ToList();
            var minHeight = Math.Max(MinimumSize.Height, controls.Sum(c => c.MinimumSize.Height));
            var minWidth = Math.Max(MinimumSize.Width, controls.Sum(c => c.MinimumSize.Width));
            MinimumSize = new Size(minWidth, minHeight);
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            if (_rowHeights.Length == 0) return;

            var totalHeight = _rowHeights.Sum();
            var delta = ClientSize.Height - totalHeight;
            if (delta == 0) return;

            var rowIndices = Enumerable.Range(0, RowCount);
            if (delta > 0) rowIndices = rowIndices.Reverse();
            rowIndices = rowIndices.Where(i => !IsFixed(i)).ToList();

            int height = 0, row = -1;
            foreach(var r in rowIndices)
            { 
                // check if we can apply the delta on this row
                height = _rowHeights[r] + delta;
                int pairingRow;
                var coercedHeight = CoerceRowHeight(r, height, out pairingRow, false);
                if (coercedHeight != height) continue;
                row = r;
                break;
            }
            if (row == -1) return;

            SizeRow(row, height, false);
        }

        private void SplitTablePanel_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void SplitTablePanel_MouseUp(object sender, MouseEventArgs e)
        {
            GetSplitterGetRectangles();
        }

        private void SplitTablePanel_MouseMove(object sender, MouseEventArgs e)
        {
            // one-time initialization, defer until we have valid _rowHeights
            if (!_isNormal) NomalizeRowStyles();

            if (_splitterRects.Count <= 0) GetSplitterGetRectangles();
            if (_rowHeights.Length <= 0) _rowHeights = GetRowHeights();

            if (e.Button == MouseButtons.Left)
            {
                if (_sizingRow < 0) return;
                var newHeight = _oldHeight + _scaleY * (e.Y - _mdown.Y);
                SizeRow(_sizingRow, newHeight);
            }
            else
            {
                _currentRow = -1;
                _scaleY = 1;
                for (var i = 0; i < _splitterRects.Count; i++)
                {
                    if (!_splitterRects[i].Contains(e.Location)) continue;
                    if (!IsFixed(i))
                    {
                        if (i == RowCount - 2 && IsFixed(i + 1, true)) break;
                        _currentRow = i;
                    }
                    else
                    {
                        // don't allow dragging i, if fixed and i=first/last row
                        if (i == 0 || i == RowCount - 1 || IsFixed(i+1, true)) break;
                        _currentRow = i + 1;
                        _scaleY = -1;
                    }
                    break;
                }
                Cursor = _currentRow >= 0 ? Cursors.SizeNS : Cursors.Default;
            }
        }

        private void SplitTablePanel_MouseDown(object sender, MouseEventArgs e)
        {
            _mdown = Point.Empty;
            _sizingRow = -1;
            if (_currentRow < 0) return;
            _sizingRow = _currentRow;
            _oldHeight = _rowHeights[_sizingRow];
            _mdown = e.Location;
        }

        private void GetSplitterGetRectangles()
        {   // get a list of mouse sensitive rectangles
            var sz = SplitterSize / 2f;
            var y = 0f;
            var w = ClientSize.Width;
            var rw = GetRowHeights();

            _splitterRects.Clear();
            for (var i = 0; i < rw.Length - 1; i++)
            {
                y += rw[i];
                _splitterRects.Add(new RectangleF(0, y - sz, w, SplitterSize));
            }
        }

        private void SizeRow(int row, int height, bool adjustPair = true)
        {   // change the height of one row
            if (height == 0) return;
            if (row < 0) return;
            _rowHeights = GetRowHeights();
            if (row >= _rowHeights.Length) return;

            int pairingRow;
            var newHeight = CoerceRowHeight(row, height, out pairingRow, adjustPair);
            if (newHeight == _rowHeights[row]) return; // no change

            if (adjustPair && row == pairingRow) // cannot change
                return;


            SuspendLayout();
            {
                RowStyles[row] = new RowStyle(SizeType.Absolute, newHeight);

                if (adjustPair)
                {
                    var deltaHeight = newHeight - _rowHeights[row];
                    var pairingHeight = _rowHeights[pairingRow] - deltaHeight;

                    RowStyles[pairingRow] = new RowStyle(SizeType.Absolute, pairingHeight);
                }
            }
            ResumeLayout();

            _rowHeights = GetRowHeights();
            GetSplitterGetRectangles();
        }

        private int CoerceRowHeight(int row, int height, out int pairingRow, bool checkPair = true)
        {
            pairingRow = row;
            if (height == _rowHeights[row]) return height;

            // coerce height by control min/max size (e.g. expander)
            var minHeight = MinRowHeight;
            var maxHeight = height;

            var c1 = GetControlFromPosition(0, row);
            // preserve c1 min/max size
            var c1MaxH = c1.MaximumSize.Height;
            if (c1MaxH > 0) maxHeight = Math.Min(maxHeight, c1MaxH - SplitterSize);
            var c1MinH = c1.MinimumSize.Height;
            if (c1MinH > 0) minHeight = Math.Max(minHeight, c1MinH + SplitterSize);

            var coercedHeight = Math.Max(Math.Min(height, maxHeight), minHeight);

            var deltaHeight = coercedHeight - _rowHeights[row];
            if (deltaHeight == 0) return coercedHeight;

            if (!checkPair) return coercedHeight;

            // coerce adjacent row
            pairingRow = GetPairingRow(row);

            if (row == pairingRow) // cannot change
                return _rowHeights[row];

            var c2 = GetControlFromPosition(0, pairingRow);
            var c2HeightToBe = _rowHeights[pairingRow] - deltaHeight;
            if (deltaHeight > 0) // c1 wants to grow
            {
                // preserve c2.minsize
                var c2MinH = c2.MinimumSize.Height;
                if (c2MinH > 0)
                {
                    c2MinH += SplitterSize;
                    if (c2HeightToBe < c2MinH)
                        coercedHeight -= (c2MinH - c2HeightToBe);
                }
            }
            else // c1 wants to shrink
            {
                // preserve c1.maxsize
                var c2MaxH = c2.MaximumSize.Height;
                if (c2MaxH > 0)
                {
                    c2MaxH += SplitterSize;
                    if (c2HeightToBe > c2MaxH)
                        coercedHeight += (c2HeightToBe - c2MaxH);
                }
            }

            return coercedHeight;
        }

        private int GetPairingRow(int row)
        {
            var dir = row == RowCount - 1 ? -1 : 1;
            var pairingRow = row + dir;
            if (pairingRow == -1) pairingRow = RowCount - 1;
            else if (pairingRow == RowCount) pairingRow = 0;
            while (IsFixed(pairingRow) && pairingRow != row)
            {
                pairingRow += dir;
                if (pairingRow == -1) pairingRow = RowCount - 1;
                else if (pairingRow == RowCount) pairingRow = 0;
            }
            return pairingRow;
        }

        private bool IsFixed(int row, bool checkToggle = false)
        {
            var control = GetControlFromPosition(0, row);
            if (control == null)
            {
                return false;
            }
            var expander = control as Expander;
            if (expander != null)
            {
                var isFixed = !expander.IsExpanded;
                if (checkToggle)
                    isFixed |= !expander.CanToggle;
                return isFixed;
            }

            return control.MinimumSize.Height > 0 && control.MaximumSize.Height > 0 &&
                   control.MinimumSize.Height == control.MaximumSize.Height;
        }

        private void NomalizeRowStyles()
        {
            // set all rows to absolute and the last one to percent=100!
            if (_rowHeights.Length <= 0) return;
            _rowHeights = GetRowHeights();
            RowStyles.Clear();
            for (var i = 0; i < RowCount - 1; i++)
            {
                var rowStyle = new RowStyle(SizeType.AutoSize);
                if (i < _rowHeights.Length)
                    rowStyle = new RowStyle(SizeType.Absolute, _rowHeights[i]);
                RowStyles.Add(rowStyle);
            }
            RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            _isNormal = true;
        }
    }
}
