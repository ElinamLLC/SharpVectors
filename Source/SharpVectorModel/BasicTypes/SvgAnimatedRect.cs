using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// 
	/// </summary>
    public sealed class SvgAnimatedRect : ISvgAnimatedRect
	{
		#region Private Fields

		private SvgRect _baseVal;
		private SvgRect _animVal;
		
        #endregion

		#region Constructors

		public SvgAnimatedRect(string str)
		{
			_baseVal = new SvgRect(str);
			_animVal = _baseVal;
		}

		public SvgAnimatedRect(SvgRect rect)
		{
			_baseVal = rect;
			_animVal = _baseVal;
		}

		#endregion

        #region ISvgAnimatedRect Interface

		public ISvgRect BaseVal
		{
			get
			{
				return _baseVal;
			}
		}
		
		public ISvgRect AnimVal
		{
			get
			{
				return _animVal;
			}
		}

		#endregion
	}
}