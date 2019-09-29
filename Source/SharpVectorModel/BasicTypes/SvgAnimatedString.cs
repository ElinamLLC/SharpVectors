using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgAnimatedString.
    /// </summary>
    public sealed class SvgAnimatedString : ISvgAnimatedString
    {
        #region Private Fields

        private string _baseVal;
        private string _animVal;

        #endregion

        #region Constructor

        public SvgAnimatedString(string str)
        {
            _baseVal = str;
            _animVal = _baseVal;
        }

        #endregion

        #region ISvgAnimatedString Interface

        public string BaseVal
        {
            get {
                return _baseVal;
            }
            set {
                _baseVal = value;
            }
        }

        public string AnimVal
        {
            get {
                return _animVal;
            }
        }

        #endregion
    }
}