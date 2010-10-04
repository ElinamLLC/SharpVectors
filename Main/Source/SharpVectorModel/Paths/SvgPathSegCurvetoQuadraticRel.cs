using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgPathSegCurvetoCubicAbs.
    /// </summary>
    public sealed class SvgPathSegCurvetoQuadraticRel : SvgPathSegCurvetoQuadratic, ISvgPathSegCurvetoQuadraticRel
    {
        #region constructors

        public SvgPathSegCurvetoQuadraticRel(double x, double y, double x1, double y1)
            : base(SvgPathSegType.CurveToQuadraticRel)
        {
            this.x = x;
            this.y = y;
            this.x1 = x1;
            this.y1 = y1;
        }

        #endregion

        #region SvgPathSegCurvetoQuadraticRel Members

        private double x;
        public double X
        {
            get { return x; }
            set { x = value; }
        }

        private double y;
        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        private double x1;
        public double X1
        {
            get { return x1; }
            set { x1 = value; }
        }

        private double y1;
        public double Y1
        {
            get { return y1; }
            set { y1 = value; }
        }
        #endregion

        #region Pubic Methods

        public override SvgPointF AbsXY
        {
            get
            {
                SvgPathSeg prevSeg = PreviousSeg;
                SvgPointF prevPoint;
                if (prevSeg == null) prevPoint = new SvgPointF(0, 0);
                else prevPoint = prevSeg.AbsXY;

                return new SvgPointF(prevPoint.X + X, prevPoint.Y + Y);
            }
        }

        public override SvgPointF QuadraticX1Y1
        {
            get
            {
                SvgPathSeg prevSeg = PreviousSeg;
                SvgPointF prevPoint;
                if (prevSeg == null) prevPoint = new SvgPointF(0, 0);
                else prevPoint = prevSeg.AbsXY;

                return new SvgPointF(prevPoint.X + X1, prevPoint.Y + Y1);
            }
        }

        public override SvgPointF CubicX1Y1
        {
            get
            {
                SvgPointF prevPoint = PreviousSeg.AbsXY;

                double x1 = prevPoint.X + X1 * 2 / 3;
                double y1 = prevPoint.Y + Y1 * 2 / 3;

                return new SvgPointF(x1, y1);
            }
        }

        public override SvgPointF CubicX2Y2
        {
            get
            {
                SvgPointF prevPoint = PreviousSeg.AbsXY;

                double x2 = X1 + prevPoint.X + (X - X1) / 3;
                double y2 = Y1 + prevPoint.Y + (Y - Y1) / 3;

                return new SvgPointF(x2, y2);
            }
        }

        public override string PathText
        {
            get
            {
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
