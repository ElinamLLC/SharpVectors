using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPathSegLinetoRel : SvgPathSegLineto, ISvgPathSegLinetoRel
    {
        private double _x;
        private double _y;

        public SvgPathSegLinetoRel(double x, double y)
            : base(SvgPathSegType.LineToRel)
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
                //if (_limits != null && _limits.Length == 2)
                //{
                //    return _limits[1];
                //}
                SvgPathSeg prevSeg = this.PreviousSeg;
                SvgPointF prevPoint;
                if (prevSeg == null) prevPoint = new SvgPointF(0, 0);
                else prevPoint = prevSeg.AbsXY;
                return new SvgPointF(prevPoint.X + X, prevPoint.Y + Y);
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
