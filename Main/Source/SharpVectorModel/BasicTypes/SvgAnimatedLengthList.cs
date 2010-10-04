// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgAnimatedLengthList.
	/// </summary>
    public sealed class SvgAnimatedLengthList : ISvgAnimatedLengthList
	{
		#region Fields
		private SvgLengthList baseVal;
		private SvgLengthList animVal;
		#endregion

		#region Constructors
		public SvgAnimatedLengthList(string propertyName, string str, SvgElement ownerElement, SvgLengthDirection direction)
		{
			baseVal = new SvgLengthList(propertyName, str, ownerElement, direction);
			animVal = baseVal;
		}
		#endregion

        #region ISvgAnimatedLengthList Interface
		public ISvgLengthList BaseVal
		{
			get
			{
				return baseVal;
			}
		}

		public ISvgLengthList AnimVal
		{
			get
			{
				return animVal;
			}
		}
		#endregion
	}
}