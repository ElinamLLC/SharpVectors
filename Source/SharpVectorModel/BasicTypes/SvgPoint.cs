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
		#region Private Fields

		private double _x;
		private double _y;
		
        #endregion
		
		#region Constructor

		public SvgPoint(double x, double y)
		{
			_x = x;
			_y = y;
		}

		#endregion

        #region ISvgPoint Members

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

		public ISvgPoint MatrixTransform(ISvgMatrix matrix)
		{
			return new SvgPoint(matrix.A*_x + matrix.C*_y + matrix.E, matrix.B*_x + matrix.D*_y + matrix.F);
		}

		#endregion

        #region Additional operators

        public SvgPoint lerp(SvgPoint that, double percent)         {
            return new SvgPoint(_x + (that._x - _x)*percent, _y + (that._y - _y)*percent);
        }

        public static SvgPoint operator+(SvgPoint a, SvgPoint b)
        {
            return new SvgPoint(a._x + b._x, a._y + b._y);
        }

        public static SvgPoint operator-(SvgPoint a, SvgPoint b)
        {
            return new SvgPoint(a._x - b._x, a._y - b._y);
        }

        public static SvgPoint operator*(SvgPoint a, double scalar)
        {
            return new SvgPoint(a._x * scalar, a._y * scalar);
        }

        public static SvgPoint operator*(double scalar, SvgPoint a)
        {
            return new SvgPoint(scalar * a._x, scalar * a._y);
        }
        
        public static SvgPoint operator/(SvgPoint a, double scalar)
        {
            return new SvgPoint(a._x / scalar, a._y / scalar);
        }

        public static SvgPoint operator/(double scalar, SvgPoint a)
        {
            return new SvgPoint(scalar / a._x, scalar / a._y);
        }

        #endregion
	}
}