using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPathSegLinetoRel : SvgPathSegLineto, ISvgPathSegLinetoRel
    {
        private double x;
        private double y;

        public SvgPathSegLinetoRel(double x, double y)
            : base(SvgPathSegType.LineToRel)
        {
            this.x = x;
            this.y = y;
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

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
    }
}
