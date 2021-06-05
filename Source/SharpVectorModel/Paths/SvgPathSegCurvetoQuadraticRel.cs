using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgPathSegCurvetoCubicAbs.
    /// </summary>
    public sealed class SvgPathSegCurvetoQuadraticRel : SvgPathSegCurvetoQuadratic, ISvgPathSegCurvetoQuadraticRel
    {
        #region Private Fields

        private double _x;
        private double _y;
        private double _x1;
        private double _y1;

        #endregion

        #region Constructors

        public SvgPathSegCurvetoQuadraticRel(double x, double y, double x1, double y1)
            : base(SvgPathSegType.CurveToQuadraticRel)
        {
            _x  = x;
            _y  = y;
            _x1 = x1;
            _y1 = y1;
        }

        #endregion

        #region SvgPathSegCurvetoQuadraticRel Members

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public double X1
        {
            get { return _x1; }
            set { _x1 = value; }
        }

        public double Y1
        {
            get { return _y1; }
            set { _y1 = value; }
        }

        public override SvgPointF AbsXY
        {
            get {
                SvgPathSeg prevSeg = PreviousSeg;
                SvgPointF prevPoint;
                if (prevSeg == null) prevPoint = new SvgPointF(0, 0);
                else prevPoint = prevSeg.AbsXY;

                return new SvgPointF(prevPoint.X + X, prevPoint.Y + Y);
            }
        }

        public override SvgPointF QuadraticX1Y1
        {
            get {
                SvgPathSeg prevSeg = PreviousSeg;
                SvgPointF prevPoint;
                if (prevSeg == null) prevPoint = new SvgPointF(0, 0);
                else prevPoint = prevSeg.AbsXY;

                return new SvgPointF(prevPoint.X + X1, prevPoint.Y + Y1);
            }
        }

        public override SvgPointF CubicX1Y1
        {
            get {
                SvgPointF prevPoint = PreviousSeg.AbsXY;

                double x1 = prevPoint.X + X1 * 2 / 3;
                double y1 = prevPoint.Y + Y1 * 2 / 3;

                return new SvgPointF(x1, y1);
            }
        }

        public override SvgPointF CubicX2Y2
        {
            get {
                SvgPointF prevPoint = PreviousSeg.AbsXY;

                double x2 = X1 + prevPoint.X + (X - X1) / 3;
                double y2 = Y1 + prevPoint.Y + (Y - Y1) / 3;

                return new SvgPointF(x2, y2);
            }
        }

        public override string PathText
        {
            get {
                StringBuilder sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(X1);
                sb.Append(",");
                sb.Append(Y1);
                sb.Append(",");
                sb.Append(X);
                sb.Append(",");
                sb.Append(Y);

                return sb.ToString();
            }
        }

        #endregion
    }
}
