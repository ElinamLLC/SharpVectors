// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgAnimatedLengthList.
	/// </summary>
    public sealed class SvgAnimatedString : ISvgAnimatedString
	{
		#region Private Fields
		private string baseVal;
		private string animVal;
		#endregion

		#region Constructor
		public SvgAnimatedString(string str)
		{
			baseVal = str;
			animVal = baseVal;
		}
		#endregion

        #region ISvgAnimatedString Interface
		public string BaseVal
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

		public string AnimVal
		{
			get
			{
				return animVal;
			}
		}
		#endregion
	}
}