using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPathSegLinetoAbs : SvgPathSegLineto, ISvgPathSegLinetoAbs
    {
        private double _x;
        private double _y;

        public SvgPathSegLinetoAbs(double x, double y)
            : base(SvgPathSegType.LineToAbs)
        {
            _x = x;
            _y = y;
        }

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

        public override SvgPointF AbsXY
        {
            get {
                return new SvgPointF(X, Y);
            }
        }

        public override string PathText
        {
            get {
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
