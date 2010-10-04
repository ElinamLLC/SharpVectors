// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>

using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgMatrix.
    /// </summary>
    public sealed class SvgMatrix : ISvgMatrix
    {
        #region Public Fields

        public static readonly SvgMatrix Identity = new SvgMatrix();

        #endregion

        #region Private Fields

        private double a;
        private double b;
        private double c;
        private double d;
        private double e;
        private double f;

        private bool _isIdentity;

        #endregion

        #region Constructors

        public SvgMatrix()
            : this(1, 0, 0, 1, 0, 0)
        {
            this._isIdentity = true;
        }

        public SvgMatrix(double a, double b, double c, double d, double e, double f)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.f = f;

            this._isIdentity = (a == 1 && b == 0 && c == 0 && d == 1 && e == 0 && f == 0);
        }

        #endregion

        #region ISvgMatrix Members

        public bool IsIdentity
        {
            get
            {
                if (_isIdentity)
                {
                    return true;
                }

                return (a == 1 && b == 0 && c == 0 && d == 1 && e == 0 && f == 0);
            }
        }

        public double A
        {
            get { return this.a; }
            set { this.a = value; }
        }

        public double B
        {
            get { return this.b; }
            set { this.b = value; }
        }

        public double C
        {
            get { return this.c; }
            set { this.c = value; }
        }

        public double D
        {
            get { return this.d; }
            set { this.d = value; }
        }

        public double E
        {
            get { return this.e; }
            set { this.e = value; }
        }

        public double F
        {
            get { return this.f; }
            set { this.f = value; }
        }

        public ISvgMatrix Multiply(ISvgMatrix secondMatrix)
        {
            if (secondMatrix == null)
            {
                secondMatrix = new SvgMatrix();
            }

            SvgMatrix matrix = (SvgMatrix)secondMatrix;
            return new SvgMatrix(
              this.a * matrix.a + this.c * matrix.b,
              this.b * matrix.a + this.d * matrix.b,
              this.a * matrix.c + this.c * matrix.d,
              this.b * matrix.c + this.d * matrix.d,
              this.a * matrix.e + this.c * matrix.f + this.e,
              this.b * matrix.e + this.d * matrix.f + this.f
              );
        }

        public ISvgMatrix Inverse()
        {
            double det1 = this.a * this.d - this.b * this.c;

            if (det1 == 0)
                throw new SvgException(SvgExceptionType.SvgMatrixNotInvertable);

            double iDet = 1.0 / det1;
            double det2 = this.f * this.c - this.e * this.d;
            double det3 = this.e * this.b - this.f * this.a;

            return new SvgMatrix(
              this.d * iDet,
              -this.b * iDet,
              -this.c * iDet,
              this.a * iDet,
              det2 * iDet,
              det3 * iDet
              );
        }

        public ISvgMatrix Translate(double x, double y)
        {
            return new SvgMatrix(
              this.a,
              this.b,
              this.c,
              this.d,
              this.a * x + this.c * y + this.e,
              this.b * x + this.d * y + this.f
              );
        }

        public ISvgMatrix Scale(double scaleFactor)
        {
            return new SvgMatrix(
              this.a * scaleFactor,
              this.b * scaleFactor,
              this.c * scaleFactor,
              this.d * scaleFactor,
              this.e,
              this.f
              );
        }

        public ISvgMatrix ScaleNonUniform(double scaleFactorX, double scaleFactorY)
        {
            return new SvgMatrix(
              this.a * scaleFactorX,
              this.b * scaleFactorX,
              this.c * scaleFactorY,
              this.d * scaleFactorY,
              this.e,
              this.f
              );
        }

        public ISvgMatrix Rotate(double angle)
        {
            double radians = angle * (Math.PI / 180.0);
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            return new SvgMatrix(
              this.a * cos + this.c * sin,
              this.b * cos + this.d * sin,
              this.a * (-sin) + this.c * cos,
              this.b * (-sin) + this.d * cos,
              this.e,
              this.f
              );
        }

        public ISvgMatrix RotateFromVector(double x, double y)
        {
            if (x == 0 || y == 0)
                throw new SvgException(SvgExceptionType.SvgInvalidValueErr);

            double length = Math.Sqrt((x * x) + (y * y));
            double cos = x / length;
            double sin = y / length;

            return new SvgMatrix(
              this.a * cos + this.c * sin,
              this.b * cos + this.d * sin,
              this.a * (-sin) + this.c * cos,
              this.b * (-sin) + this.d * cos,
              this.e,
              this.f
              );
        }

        public ISvgMatrix FlipX()
        {
            return new SvgMatrix(
              -this.a,
              -this.b,
              this.c,
              this.d,
              this.e,
              this.f
              );
        }

        public ISvgMatrix FlipY()
        {
            return new SvgMatrix(
              this.a,
              this.b,
              -this.c,
              -this.d,
              this.e,
              this.f
              );
        }

        public ISvgMatrix SkewX(double angle)
        {
            double tan = Math.Tan(angle * (Math.PI / 180.0));

            return new SvgMatrix(
              this.a,
              this.b,
              this.a * tan + this.c,
              this.b * tan + this.d,
              this.e,
              this.f
              );
        }

        public ISvgMatrix SkewY(double angle)
        {
            double tan = Math.Tan(angle * (Math.PI / 180.0));

            return new SvgMatrix(
              this.a + this.c * tan,
              this.b + this.d * tan,
              this.c,
              this.d,
              this.e,
              this.f
              );
        }

        #endregion

        #region Additional operators

        public static SvgMatrix operator *(SvgMatrix a, SvgMatrix b)
        {
            return (SvgMatrix)a.Multiply(b);
        }

        #endregion
    }
}
