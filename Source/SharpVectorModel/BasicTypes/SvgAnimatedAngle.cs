using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgAnimatedAngle : ISvgAnimatedAngle
	{
        #region Private Fields

        private ISvgAngle _baseVal;
        private ISvgAngle _animVal;
        
        #endregion

        #region Constructor

		public SvgAnimatedAngle(string s, string defaultValue)
		{
			_animVal = _baseVal = new SvgAngle(s, defaultValue, false);
		}

		public SvgAnimatedAngle(ISvgAngle angle)
		{
			_animVal = _baseVal = angle;
		}

        #endregion

		#region Implementation of ISvgAnimatedAngle

		public ISvgAngle BaseVal
		{
			get
			{
				return _baseVal;
			}
		}
		
		public ISvgAngle AnimVal
		{
			get
			{
				return _animVal;
			}
		}

		#endregion
	}
}