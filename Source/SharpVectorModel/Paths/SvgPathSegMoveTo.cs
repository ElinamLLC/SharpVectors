using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgMoveToSeg.
    /// </summary>
    public abstract class SvgPathSegMoveto : SvgPathSeg
    {
        protected double _x;
        protected double _y;

        protected SvgPathSegMoveto(SvgPathSegType type, double x, double y)
            : base(type)
        {
            _x = x;
            _y = y;
        }

        public override SvgPathType PathType
        {
            get {
                return SvgPathType.MoveTo;
            }
        }

        public abstract override SvgPointF AbsXY { get; }

        public override double StartAngle
        {
            get {
                return 0;
            }
        }

        public override double EndAngle
        {
            get {
                return 0;
            }
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

        public override string PathText
        {
            get {
                StringBuilder sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(_x);
                sb.Append(",");
                sb.Append(_y);

                return sb.ToString();
            }
        }
    }
}
