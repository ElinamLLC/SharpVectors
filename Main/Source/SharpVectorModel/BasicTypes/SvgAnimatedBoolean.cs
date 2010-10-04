// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgAnimatedNumber.
	/// </summary>
    public sealed class SvgAnimatedBoolean : ISvgAnimatedBoolean
	{
		#region Private Fields
		private bool baseVal;
		private bool animVal;
		#endregion

		#region Constructor
		public SvgAnimatedBoolean(string str, bool defaultValue)
		{
            switch (str)
            {
                case "true":
                    baseVal = true;
                    break;
                case "false":
                    baseVal = false;
                    break;
                default:
                    baseVal = defaultValue;
                    break;
            }
            animVal = baseVal;
		}
		#endregion

        #region ISvgAnimatedBoolean Interface
		public bool BaseVal
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

		public bool AnimVal
		{
			get
			{
				return animVal;
			}
		}
		#endregion
	}
}
