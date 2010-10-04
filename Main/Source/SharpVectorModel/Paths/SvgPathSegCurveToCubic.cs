using System;

using SharpVectors.Polynomials;

namespace SharpVectors.Dom.Svg
{
	public abstract class SvgPathSegCurvetoCubic : SvgPathSegCurveto
	{
        #region constructors
		protected SvgPathSegCurvetoCubic(SvgPathSegType type) : base(type)
		{
		}
        #endregion

        #region abstract properties
        public abstract override SvgPointF AbsXY { get; }
        public abstract override SvgPointF CubicX1Y1 { get; }
        public abstract override SvgPointF CubicX2Y2 { get; }
        #endregion

        #region protected methods
        protected override SqrtPolynomial getArcLengthPolynomial() 
        {
            double c3x, c3y, c2x, c2y, c1x, c1y;
            SvgPointF p1 = PreviousSeg.AbsXY;
            SvgPointF p2 = CubicX1Y1;
            SvgPointF p3 = CubicX2Y2;
            SvgPointF p4 = AbsXY;
            
            // convert curve into polynomial
            c3x = -1.0*p1.X + 3.0*p2.X - 3.0*p3.X + p4.X;
            c3y = -1.0*p1.Y + 3.0*p2.Y - 3.0*p3.Y + p4.Y;

            c2x = 3.0*p1.X - 6.0*p2.X + 3.0*p3.X;
            c2y = 3.0*p1.Y - 6.0*p2.Y + 3.0*p3.Y;

            c1x = -3.0*p1.X + 3.0*p2.X;
            c1y = -3.0*p1.Y + 3.0*p2.Y;

            // build polynomial
            // dx = dx/dt
            // dy = dy/dt
            // sqrt poly = sqrt( (dx*dx) + (dy*dy) )
            return new SqrtPolynomial(
                c1x*c1x + c1y*c1y,
                4.0*(c1x*c2x + c1y*c2y),
                4.0*(c2x*c2x + c2y*c2y) + 6.0*(c1x*c3x + c1y*c3y),
                12.0*(c2x*c3x + c2y*c3y),
                9.0*(c3x*c3x + c3y*c3y)
            );
        }
        #endregion
	}
}
