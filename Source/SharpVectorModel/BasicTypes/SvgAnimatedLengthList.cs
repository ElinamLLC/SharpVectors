using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgAnimatedLengthList.
	/// </summary>
    public sealed class SvgAnimatedLengthList : ISvgAnimatedLengthList
	{
		#region Private Fields

		private SvgLengthList _baseVal;
		private SvgLengthList _animVal;
		
        #endregion

		#region Constructors

		public SvgAnimatedLengthList(string propertyName, string str, SvgElement ownerElement, SvgLengthDirection direction)
		{
			_baseVal = new SvgLengthList(propertyName, str, ownerElement, direction);
			_animVal = _baseVal;
		}

		#endregion

        #region ISvgAnimatedLengthList Interface

		public ISvgLengthList BaseVal
		{
			get
			{
				return _baseVal;
			}
		}

		public ISvgLengthList AnimVal
		{
			get
			{
				return _animVal;
			}
		}

		#endregion
	}
}