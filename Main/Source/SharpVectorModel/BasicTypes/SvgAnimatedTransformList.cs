// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Used for the various attributes which specify a set of 
	/// transformations, such as the transform attribute which is 
	/// available for many of Svg's elements, and which can be animated.
	/// </summary>
    public sealed class SvgAnimatedTransformList : ISvgAnimatedTransformList
	{
		#region Private Fields
		private SvgTransformList baseVal;
		private SvgTransformList animVal;
		#endregion

		#region Constructors
		public SvgAnimatedTransformList(string transform)
		{
			baseVal = new SvgTransformList(transform);
			animVal = baseVal;
		}
		#endregion

        #region ISvgAnimagedTransformList Interface
		public ISvgTransformList BaseVal
		{
			get { return baseVal; }
		}

		public ISvgTransformList AnimVal
		{
			get { return animVal; }
		}
		#endregion
	}
}