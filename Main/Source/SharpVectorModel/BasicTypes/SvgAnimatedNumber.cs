// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgAnimatedNumber.
    /// </summary>
    public sealed class SvgAnimatedNumber : ISvgAnimatedNumber
    {
		#region Private Fields
        private double baseVal;
        private double animVal;
		#endregion

		#region Constructors

        public SvgAnimatedNumber(string str)
        {
            baseVal = SvgNumber.ParseNumber(str);
            animVal = baseVal;
        }

		#endregion

        #region ISvgAnimatedNumber Interface
        public double BaseVal
        {
            get
            {
                return baseVal;
            }
            set
            {
                baseVal = value;
            }
        }

        public double AnimVal
        {
            get
            {
                return animVal;
            }
        }
		#endregion
    }
}