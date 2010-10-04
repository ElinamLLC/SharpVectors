using System;

namespace SharpVectors.Polynomials
{
	/// <summary>
	/// Summary description for Polynomial.
	/// </summary>
    /// <developer>kevin@kevlindev.com</developer>
    /// <completed>100</completed>
	public class Polynomial
	{
        #region fields
        private double[] coefficients;
        private double s;
        #endregion

        #region properties
        public int Degree 
        {
            get { return this.coefficients.Length - 1; }
        }

        public double this[int index] 
        {
            get { return this.coefficients[index]; }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Polynomial constuctor
        /// </summary>
        /// <param name="coefficients"></param>
		public Polynomial(params double[] coefficients)
		{
            int end = 0;
            double TOLERANCE = 1e-9;

            for ( end = coefficients.Length; end > 0; end-- )
            {
                if ( Math.Abs(coefficients[end-1]) > TOLERANCE )
                    break;
            }

            if ( end > 0 )
            {
                this.coefficients = new double[coefficients.Length - (coefficients.Length - end)];
                for ( int i = 0; i < end; i++ ) 
                {
                    this.coefficients[i] = coefficients[i];
                }
            } 
            else 
            {
                this.coefficients = new double[0];
            }
		}

        public Polynomial(Polynomial that)
        {
            this.coefficients = that.coefficients;
        }
        #endregion

        #region class methods
        /// <summary>
        /// Interpolate - adapted from "Numerical Recipes in C"
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="n"></param>
        /// <param name="offset"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        static public ValueWithError Interpolate(double[] xs, double[] ys, int n, int offset, double x) 
        {
            double y;
            double dy = 0.0;
            double[] c = new double[n];
            double[] d = new double[n];
            int ns = 0;
        
            double diff = Math.Abs(x - xs[offset]);
            for ( int i = 0; i < n; i++ ) 
            {
                double dift = Math.Abs(x - xs[offset+i]);

                if ( dift < diff ) 
                {
                    ns = i;
                    diff = dift;
                }
                c[i] = d[i] = ys[offset+i];
            }
            y = ys[offset+ns];
            ns--;

            for ( int m = 1; m < n; m++ ) 
            {
                for ( int i = 0; i < n-m; i++ ) 
                {
                    double ho = xs[offset+i] - x;
                    double hp = xs[offset+i+m] - x;
                    double w = c[i+1]-d[i];
                    double den = ho - hp;

                    if ( den == 0.0 ) return new ValueWithError(0,0);

                    den = w / den;
                    d[i] = hp*den;
                    c[i] = ho*den;
                }
                dy = (2*(ns+1) < (n-m)) ? c[ns+1] : d[ns--];
                y += dy;
            }

            return new ValueWithError(y, dy);
        }
        #endregion

        #region protected methods
        /// <summary>
        /// trapezoid - adapted from "Numerical Recipes in C"
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        protected double trapezoid(double min, double max, int n) 
        {
            double range = max - min;

            if ( n == 1 ) 
            {
                this.s = 0.5*range*(this.Evaluate(min)+this.Evaluate(max));
            } 
            else 
            {
                int it = 1 << (n-2);
                double delta = range / it;
                double x = min + 0.5*delta;
                double sum = 0.0;

                for ( int i = 0; i < it; i++ ) 
                {
                    sum += this.Evaluate(x);
                    x += delta;
                }
                this.s = 0.5*(this.s + range*sum/it);
            }

            return this.s;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Evaluate
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual double Evaluate(double t)
        {
            double result = 0.0;

            for ( int i = this.coefficients.Length - 1; i >= 0; i-- ) 
            {
                result = result * t + this.coefficients[i];
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

            for ( int j = 1; j <= MAX; j++ ) 
            {
                st = this.trapezoid(min, max, j);
                s = (4.0 * st - ost) / 3.0;
                if ( Math.Abs(s - os) < TOLERANCE*Math.Abs(os)) break;
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
            double[] s = new double[MAX+1];
            double[] h = new double[MAX+1];
            ValueWithError result = new ValueWithError(0,0);

            h[0] = 1.0;
            for ( int j = 1; j <= MAX; j++ ) 
            {
                s[j-1] = trapezoid(min, max, j);
                if ( j >= K ) 
                {
                    result = Polynomial.Interpolate(h, s, K, j-K, 0.0);
                    if ( Math.Abs(result.Error) < TOLERANCE*result.Value ) break;
                }
                s[j] = s[j-1];
                h[j] = 0.25 * h[j-1];
            }

            return result.Value;
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