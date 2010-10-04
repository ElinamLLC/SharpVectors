using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgPathSegCurvetoQuadraticSmoothRel interface corresponds to an "relative smooth quadratic curveto" (t) path data command. 
    /// </summary>
    public sealed class SvgPathSegCurvetoQuadraticSmoothRel : SvgPathSegCurvetoQuadratic, ISvgPathSegCurvetoQuadraticSmoothRel
    {
        #region Constructors

        public SvgPathSegCurvetoQuadraticSmoothRel(double x, double y)
            : base(SvgPathSegType.CurveToQuadraticSmoothRel)
        {
            this.x = x;
            this.y = y;
        }

        #endregion

        #region SvgpathSegCurvetoQuadraticSmoothRel Members

        private double x;
        /// <summary>
        /// The absolute X coordinate for the end point of this path segment. 
        /// </summary>
        public double X
        {
            get { return x; }
            set { x = value; }
        }

        private double y;
        /// <summary>
        /// The absolute Y coordinate for the end point of this path segment. 
        /// </summary>
        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        #endregion

        #region Public Methods

        public override SvgPointF QuadraticX1Y1
        {
            get
            {
                SvgPathSeg prevSeg = PreviousSeg;
                if (prevSeg == null || !(prevSeg is SvgPathSegCurvetoQuadratic))
                {
                    return prevSeg.AbsXY;
                }
                else
                {
                    SvgPointF prevXY   = prevSeg.AbsXY;
                    SvgPointF prevX1Y1 = ((SvgPathSegCurvetoQuadratic)prevSeg).QuadraticX1Y1;

                    return new SvgPointF(2 * prevXY.X - prevX1Y1.X, 2 * prevXY.Y - prevX1Y1.Y);
                }
            }
        }

        public override SvgPointF AbsXY
        {
            get
            {
                SvgPathSeg prevSeg = PreviousSeg;
                SvgPointF prevPoint;
                if (prevSeg == null) 
                    prevPoint = new SvgPointF(0, 0);
                else 
                    prevPoint = prevSeg.AbsXY;

                return new SvgPointF(prevPoint.X + X, prevPoint.Y + Y);
            }
        }

        public override SvgPointF CubicX1Y1
        {
            get
            {
                SvgPointF prevPoint = PreviousSeg.AbsXY;
                SvgPointF x1y1 = QuadraticX1Y1;

                double x1 = prevPoint.X + (x1y1.X - prevPoint.X) * 2 / 3;
                double y1 = prevPoint.Y + (x1y1.Y - prevPoint.Y) * 2 / 3;

                return new SvgPointF(x1, y1);
            }
        }

        public override SvgPointF CubicX2Y2
        {
            get
            {
                SvgPointF xy = AbsXY;
                SvgPointF x1y1 = QuadraticX1Y1;
                double x2 = x1y1.X + (xy.X - x1y1.X) / 3;
                double y2 = x1y1.Y + (xy.Y - x1y1.Y) / 3;

                return new SvgPointF(x2, y2);
            }
        }

        public override string PathText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(X);
                sb.Append(",");
                sb.Append(Y);

                return sb.ToString();
            }
        }

        #endregion
    }
}
