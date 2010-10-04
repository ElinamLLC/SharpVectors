// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgAnimatedLengthList.
    /// </summary>
    public sealed class SvgAnimatedNumberList : ISvgAnimatedNumberList
	{
        #region Fields
        private SvgNumberList baseVal;
        private SvgNumberList animVal;
        #endregion

        #region Constructor
		public SvgAnimatedNumberList(string str)
		{
			baseVal = new SvgNumberList(str);
			animVal = baseVal;
		}
        #endregion
		
        #region ISvgAnimatedNumberList Interface
		public ISvgNumberList BaseVal
		{
			get
			{
				return baseVal;
			}
		}
		
		public ISvgNumberList AnimVal
		{
			get
			{
				return animVal;
			}
		}
        #endregion
	}
}