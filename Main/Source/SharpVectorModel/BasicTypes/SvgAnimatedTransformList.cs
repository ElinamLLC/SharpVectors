using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Used for the various attributes which specify a set of transformations, such as the transform 
    /// attribute which is available for many of Svg's elements, and which can be animated.
	/// </summary>
    public sealed class SvgAnimatedTransformList : ISvgAnimatedTransformList
	{
		#region Private Fields

		private SvgTransformList _baseVal;
		private SvgTransformList _animVal;
		
        #endregion

		#region Constructors

		public SvgAnimatedTransformList(string transform)
		{
			_baseVal = new SvgTransformList(transform);
			_animVal = _baseVal;
		}

		#endregion

        #region ISvgAnimagedTransformList Interface

		public ISvgTransformList BaseVal
		{
			get { return _baseVal; }
		}

		public ISvgTransformList AnimVal
		{
			get { return _animVal; }
		}

		#endregion
	}
}