using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgPathSegCurvetoCubicRel.
    /// </summary>
    public sealed class SvgPathSegCurvetoCubicRel : SvgPathSegCurvetoCubic, ISvgPathSegCurvetoCubicRel
    {
        #region constructors
        internal SvgPathSegCurvetoCubicRel(double x, double y, double x1, double y1, double x2, double y2)
            : base(SvgPathSegType.CurveToCubicRel)
        {
            this.x = x;
            this.y = y;
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
        #endregion

        #region ISvgPathSegCurvetoCubicRel Members

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
                SvgPointF prevPoint;
                if (prevSeg == null) prevPoint = new SvgPointF(0, 0);
                else prevPoint = prevSeg.AbsXY;

                return new SvgPointF(prevPoint.X + X1, prevPoint.Y + Y1);
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
                sb.Append(X1);
                sb.Append(",");
                sb.Append(Y1);
                sb.Append(",");
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
