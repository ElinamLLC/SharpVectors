using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPathSegLinetoHorizontalRel : SvgPathSegLineto, ISvgPathSegLinetoHorizontalRel
    {
        private double x;

        public SvgPathSegLinetoHorizontalRel(double x)
            : base(SvgPathSegType.LineToHorizontalRel)
        {
            this.x = x;
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public override SvgPointF AbsXY
        {
            get
            {
                SvgPathSeg prevSeg = PreviousSeg;
                SvgPointF prevPoint;
                if (prevSeg == null) prevPoint = new SvgPointF(0, 0);
                else prevPoint = prevSeg.AbsXY;
                return new SvgPointF(prevPoint.X + X, prevPoint.Y);
            }
        }

        public override string PathText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(X);

                return sb.ToString();
            }
        }
    }
}
