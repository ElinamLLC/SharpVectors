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
        private IList<SvgPathSeg> _segments;

        #endregion

        #region Constructors

        public SvgPathSegList(string d, bool readOnly)
        {
            _segments = new List<SvgPathSeg>();

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
                SvgPathSegHandler hardler = new SvgPathSegHandler(this);
                SvgPathParser parser = new SvgPathParser(hardler);

                if (parser.Parse(d))
                {
                    _isClosed      = hardler.IsClosed;
                    _mayHaveCurves = hardler.MayHaveCurves;
                }
                //else if (_segments.Count != 0)
                //{
                //    _segments = new List<SvgPathSeg>();
                //}

                //SvgPathSegParser parser = new SvgPathSegParser();
                //if (parser.Parse(this, d))
                //{
                //    _isClosed      = parser.IsClosed;
                //    _mayHaveCurves = parser.MayHaveCurves;
                //}
                //else if (_segments.Count != 0)
                //{
                //    _segments = new List<SvgPathSeg>();
                //}
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
                _segments = new List<SvgPathSeg>();
            }
            else
            {
                _segments = new List<SvgPathSeg>(pathList.NumberOfItems);
                for (int i = 0; i < pathList.NumberOfItems; i++)
                {
                    _segments.Add((SvgPathSeg)pathList.GetItem(i));
                }

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
            //            int index = _segments.IndexOf(seg);
            int index = seg.Index;
            if (index == -1)
            {
                throw new Exception("Path segment not part of this list");
            }
            if (index == 0)
            {
                return null;
            }
            return GetItem(index - 1);
        }

        public SvgPathSeg GetNextSegment(SvgPathSeg seg)
        {
            //            int index = _segments.IndexOf(seg);
            int index = seg.Index;
            if (index == -1)
            {
                throw new Exception("Path segment not part of this list");
            }
            if (index == _segments.Count - 1)
            {
                return null;
            }
            return this[index + 1];
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

        public SvgPathSeg Initialize(SvgPathSeg newItem)
        {
            Clear();

            return AppendItem(newItem);
        }

        public SvgPathSeg GetItem(int index)
        {
            if (index < 0 || index >= NumberOfItems)
            {
                throw new DomException(DomExceptionType.IndexSizeErr);
            }

            return _segments[index];
        }

        public SvgPathSeg this[int index]
        {
            get {
                return this.GetItem(index);
            }
            set {
                this.ReplaceItem(value, index);
            }
        }

        public SvgPathSeg InsertItemBefore(SvgPathSeg newItem, int index)
        {
            if (_readOnly)
            {
                throw new DomException(DomExceptionType.NoModificationAllowedErr);
            }
            _segments.Insert(index, newItem);
            SetListAndIndex(newItem, index);
            ChangeIndexes(index + 1, 1);

            return newItem;
        }

        public ISvgPathSeg ReplaceItem(SvgPathSeg newItem, int index)
        {
            if (_readOnly)
            {
                throw new DomException(DomExceptionType.NoModificationAllowedErr);
            }
            SvgPathSeg replacedItem = this.GetItem(index);
            _segments[index] = newItem;
            SetListAndIndex(newItem, index);

            return replacedItem;
        }

        public SvgPathSeg RemoveItem(int index)
        {
            if (_readOnly)
            {
                throw new DomException(DomExceptionType.NoModificationAllowedErr);
            }
            SvgPathSeg result = GetItem(index);
            _segments.RemoveAt(index);
            ChangeIndexes(index, -1);

            return result;
        }

        public SvgPathSeg AppendItem(SvgPathSeg newItem)
        {
            if (_readOnly)
            {
                throw new DomException(DomExceptionType.NoModificationAllowedErr);
            }
            _segments.Add(newItem);
            SetListAndIndex(newItem, _segments.Count - 1);

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

        public void Add(SvgPathSeg item)
        {
            this.AppendItem(item);
        }

        public bool Contains(SvgPathSeg item)
        {
            return this._segments.Contains(item);
        }

        public void CopyTo(SvgPathSeg[] array, int arrayIndex)
        {
            this._segments.CopyTo(array, arrayIndex);
        }

        public IEnumerator<SvgPathSeg> GetEnumerator()
        {
            return this._segments.GetEnumerator();
        }

        public int IndexOf(SvgPathSeg item)
        {
            return this._segments.IndexOf(item);
        }

        public void Insert(int index, SvgPathSeg item)
        {
            this.ReplaceItem(item, index);
        }

        public bool Remove(SvgPathSeg item)
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

        #region ISvgPathSegList Members

        ISvgPathSeg IList<ISvgPathSeg>.this[int index]
        {
            get {
                return _segments[index];
            }
            set {
                _segments[index] = (SvgPathSeg)value;
            }
        }

        ISvgPathSeg ISvgPathSegList.Initialize(ISvgPathSeg newItem)
        {
            return this.Initialize((SvgPathSeg)newItem);
        }

        ISvgPathSeg ISvgPathSegList.GetItem(int index)
        {
            return _segments[index];
        }

        ISvgPathSeg ISvgPathSegList.InsertItemBefore(ISvgPathSeg newItem, int index)
        {
            return this.InsertItemBefore((SvgPathSeg)newItem, index);
        }

        ISvgPathSeg ISvgPathSegList.RemoveItem(int index)
        {
            return this.RemoveItem(index);
        }

        ISvgPathSeg ISvgPathSegList.ReplaceItem(ISvgPathSeg newItem, int index)
        {
            return this.ReplaceItem((SvgPathSeg)newItem, index);
        }

        ISvgPathSeg ISvgPathSegList.AppendItem(ISvgPathSeg newItem)
        {
            return this.AppendItem((SvgPathSeg)newItem);
        }

        int IList<ISvgPathSeg>.IndexOf(ISvgPathSeg item)
        {
            return this.IndexOf((SvgPathSeg)item);
        }

        void IList<ISvgPathSeg>.Insert(int index, ISvgPathSeg item)
        {
            this.Insert(index, (SvgPathSeg)item);
        }

        void ICollection<ISvgPathSeg>.Add(ISvgPathSeg item)
        {
            this.Add((SvgPathSeg)item);
        }

        bool ICollection<ISvgPathSeg>.Contains(ISvgPathSeg item)
        {
            return this.Contains((SvgPathSeg)item);
        }

        void ICollection<ISvgPathSeg>.CopyTo(ISvgPathSeg[] array, int arrayIndex)
        {
            this.CopyTo((SvgPathSeg[])array, arrayIndex);
        }

        bool ICollection<ISvgPathSeg>.Remove(ISvgPathSeg item)
        {
            return this.Remove((SvgPathSeg)item);
        }

        IEnumerator<ISvgPathSeg> IEnumerable<ISvgPathSeg>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Private Methods

        private void SetListAndIndex(SvgPathSeg newItem, int index)
        {
            if (newItem == null)
            {
                throw new SvgException(SvgExceptionType.SvgWrongTypeErr,
                    "Can only add SvgPathSeg subclasses to ISvgPathSegList");
            }

            newItem._list = this;
            newItem._index = index;

            //newItem.SetList(this);
            //newItem.SetIndex(index);
        }

        private void ChangeIndexes(int startAt, int diff)
        {
            int count = _segments.Count;
            for (int i = startAt; i < count; i++)
            {
                SvgPathSeg seg = _segments[i];
                if (seg != null)
                {
                    seg.SetIndexWithDiff(diff);
                }
            }
        }

        #endregion
    }
}
