using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPathSegLinetoVerticalAbs : SvgPathSegLineto, ISvgPathSegLinetoVerticalAbs
    {
        private double _y;

        public SvgPathSegLinetoVerticalAbs(double y)
            : base(SvgPathSegType.LineToVerticalAbs)
        {
            _y = y;
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public override SvgPointF AbsXY
        {
            get {
                SvgPathSeg prevSeg = this.PreviousSeg;
                SvgPointF prevPoint;
                if (prevSeg == null) prevPoint = new SvgPointF(0, 0);
                else prevPoint = prevSeg.AbsXY;
                return new SvgPointF(prevPoint.X, Y);
            }
        }

        public override string PathText
        {
            get {
                StringBuilder sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(Y);

                return sb.ToString();
            }
        }
    }
}
