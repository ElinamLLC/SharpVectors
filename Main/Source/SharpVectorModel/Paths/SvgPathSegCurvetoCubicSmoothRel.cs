using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgPathSegCurvetoCubicSmoothAbs.
    /// </summary>
    public sealed class SvgPathSegCurvetoCubicSmoothRel : SvgPathSegCurvetoCubic, ISvgPathSegCurvetoCubicSmoothRel
    {
        #region constructors

        public SvgPathSegCurvetoCubicSmoothRel(double x, double y, double x2, double y2)
            : base(SvgPathSegType.CurveToCubicSmoothRel)
        {
            this.x = x;
            this.y = y;
            this.x2 = x2;
            this.y2 = y2;
        }

        #endregion

        #region ISvgPathSegCurvetoCubicSmoothRel Members

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

        private double x2;
        public double X2
        {
            get { return x2; }
            set { x2 = value; }
        }

        private double y2;
        public double Y2
        {
            get { return y2; }
            set { y2 = value; }
        }

        #endregion

        #region Public Methods

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

        public override SvgPointF CubicX1Y1
        {
            get
            {
                SvgPathSeg prevSeg = PreviousSeg;
                if (prevSeg == null || !(prevSeg is SvgPathSegCurvetoCubic))
                {
                    return prevSeg.AbsXY;
                }
                else
                {
                    SvgPointF prevXY = prevSeg.AbsXY;
                    SvgPointF prevX2Y2 = ((SvgPathSegCurvetoCubic)prevSeg).CubicX2Y2;

                    return new SvgPointF(2 * prevXY.X - prevX2Y2.X, 2 * prevXY.Y - prevX2Y2.Y);
                }
            }
        }

        public override SvgPointF CubicX2Y2
        {
            get
            {
                SvgPathSeg prevSeg = PreviousSeg;
                SvgPointF prevPoint;
                if (prevSeg == null) prevPoint = new SvgPointF(0, 0);
                else prevPoint = prevSeg.AbsXY;
                return new SvgPointF(prevPoint.X + X2, prevPoint.Y + Y2);
            }
        }

        public override string PathText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(X2);
                sb.Append(",");
                sb.Append(Y2);
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
