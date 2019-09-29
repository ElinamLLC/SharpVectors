using System;

namespace SharpVectors.Polynomials
{
    /// <summary>
    /// Summary description for Polynomial.
    /// </summary>
    public class Polynomial
    {
        #region Private Fields

        private double[] _coefficients;
        private double _s;

        #endregion

        #region Constructors

        /// <summary>
        /// Polynomial constuctor
        /// </summary>
        /// <param name="coefficients"></param>
		public Polynomial(params double[] coefficients)
        {
            int end = 0;
            double TOLERANCE = 1e-9;

            for (end = coefficients.Length; end > 0; end--)
            {
                if (Math.Abs(coefficients[end - 1]) > TOLERANCE)
                    break;
            }

            if (end > 0)
            {
                this._coefficients = new double[coefficients.Length - (coefficients.Length - end)];
                for (int i = 0; i < end; i++)
                {
                    this._coefficients[i] = coefficients[i];
                }
            }
            else
            {
                this._coefficients = new double[0];
            }
        }

        public Polynomial(Polynomial that)
        {
            this._coefficients = that._coefficients;
        }

        #endregion

        #region Public Properties

        public int Degree
        {
            get { return this._coefficients.Length - 1; }
        }

        public double this[int index]
        {
            get { return this._coefficients[index]; }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Interpolate - adapted from "Numerical Recipes in C"
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="n"></param>
        /// <param name="offset"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static ValueWithError Interpolate(double[] xs, double[] ys, int n, int offset, double x)
        {
            double y;
            double dy = 0.0;
            double[] c = new double[n];
            double[] d = new double[n];
            int ns = 0;

            double diff = Math.Abs(x - xs[offset]);
            for (int i = 0; i < n; i++)
            {
                double dift = Math.Abs(x - xs[offset + i]);

                if (dift < diff)
                {
                    ns = i;
                    diff = dift;
                }
                c[i] = d[i] = ys[offset + i];
            }
            y = ys[offset + ns];
            ns--;

            for (int m = 1; m < n; m++)
            {
                for (int i = 0; i < n - m; i++)
                {
                    double ho = xs[offset + i] - x;
                    double hp = xs[offset + i + m] - x;
                    double w = c[i + 1] - d[i];
                    double den = ho - hp;

                    if (den.Equals(0.0)) return new ValueWithError(0, 0);

                    den = w / den;
                    d[i] = hp * den;
                    c[i] = ho * den;
                }
                dy = (2 * (ns + 1) < (n - m)) ? c[ns + 1] : d[ns--];
                y += dy;
            }

            return new ValueWithError(y, dy);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Evaluate
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual double Evaluate(double t)
        {
            double result = 0.0;

            for (int i = _coefficients.Length - 1; i >= 0; i--)
            {
                result = result * t + _coefficients[i];
            }

            return result;
        }

        /// <summary>
        /// Simspon - adapted from "Numerical Recipes in C"
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public double Simpson(double min, double max)
        {
            double s = 0.0;
            double st = 0.0;
            double os = 0.0;
            double ost = 0.0;
            int MAX = 20;
            double TOLERANCE = 1e-7;

            for (int j = 1; j <= MAX; j++)
            {
                st = this.Trapezoid(min, max, j);
                s = (4.0 * st - ost) / 3.0;
                if (Math.Abs(s - os) < TOLERANCE * Math.Abs(os)) break;
                os = s;
                ost = st;
            }

            return s;
        }

        /// <summary>
        /// Romberg - adapted from "Numerical Recipes in C"
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public double Romberg(double min, double max)
        {
            int MAX = 20;
            double TOLERANCE = 1e-7;
            int K = 4;
            double[] s = new double[MAX + 1];
            double[] h = new double[MAX + 1];
            ValueWithError result = new ValueWithError(0, 0);

            h[0] = 1.0;
            for (int j = 1; j <= MAX; j++)
            {
                s[j - 1] = Trapezoid(min, max, j);
                if (j >= K)
                {
                    result = Polynomial.Interpolate(h, s, K, j - K, 0.0);
                    if (Math.Abs(result.Error) < TOLERANCE * result.Value) break;
                }
                s[j] = s[j - 1];
                h[j] = 0.25 * h[j - 1];
            }

            return result.Value;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// trapezoid - adapted from "Numerical Recipes in C"
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        protected double Trapezoid(double min, double max, int n)
        {
            double range = max - min;

            if (n == 1)
            {
                this._s = 0.5 * range * (this.Evaluate(min) + this.Evaluate(max));
            }
            else
            {
                int it = 1 << (n - 2);
                double delta = range / it;
                double x = min + 0.5 * delta;
                double sum = 0.0;

                for (int i = 0; i < it; i++)
                {
                    sum += this.Evaluate(x);
                    x += delta;
                }
                this._s = 0.5 * (this._s + range * sum / it);
            }

            return this._s;
        }
        #endregion
    }

    /// <summary>
    /// Stucture used to return values with associated error tolerances
    /// </summary>
    public struct ValueWithError
    {
        public double Value;
        public double Error;

        public ValueWithError(double value, double error)
        {
            this.Value = value;
            this.Error = error;
        }
    }
}