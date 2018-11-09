using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace SharpVectors.Dom.Svg
{
    // TODO: should we check that the list starts with a M/m since that's required by the spec?
    public class SvgPathSegList : ISvgPathSegList
    {
        #region Private Fields

        private bool _isClosed;
        private bool _mayHaveCurves;

        private string _pathScript;
        private bool _readOnly;
        private IList<ISvgPathSeg> _segments;

        #endregion

        #region Constructors

        public SvgPathSegList(string d, bool readOnly)
        {
            _segments = new List<ISvgPathSeg>();

            if (d == null)
            {
                d = string.Empty;
            }

            _isClosed      = false;
            _mayHaveCurves = false;

            _pathScript    = d;
            _readOnly      = readOnly;

            if (!string.IsNullOrWhiteSpace(d))
            {
                SvgPathSegParser parser = new SvgPathSegParser();
                if (parser.Parse(this, d))
                {
                    _isClosed      = parser.IsClosed;
                    _mayHaveCurves = parser.MayHaveCurves;
                }
            }
        }

        public SvgPathSegList(ISvgPathSegList pathList)
        {
            _pathScript    = string.Empty;
            _readOnly      = false;

            _isClosed      = false;
            _mayHaveCurves = false;

            if (pathList == null)
            {
                _segments = new List<ISvgPathSeg>();
            }
            else
            {
                _segments = new List<ISvgPathSeg>(pathList);

                var segList = pathList as SvgPathSegList;

                if (segList != null)
                {
                    _pathScript    = segList.PathScript;
                    _readOnly      = segList.IsReadOnly;
                    _isClosed      = segList.IsClosed;
                    _mayHaveCurves = segList.MayHaveCurves;
                }
            }
        }

        #endregion

        #region Public Properties

        public string PathScript
        {
            get {
                return _pathScript;
            }
        }

        public bool IsClosed
        {
            get {
                return _isClosed;
            }
        }

        public bool MayHaveCurves
        {
            get {
                return _mayHaveCurves;
            }
        }

        public SvgPointF[] Points
        {
            get {
                List<SvgPointF> ret = new List<SvgPointF>();
                foreach (SvgPathSeg seg in _segments)
                {
                    ret.Add(seg.AbsXY);
                }

                return ret.ToArray();
            }
        }

        public string PathText
        {
            get {
                return this.ToString();
            }
        }

        #endregion

        #region Public members

        public SvgPathSeg GetPreviousSegment(SvgPathSeg seg)
        {
            int index = _segments.IndexOf(seg);
            if (index == -1)
            {
                throw new Exception("Path segment not part of this list");
            }
            if (index == 0)
            {
                return null;
            }
            return (SvgPathSeg)GetItem(index - 1);
        }

        public SvgPathSeg GetNextSegment(SvgPathSeg seg)
        {
            int index = _segments.IndexOf(seg);
            if (index == -1)
            {
                throw new Exception("Path segment not part of this list");
            }
            if (index == _segments.Count - 1)
            {
                return null;
            }
            return (SvgPathSeg)this[index + 1];
        }

        public double GetStartAngle(int index)
        {
            return this[index].StartAngle;
        }

        public double GetEndAngle(int index)
        {
            return this[index].EndAngle;
        }

        public double GetTotalLength()
        {
            double result = 0;
            foreach (SvgPathSeg segment in _segments)
            {
                result += segment.Length;
            }
            return result;
        }

        public int GetPathSegAtLength(double distance)
        {
            double result = 0;
            foreach (SvgPathSeg segment in _segments)
            {
                result += segment.Length;
                if (result > distance)
                {
                    return segment.Index;
                }
            }
            // distance was to big, return last item index
            // TODO: is this correct?
            return NumberOfItems - 1;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (SvgPathSeg seg in _segments)
            {
                sb.Append(seg.PathText);
            }
            return sb.ToString();
        }

        #endregion

        #region ISvgPathSegList Members

        public int NumberOfItems
        {
            get {
                return _segments.Count;
            }
        }

        public void Clear()
        {
            if (_readOnly)
            {
                throw new DomException(DomExceptionType.NoModificationAllowedErr);
            }
            _segments.Clear();
        }

        public ISvgPathSeg Initialize(ISvgPathSeg newItem)
        {
            Clear();

            return AppendItem(newItem);
        }

        public ISvgPathSeg GetItem(int index)
        {
            if (index < 0 || index >= NumberOfItems)
            {
                throw new DomException(DomExceptionType.IndexSizeErr);
            }

            return _segments[index];
        }

        public ISvgPathSeg this[int index]
        {
            get {
                return GetItem(index);
            }
            set {
                this.ReplaceItem(value, index);
            }
        }

        public ISvgPathSeg InsertItemBefore(ISvgPathSeg newItem, int index)
        {
            if (_readOnly)
            {
                throw new DomException(DomExceptionType.NoModificationAllowedErr);
            }
            _segments.Insert(index, newItem);
            setListAndIndex(newItem as SvgPathSeg, index);
            changeIndexes(index + 1, 1);

            return newItem;
        }

        public ISvgPathSeg ReplaceItem(ISvgPathSeg newItem, int index)
        {
            if (_readOnly)
            {
                throw new DomException(DomExceptionType.NoModificationAllowedErr);
            }
            ISvgPathSeg replacedItem = GetItem(index);
            _segments[index] = newItem;
            setListAndIndex(newItem as SvgPathSeg, index);

            return replacedItem;
        }

        public ISvgPathSeg RemoveItem(int index)
        {
            if (_readOnly)
            {
                throw new DomException(DomExceptionType.NoModificationAllowedErr);
            }
            ISvgPathSeg result = GetItem(index);
            _segments.RemoveAt(index);
            changeIndexes(index, -1);

            return result;
        }

        public ISvgPathSeg AppendItem(ISvgPathSeg newItem)
        {
            if (_readOnly)
            {
                throw new DomException(DomExceptionType.NoModificationAllowedErr);
            }
            _segments.Add(newItem);
            setListAndIndex(newItem as SvgPathSeg, _segments.Count - 1);

            return newItem;
        }

        #endregion

        #region IList<ISvgPathSeg> Members

        public int Count
        {
            get {
                return this._segments.Count;
            }
        }

        public bool IsReadOnly
        {
            get {
                return this._segments.IsReadOnly;
            }
        }

        public void Add(ISvgPathSeg item)
        {
            this.AppendItem(item);
        }

        public bool Contains(ISvgPathSeg item)
        {
            return this._segments.Contains(item);
        }

        public void CopyTo(ISvgPathSeg[] array, int arrayIndex)
        {
            this._segments.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ISvgPathSeg> GetEnumerator()
        {
            return this._segments.GetEnumerator();
        }

        public int IndexOf(ISvgPathSeg item)
        {
            return this._segments.IndexOf(item);
        }

        public void Insert(int index, ISvgPathSeg item)
        {
            this.ReplaceItem(item, index);
        }

        public bool Remove(ISvgPathSeg item)
        {
            return this._segments.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.RemoveItem(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._segments.GetEnumerator();
        }

        #endregion

        #region Private Methods

        private void setListAndIndex(SvgPathSeg newItem, int index)
        {
            if (newItem != null)
            {
                newItem.SetList(this);
                newItem.SetIndex(index);
            }
            else
            {
                throw new SvgException(SvgExceptionType.SvgWrongTypeErr,
                    "Can only add SvgPathSeg subclasses to ISvgPathSegList");
            }
        }

        private void changeIndexes(int startAt, int diff)
        {
            int count = _segments.Count;
            for (int i = startAt; i < count; i++)
            {
                SvgPathSeg seg = _segments[i] as SvgPathSeg;
                if (seg != null)
                {
                    seg.SetIndexWithDiff(diff);
                }
            }
        }

        #endregion
    }
}
