using System;

namespace SharpVectors.Polynomials
{
    /// <summary>
    /// An implemenation of a polynomial function as a vector of coefficients.
    /// </summary>
    public class Polynomial
    {
        #region Public Fields

        /// <summary>
        /// The error tolerance.
        /// </summary>
        public static double Tolerance = 1e-7;

        #endregion

        #region Private Fields

        private double[] _coefficients;
        private double _s;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Polynomial"/> class with the specified polynomial coefficients.
        /// </summary>
        /// <param name="coefficients">The polynomial function coefficients.</param>
		public Polynomial(params double[] coefficients)
        {
            int end = 0;

            for (end = coefficients.Length; end > 0; end--)
            {
                if (Math.Abs(coefficients[end - 1]) > Tolerance)
                    break;
            }

            if (end > 0)
            {
                _coefficients = new double[coefficients.Length - (coefficients.Length - end)];
                for (int i = 0; i < end; i++)
                {
                    _coefficients[i] = coefficients[i];
                }
            }
            else
            {
                _coefficients = new double[0];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polynomial"/> class with an instance of the <see cref="Polynomial"/> class,
        /// a copy constructor.
        /// </summary>
        /// <param name="that"></param>
        public Polynomial(Polynomial that)
        {
            if (that != null)
            {
                _coefficients = that._coefficients;
            }
            else
            {
                _coefficients = new double[0];
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value specifying the degree of the polynomial function defined by this class.
        /// </summary>
        /// <value>An integer specifying the degree of the polynomial function.</value>
        public int Degree
        {
            get { return _coefficients.Length - 1; }
        }

        /// <summary>
        /// Gets a value of the polynomial coefficient at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns></returns>
        public double this[int index]
        {
            get { return _coefficients[index]; }
        }

        /// <summary>
        /// Gets the coefficients of the polynomial.
        /// </summary>
        /// <value>An array of double specifying the coefficients.</value>
        public double[] Coefficients
        {
            get {
                return _coefficients;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Evaluates the current function at the given x-value.
        /// </summary>
        /// <param name="x">The position on the x-axis at which to evaluate polynomial function.</param>
        /// <returns>The value of the polynomial function at <paramref name="x"/>.</returns>
        public virtual double Evaluate(double x)
        {
            double result = 0.0;

            for (int i = _coefficients.Length - 1; i >= 0; i--)
            {
                result = result * x + _coefficients[i];
            }

            return result;
        }

        /// <summary>
        /// Integrate the polynomial function from <paramref name="min"/> to <paramref name="min"/> using the Simspon rule.
        /// </summary>
        /// <param name="min">The lower limit of the integrate range.</param>
        /// <param name="max">The upper limit of the integrate range.</param>
        /// <returns></returns>
        /// <remarks>Simspon - adapted from "Numerical Recipes in C"</remarks>
        public double Simpson(double min, double max)
        {
            double s     = 0.0;
            double st    = 0.0;
            double os    = 0.0;
            double ost   = 0.0;
            int maxCount = 20;

            for (int j = 1; j <= maxCount; j++)
            {
                st = this.Trapezoid(min, max, j);
                s = (4.0 * st - ost) / 3.0;
                if (Math.Abs(s - os) < Tolerance * Math.Abs(os)) break;
                os = s;
                ost = st;
            }

            return s;
        }

        /// <summary>
        /// Integrate the polynomial function from <paramref name="min"/> to <paramref name="min"/> using the Romberg rule.
        /// </summary>
        /// <param name="min">The lower limit of the integrate range.</param>
        /// <param name="max">The upper limit of the integrate range.</param>
        /// <returns></returns>
        /// <remarks>Romberg - adapted from "Numerical Recipes in C"</remarks>
        public double Romberg(double min, double max)
        {
            int maxCount = 20;
            int K = 4;
            double[] s = new double[maxCount + 1];
            double[] h = new double[maxCount + 1];
            Tuple<double, double> result = Tuple.Create(0d, 0d);

            h[0] = 1.0;
            for (int j = 1; j <= maxCount; j++)
            {
                s[j - 1] = Trapezoid(min, max, j);
                if (j >= K)
                {
                    result = Polynomial.Interpolate(h, s, K, j - K, 0.0);
                    if (Math.Abs(result.Item2) < Tolerance * result.Item1) break;
                }
                s[j] = s[j - 1];
                h[j] = 0.25 * h[j - 1];
            }

            return result.Item1;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Integrate the polynomial function from <paramref name="min"/> to <paramref name="min"/> using the trapezoidal rule.
        /// </summary>
        /// <param name="min">The lower limit of the integrate range.</param>
        /// <param name="max">The upper limit of the integrate range.</param>
        /// <param name="n">The number of trapezoid.</param>
        /// <returns>The computed numerical integration result over the specified interval.</returns>
        /// <remarks>trapezoid - adapted from "Numerical Recipes in C"</remarks>
        protected double Trapezoid(double min, double max, int n)
        {
            double range = max - min;

            if (n == 1)
            {
                _s = 0.5 * range * (this.Evaluate(min) + this.Evaluate(max));
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
                _s = 0.5 * (_s + range * sum / it);
            }

            return _s;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="n"></param>
        /// <param name="offset"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        /// <remarks>Interpolate - adapted from "Numerical Recipes in C".</remarks>
        private static Tuple<double, double> Interpolate(double[] xs, double[] ys, int n, int offset, double x)
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

                    if (den.Equals(0.0)) return Tuple.Create(0.0d, 0.0d);

                    den = w / den;
                    d[i] = hp * den;
                    c[i] = ho * den;
                }
                dy = (2 * (ns + 1) < (n - m)) ? c[ns + 1] : d[ns--];
                y += dy;
            }

            return Tuple.Create(y, dy);
        }

        #endregion
    }
}