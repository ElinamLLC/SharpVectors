using System;
using System.Text;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgMoveToSeg.
    /// </summary>
    public abstract class SvgPathSegMoveto : SvgPathSeg
    {
        protected SvgPathSegMoveto(SvgPathSegType type, double x, double y)
            : base(type)
        {
            this.x = x;
            this.y = y;
        }

        public abstract override SvgPointF AbsXY { get; }

        public override double StartAngle
        {
            get
            {
                return 0;
            }
        }

        public override double EndAngle
        {
            get
            {
                return 0;
            }
        }

        public override string PathText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(x);
                sb.Append(",");
                sb.Append(y);

                return sb.ToString();
            }
        }

        protected double x;
        public double X
        {
            get { return x; }
            set { x = value; }
        }

        protected double y;
        public double Y
        {
            get { return y; }
            set { y = value; }
        }
    }
}
