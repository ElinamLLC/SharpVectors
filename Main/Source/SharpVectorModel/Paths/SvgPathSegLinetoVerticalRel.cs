using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPathSegLinetoVerticalRel : SvgPathSegLineto, ISvgPathSegLinetoVerticalRel
    {
        private double y;

        internal SvgPathSegLinetoVerticalRel(double y)
            : base(SvgPathSegType.LineToVerticalRel)
        {
            this.y = y;
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
                return new SvgPointF(prevPoint.X, prevPoint.Y + Y);
            }
        }

        public override string PathText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(Y);

                return sb.ToString();
            }
        }
    }
}
