using System;

namespace SharpVectors.Polynomials
{
    /// <summary>
    /// This class overrides Polynomial's evaluate method to return the square root of that value.
    /// We need to integrate the square root of a polynomial when finding the arc length of a Bezier curve.
    /// </summary>
    public sealed class SqrtPolynomial : Polynomial
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqrtPolynomial"/> class with the specified polynomial coefficients.
        /// </summary>
        /// <param name="coefficients">The polynomial function coefficients.</param>
        public SqrtPolynomial(params double[] coefficients) 
            : base(coefficients)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Evaluates the current function at the given x-value.
        /// </summary>
        /// <param name="x">The position on the x-axis at which to evaluate polynomial function.</param>
        /// <returns>The value of the polynomial function at <paramref name="x"/>.</returns>
        public override double Evaluate(double t)
        {
            return Math.Sqrt(base.Evaluate(t));
        }

        #endregion
    }
}
