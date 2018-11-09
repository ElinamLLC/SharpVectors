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

                switch (_segment.PathSegType)
                {
                    case SvgPathSegType.ArcAbs:
                        return true;
                    case SvgPathSegType.ArcRel:
                        return true;
                    case SvgPathSegType.ClosePath:
                        return true;
                    case SvgPathSegType.CurveToCubicAbs:
                        return true;
                    case SvgPathSegType.CurveToCubicRel:
                        return true;
                    case SvgPathSegType.CurveToCubicSmoothAbs:
                        return true;
                    case SvgPathSegType.CurveToCubicSmoothRel:
                        return true;
                    case SvgPathSegType.CurveToQuadraticAbs:
                        return true;
                    case SvgPathSegType.CurveToQuadraticRel:
                        return true;
                    case SvgPathSegType.CurveToQuadraticSmoothAbs:
                        return true;
                    case SvgPathSegType.CurveToQuadraticSmoothRel:
                        return true;

                    case SvgPathSegType.LineToAbs:
                        return false;
                    case SvgPathSegType.LineToHorizontalAbs:
                        return false;
                    case SvgPathSegType.LineToHorizontalRel:
                        return false;
                    case SvgPathSegType.LineToRel:
                        return false;
                    case SvgPathSegType.LineToVerticalAbs:
                        return false;
                    case SvgPathSegType.LineToVerticalRel:
                        return false;
                    case SvgPathSegType.MoveToAbs:
                        return false;
                    case SvgPathSegType.MoveToRel:
                        return false;
                    default:
                        return false;
                }
            }
        }

    }
}
