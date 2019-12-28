using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// An implementation of the <see cref="ISvgAnimatedInteger"/> interface.
    /// </summary>
    public sealed class SvgAnimatedInteger : ISvgAnimatedInteger
    {
        #region Private Fields

        private long _baseVal;
        private long _animVal;

        #endregion

        #region Constructors

        public SvgAnimatedInteger(string str)
        {
            _baseVal = Convert.ToInt64(SvgNumber.ParseNumber(str));
            _animVal = _baseVal;
        }

        public SvgAnimatedInteger(long value)
        {
            _baseVal = value;
            _animVal = _baseVal;
        }

        public SvgAnimatedInteger(ulong value)
        {
            _baseVal = Convert.ToInt64(value);
            _animVal = _baseVal;
        }

        public SvgAnimatedInteger(double value)
        {
            _baseVal = Convert.ToInt64(value);
            _animVal = _baseVal;
        }

        public SvgAnimatedInteger(long baseVal, long animVal)
        {
            _baseVal = baseVal;
            _animVal = animVal;
        }

        #endregion

        #region ISvgAnimatedInteger Interface

        public long BaseVal
        {
            get {
                return _baseVal;
            }
            set {
                _baseVal = value;
            }
        }

        public long AnimVal
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
