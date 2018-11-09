using System;

using SharpVectors.Polynomials;

namespace SharpVectors.Dom.Svg
{
    public abstract class SvgPathSegCurveto : SvgPathSeg
    {
        #region Constructors

        protected SvgPathSegCurveto(SvgPathSegType type) 
            : base(type)
        {
        }

        #endregion

        #region Public Properties

        public abstract override SvgPointF AbsXY { get; }
        public abstract SvgPointF CubicX1Y1 { get; }
        public abstract SvgPointF CubicX2Y2 { get; }

        public override double Length
        {
            get {
                return this.GetArcLengthPolynomial().Simpson(0, 1);
            }
        }

        public override double StartAngle
        {
            get {
                SvgPointF p1 = PreviousSeg.AbsXY;
                SvgPointF p2 = CubicX1Y1;

                double dx = p2.X - p1.X;
                double dy = p2.Y - p1.Y;
                double a = (Math.Atan2(dy, dx) * 180 / Math.PI);
                a += 270;
                a %= 360;
                return a;
            }
        }

        public override double EndAngle
        {
            get {
                SvgPointF p1 = this.CubicX2Y2;
                SvgPointF p2 = AbsXY;

                double dx = p1.X - p2.X;
                double dy = p1.Y - p2.Y;
                double a = (Math.Atan2(dy, dx) * 180 / Math.PI);
                a += 270;
                a %= 360;
                return a;
            }
        }

        #endregion

        #region Protected Methods

        protected abstract SqrtPolynomial GetArcLengthPolynomial();

        #endregion
    }
}
