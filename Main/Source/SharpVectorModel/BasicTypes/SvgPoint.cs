// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Many of the SVG DOM interfaces refer to objects of class SvgPoint.
	/// An SvgPoint is an (x,y) coordinate pair. When used in matrix 
	/// operations, an SvgPoint is treated as a vector of the form:	
	///     [x]
	///     [y]
	///     [1]
	/// </summary>
    public sealed class SvgPoint : ISvgPoint
	{
		#region Fields

		private double x;
		private double y;
		
        #endregion
		
		#region Constructor

		public SvgPoint(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		#endregion

        #region ISvgPoint Members

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

		public ISvgPoint MatrixTransform(ISvgMatrix matrix)
		{
			return new SvgPoint(matrix.A*x + matrix.C*y + matrix.E,
                matrix.B*x + matrix.D*y + matrix.F);
		}

		#endregion

        #region Additional operators

        public SvgPoint lerp(SvgPoint that, double percent)         {
            return new SvgPoint(
                this.x + (that.x - this.x)*percent,
                this.y + (that.y - this.y)*percent
            );
        }

        public static SvgPoint operator+(SvgPoint a, SvgPoint b)
        {
            return new SvgPoint(
                a.x + b.x,
                a.y + b.y
            );
        }

        public static SvgPoint operator-(SvgPoint a, SvgPoint b)
        {
            return new SvgPoint(
                a.x - b.x,
                a.y - b.y
            );
        }

        public static SvgPoint operator*(SvgPoint a, double scalar)
        {
            return new SvgPoint(
                a.x * scalar,
                a.y * scalar
            );
        }

        public static SvgPoint operator*(double scalar, SvgPoint a)
        {
            return new SvgPoint(
                scalar * a.x,
                scalar * a.y
            );
        }
        
        public static SvgPoint operator/(SvgPoint a, double scalar)
        {
            return new SvgPoint(
                a.x / scalar,
                a.y / scalar
                );
        }

        public static SvgPoint operator/(double scalar, SvgPoint a)
        {
            return new SvgPoint(
                scalar / a.x,
                scalar / a.y
                );
        }

        #endregion
	}
}