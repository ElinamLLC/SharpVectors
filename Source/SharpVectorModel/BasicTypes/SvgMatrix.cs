using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// <para>
    /// Many of SVG's graphics operations utilize 2x3 matrices of the form:
    ///     [a c e]
    ///     [b d f]
    /// </para>
    /// <para>
    /// which, when expanded into a 3x3 matrix for the purposes of matrix arithmetic, become:
    ///     [a c e]
    ///     [b d f]
    ///     [0 0 1]
    /// </para>
    /// </summary>
    public sealed class SvgMatrix : ISvgMatrix
    {
        #region Public Fields

        public static readonly SvgMatrix Identity = new SvgMatrix();

        #endregion

        #region Private Fields

        private double _a;
        private double _b;
        private double _c;
        private double _d;
        private double _e;
        private double _f;

        private bool _isIdentity;

        #endregion

        #region Constructors

        public SvgMatrix()
            : this(1, 0, 0, 1, 0, 0)
        {
            _isIdentity = true;
        }

        public SvgMatrix(double a, double b, double c, double d, double e, double f)
        {
            _a = a;
            _b = b;
            _c = c;
            _d = d;
            _e = e;
            _f = f;

            _isIdentity = IsIdentityMatrix();
        }

        #endregion

        #region ISvgMatrix Members

        public bool IsIdentity
        {
            get
            {
                return IsIdentityMatrix();
            }
        }

        public double A
        {
            get { return _a; }
            set { _a = value; }
        }

        public double B
        {
            get { return _b; }
            set { _b = value; }
        }

        public double C
        {
            get { return _c; }
            set { _c = value; }
        }

        public double D
        {
            get { return _d; }
            set { _d = value; }
        }

        public double E
        {
            get { return _e; }
            set { _e = value; }
        }

        public double F
        {
            get { return _f; }
            set { _f = value; }
        }

        public ISvgMatrix Multiply(ISvgMatrix secondMatrix)
        {
            if (secondMatrix == null)
            {
                secondMatrix = new SvgMatrix();
            }

            SvgMatrix matrix = (SvgMatrix)secondMatrix;
            return new SvgMatrix(_a * matrix._a + _c * matrix._b, _b * matrix._a + _d * matrix._b,
              _a * matrix._c + _c * matrix._d, _b * matrix._c + _d * matrix._d,
              _a * matrix._e + _c * matrix._f + _e, _b * matrix._e + _d * matrix._f + _f);
        }

        public ISvgMatrix Inverse()
        {
            double det1 = _a * _d - _b * _c;

            if (det1.Equals(0))
                throw new SvgException(SvgExceptionType.SvgMatrixNotInvertable);

            double iDet = 1.0 / det1;
            double det2 = _f * _c - _e * _d;
            double det3 = _e * _b - _f * _a;

            return new SvgMatrix(_d * iDet, -_b * iDet, -_c * iDet, _a * iDet, det2 * iDet, det3 * iDet);
        }

        public ISvgMatrix Translate(double x, double y)
        {
            return new SvgMatrix(_a, _b, _c, _d, _a * x + _c * y + _e, _b * x + _d * y + _f);
        }

        public ISvgMatrix Scale(double scaleFactor)
        {
            return new SvgMatrix(_a * scaleFactor, _b * scaleFactor, _c * scaleFactor, _d * scaleFactor, _e, _f);
        }

        public ISvgMatrix ScaleNonUniform(double scaleFactorX, double scaleFactorY)
        {
            return new SvgMatrix(_a * scaleFactorX, _b * scaleFactorX, _c * scaleFactorY, _d * scaleFactorY, _e, _f);
        }

        public ISvgMatrix Rotate(double angle)
        {
            double radians = angle * (Math.PI / 180.0);
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            return new SvgMatrix(_a * cos + _c * sin, _b * cos + _d * sin,
              _a * (-sin) + _c * cos, _b * (-sin) + _d * cos, _e, _f);
        }

        public ISvgMatrix RotateFromVector(double x, double y)
        {
            if (x.Equals(0) || y.Equals(0))
                throw new SvgException(SvgExceptionType.SvgInvalidValueErr);

            double length = Math.Sqrt((x * x) + (y * y));
            double cos = x / length;
            double sin = y / length;

            return new SvgMatrix(_a * cos + _c * sin, _b * cos + _d * sin,
              _a * (-sin) + _c * cos, _b * (-sin) + _d * cos, _e, _f);
        }

        public ISvgMatrix FlipX()
        {
            return new SvgMatrix(-_a, -_b, _c, _d, _e, _f);
        }

        public ISvgMatrix FlipY()
        {
            return new SvgMatrix(_a, _b, -_c, -_d, _e, _f);
        }

        public ISvgMatrix SkewX(double angle)
        {
            double tan = Math.Tan(angle * (Math.PI / 180.0));

            return new SvgMatrix(_a, _b, _a * tan + _c, _b * tan + _d, _e, _f);
        }

        public ISvgMatrix SkewY(double angle)
        {
            double tan = Math.Tan(angle * (Math.PI / 180.0));

            return new SvgMatrix(_a + _c * tan, _b + _d * tan, _c, _d, _e, _f);
        }

        #endregion

        #region Additional operators

        public static SvgMatrix operator *(SvgMatrix a, SvgMatrix b)
        {
            return (SvgMatrix)a.Multiply(b);
        }

        #endregion

        #region Private Methods

        private bool IsIdentityMatrix()
        {
            if (_isIdentity)
            {
                return true;
            }

            if (!_a.Equals(1))
                return false;
            if (!_b.Equals(0))
                return false;
            if (!_c.Equals(0))
                return false;
            if (!_d.Equals(1))
                return false;
            if (!_e.Equals(0))
                return false;
            if (!_f.Equals(0))
                return false;

            _isIdentity = true;

            return true;
        }

        #endregion
    }
}
