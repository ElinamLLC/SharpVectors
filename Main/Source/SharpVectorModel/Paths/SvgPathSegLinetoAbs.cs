using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPathSegLinetoAbs : SvgPathSegLineto, ISvgPathSegLinetoAbs
    {
        private double x;
        private double y;

        public SvgPathSegLinetoAbs(double x, double y)
            : base(SvgPathSegType.LineToAbs)
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
                return new SvgPointF(X, Y);
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
