using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgAnimatedNumber.
    /// </summary>
    public sealed class SvgAnimatedNumber : ISvgAnimatedNumber
    {
        #region Private Fields

        private double _baseVal;
        private double _animVal;

        #endregion

        #region Constructors

        public SvgAnimatedNumber(string str)
        {
            _baseVal = SvgNumber.ParseNumber(str);
            _animVal = _baseVal;
        }

        public SvgAnimatedNumber(double value)
        {
            _baseVal = value;
            _animVal = value;
        }

        public SvgAnimatedNumber(double baseVal, double animVal)
        {
            _baseVal = baseVal;
            _animVal = animVal;
        }

        #endregion

        #region ISvgAnimatedNumber Interface

        public double BaseVal
        {
            get {
                return _baseVal;
            }
            set {
                _baseVal = value;
            }
        }

        public double AnimVal
        {
            get {
                return _animVal;
            }
            set {
                _animVal = value;
            }
        }

        #endregion
    }
}