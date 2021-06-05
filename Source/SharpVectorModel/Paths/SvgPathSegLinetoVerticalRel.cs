using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPathSegLinetoVerticalRel : SvgPathSegLineto, ISvgPathSegLinetoVerticalRel
    {
        private double _y;

        public SvgPathSegLinetoVerticalRel(double y)
            : base(SvgPathSegType.LineToVerticalRel)
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
                //if (_limits != null && _limits.Length == 2)
                //{
                //    return _limits[1];
                //}
                SvgPathSeg prevSeg = this.PreviousSeg;
                SvgPointF prevPoint;
                if (prevSeg == null) prevPoint = new SvgPointF(0, 0);
                else prevPoint = prevSeg.AbsXY;
                return new SvgPointF(prevPoint.X, prevPoint.Y + Y);
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
