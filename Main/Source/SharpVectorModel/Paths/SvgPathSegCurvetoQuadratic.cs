using System;

using SharpVectors.Polynomials;

namespace SharpVectors.Dom.Svg
{
	public abstract class SvgPathSegCurvetoQuadratic : SvgPathSegCurveto
	{
		protected SvgPathSegCurvetoQuadratic(SvgPathSegType type) : base(type)
		{
		}

		public abstract override SvgPointF AbsXY{get;}
		public abstract override SvgPointF CubicX1Y1{get;}
		public abstract override SvgPointF CubicX2Y2{get;}

		public abstract SvgPointF QuadraticX1Y1{get;}

        protected override SqrtPolynomial getArcLengthPolynomial() 
        {
            double c2x, c2y, c1x, c1y;
            SvgPointF p1 = PreviousSeg.AbsXY;
            SvgPointF p2 = QuadraticX1Y1;
            SvgPointF p3 = AbsXY;
            
            c2x = p1.X - 2.0 * p2.X + p3.X;
            c2y = p1.Y - 2.0 * p2.Y + p3.Y;

            c1x = -2.0*p1.X + 2.0*p2.X;
            c1y = -2.0*p1.Y + 2.0*p2.Y;

            // build polynomial
            // dx = dx/dt
            // dy = dy/dt
            // sqrt poly = sqrt( (dx*dx) + (dy*dy) )
            return new SqrtPolynomial(
                c1x*c1x + c1y*c1y, 4.0*(c1x*c2x + c1y*c2y), 4.0*(c2x*c2x + c2y*c2y));
        }
	}
}
