using System;

namespace SharpVectors.Dom.Svg
{
    public interface ISvgMarker
    {
        int Index { get; }

        bool IsCurve { get; }

        SvgPointF Position { get; }

        ISvgPathSeg Segment { get; }
    }

    public sealed class SvgMarker : ISvgMarker
    {
        private readonly int _index;
        private readonly SvgPointF _position;
        private readonly ISvgPathSeg _segment;

        public SvgMarker(int index, ISvgPathSeg segment)
        {
            _index    = index;
            _segment  = segment;
            if (segment != null)
            {
                _position = segment.AbsXY;
            }
        }

        public SvgMarker(int index, SvgPointF position)
        {
            _index    = index;
            _position = position;
        }

        public SvgMarker(int index, SvgPointF position, ISvgPathSeg segment)
        {
            _index    = index;
            _position = position;
            _segment  = segment;
        }

        public int Index
        {
            get {
                return _index;
            }
        }

        public SvgPointF Position
        {
            get {
                return _position;
            }
        }

        public ISvgPathSeg Segment
        {
            get {
                return _segment;
            }
        }

        public bool IsCurve
        {
            get {
                if (_segment == null)
                {
                    return false;
                }
                return _segment.IsCurve;
            }
        }
    }
}
