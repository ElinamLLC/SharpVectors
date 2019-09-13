using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPathSegHandler : ISvgPathHandler
    {
        private bool _isClosed;
        private bool _mayHaveCurves;

        private int _closedPath;
        private SvgPointF _startPoint;
        private SvgPathSegList _pathList;

        public SvgPathSegHandler(SvgPathSegList pathList)
        {
            _closedPath     = 0;
            _startPoint     = new SvgPointF(0, 0);
            _isClosed       = false;
            _mayHaveCurves  = false;
            _pathList       = pathList;
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

        public void StartPath()
        {
            _pathList.Clear();
        }

        public void EndPath()
        {
            _isClosed = (_closedPath == 1);
        }

        public void MovetoAbs(float x, float y)
        {
            var seg = new SvgPathSegMovetoAbs(x, y);

            _startPoint = new SvgPointF(x, y);
            SvgPointF endPoint = new SvgPointF(x, y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void MovetoRel(float x, float y)
        {
            var seg = new SvgPathSegMovetoRel(x, y);

            SvgPointF endPoint = new SvgPointF(x + _startPoint.X, y + _startPoint.Y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void ClosePath()
        {
            _closedPath++;
            var seg = new SvgPathSegClosePath();
            _pathList.AppendItem(seg);

            if (_pathList.Count >= 2)
            {
                SvgPointF endPoint = _pathList[0].Limits[0];
                seg.Limits = new SvgPointF[] { endPoint, _startPoint };
                _startPoint = endPoint;
            }
            else
            {
                seg.Limits = new SvgPointF[] { _startPoint, _startPoint };
                _startPoint = new SvgPointF(0, 0);
            }
        }

        public void LinetoAbs(float x, float y)
        {
            var seg = new SvgPathSegLinetoAbs(x, y);

            SvgPointF endPoint = new SvgPointF(x, y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void LinetoRel(float x, float y)
        {
            var seg = new SvgPathSegLinetoRel(x, y);

            SvgPointF endPoint = new SvgPointF(x + _startPoint.X, y + _startPoint.Y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void LinetoHorizontalAbs(float x)
        {
            var seg = new SvgPathSegLinetoHorizontalAbs(x);

            SvgPointF endPoint = new SvgPointF(x, _startPoint.Y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void LinetoHorizontalRel(float x)
        {
            var seg = new SvgPathSegLinetoHorizontalRel(x);

            SvgPointF endPoint = new SvgPointF(x + _startPoint.X, _startPoint.Y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void LinetoVerticalAbs(float y)
        {
            var seg = new SvgPathSegLinetoVerticalAbs(y);

            SvgPointF endPoint = new SvgPointF(_startPoint.X, y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void LinetoVerticalRel(float y)
        {
            var seg = new SvgPathSegLinetoVerticalRel(y);

            SvgPointF endPoint = new SvgPointF(_startPoint.X, y + _startPoint.Y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void CurvetoCubicAbs(float x1, float y1, float x2, float y2, float x, float y)
        {
            var seg = new SvgPathSegCurvetoCubicAbs(x, y, x1, y1, x2, y2);

            _mayHaveCurves = true;

            SvgPointF endPoint = new SvgPointF(x, y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void CurvetoCubicRel(float x1, float y1, float x2, float y2, float x, float y)
        {
            var seg = new SvgPathSegCurvetoCubicRel(x, y, x1, y1, x2, y2);

            _mayHaveCurves = true;

            SvgPointF endPoint = new SvgPointF(x + _startPoint.X, y + _startPoint.Y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void CurvetoCubicSmoothAbs(float x2, float y2, float x, float y)
        {
            var seg = new SvgPathSegCurvetoCubicSmoothAbs(x, y, x2, y2);

            _mayHaveCurves = true;

            SvgPointF endPoint = new SvgPointF(x, y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void CurvetoCubicSmoothRel(float x2, float y2, float x, float y)
        {
            var seg = new SvgPathSegCurvetoCubicSmoothRel(x, y, x2, y2);

            _mayHaveCurves = true;

            SvgPointF endPoint = new SvgPointF(x + _startPoint.X, y + _startPoint.Y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void CurvetoQuadraticAbs(float x1, float y1, float x, float y)
        {
            var seg = new SvgPathSegCurvetoQuadraticAbs(x, y, x1, y1);

            _mayHaveCurves = true;

            SvgPointF endPoint = new SvgPointF(x, y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void CurvetoQuadraticRel(float x1, float y1, float x, float y)
        {
            var seg = new SvgPathSegCurvetoQuadraticRel(x, y, x1, y1);

            _mayHaveCurves = true;

            SvgPointF endPoint = new SvgPointF(x + _startPoint.X, y + _startPoint.Y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void CurvetoQuadraticSmoothAbs(float x, float y)
        {
            var seg = new SvgPathSegCurvetoQuadraticSmoothAbs(x, y);

            _mayHaveCurves = true;

            SvgPointF endPoint = new SvgPointF(x, y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void CurvetoQuadraticSmoothRel(float x, float y)
        {
            var seg = new SvgPathSegCurvetoQuadraticSmoothRel(x, y);

            _mayHaveCurves = true;

            SvgPointF endPoint = new SvgPointF(x + _startPoint.X, y + _startPoint.Y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void ArcAbs(float rx, float ry, float xAxisRotation, bool largeArcFlag, bool sweepFlag, float x, float y)
        {
            var seg = new SvgPathSegArcAbs(x, y, rx, ry, xAxisRotation, largeArcFlag, sweepFlag);

            _mayHaveCurves = true;

            SvgPointF endPoint = new SvgPointF(x, y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }

        public void ArcRel(float rx, float ry, float xAxisRotation, bool largeArcFlag, bool sweepFlag, float x, float y)
        {
            var seg = new SvgPathSegArcRel(x, y, rx, ry, xAxisRotation, largeArcFlag, sweepFlag);

            SvgPointF endPoint = new SvgPointF(x + _startPoint.X, y + _startPoint.Y);
            seg.Limits = new SvgPointF[] { _startPoint, endPoint };
            _startPoint = endPoint;

            _pathList.AppendItem(seg);
        }
    }
}
