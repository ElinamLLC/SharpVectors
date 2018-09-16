using System;

namespace SharpVectors.Polynomials
{
    /// <summary>
    /// This class overrides Polynomial's evaluate method to return the square root of that value.
    /// We need to integrate the square root of a polynomial when finding the arc length of a Bezier curve.
    /// </summary>
    public class SqrtPolynomial : Polynomial
    {
        #region Constructors

        public SqrtPolynomial(params double[] coefficients) 
            : base(coefficients)
        {
        }

        #endregion

        #region Public Methods

        public override double Evaluate(double t)
        {
            return Math.Sqrt(base.Evaluate(t));
        }

        #endregion
    }
}
