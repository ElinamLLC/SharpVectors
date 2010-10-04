using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgPathSegCurvetoCubicAbs.
    /// </summary>
    public sealed class SvgPathSegCurvetoQuadraticAbs : SvgPathSegCurvetoQuadratic, ISvgPathSegCurvetoQuadraticAbs
    {
        #region constructors

        public SvgPathSegCurvetoQuadraticAbs(double x, double y, double x1, double y1)
            : base(SvgPathSegType.CurveToQuadraticAbs)
        {
            this.x = x;
            this.y = y;
            this.x1 = x1;
            this.y1 = y1;
        }

        #endregion

        #region ISvgPathSegCurvetoQuadraticAbs Members

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

        #region Public Methods

        public override SvgPointF AbsXY
        {
            get
            {
                return new SvgPointF(X, Y);
            }
        }

        public override SvgPointF QuadraticX1Y1
        {
            get
            {
                return new SvgPointF(X1, Y1);
            }
        }

        /*
        * Convert to cubic bezier using the algorithm from Math:Bezier:Convert in CPAN
        * $p0x+($p1x-$p0x)*2/3
        * $p0y+($p1y-$p0y)*2/3
        * $p1x+($p2x-$p1x)/3
        * $p1x+($p2x-$p1x)/3
        * */

        public override SvgPointF CubicX1Y1
        {
            get
            {
                SvgPointF prevPoint = PreviousSeg.AbsXY;

                double x1 = prevPoint.X + (X1 - prevPoint.X) * 2 / 3;
                double y1 = prevPoint.Y + (Y1 - prevPoint.Y) * 2 / 3;

                return new SvgPointF(x1, y1);
            }
        }

        public override SvgPointF CubicX2Y2
        {
            get
            {
                double x2 = X1 + (X - X1) / 3;
                double y2 = Y1 + (Y - Y1) / 3;

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
