// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgAnimatedPreserveAspectRatio.
	/// </summary>
    public sealed class SvgAnimatedPreserveAspectRatio : ISvgAnimatedPreserveAspectRatio
	{
		#region Private Fields
		private SvgPreserveAspectRatio baseVal;
		private SvgPreserveAspectRatio animVal;
 		#endregion

		#region Constructor
		public SvgAnimatedPreserveAspectRatio(string attr, SvgElement ownerElement)
		{
			baseVal = new SvgPreserveAspectRatio(attr, ownerElement);
			animVal = baseVal;
		}
		#endregion

        #region ISvgAnimatedPreserveAspectRatio Interface
		public ISvgPreserveAspectRatio BaseVal
		{
			get
			{
				return baseVal;
			}
		}

		public ISvgPreserveAspectRatio AnimVal
		{
			get
			{
				return animVal;
			}
		}
		#endregion
	}
}