// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Used for attributes of basic type 'length' which can be 
	/// animated. 
	/// </summary>
    public sealed class SvgAnimatedLength : ISvgAnimatedLength
    {
        #region Private Fields

        private SvgLength baseVal;

        #endregion

        #region Constructors

        /// <summary>
		/// Used for attributes of basic type 'length' which can be animated. 
		/// </summary>
		/// <param name="stringValue">String to parse for the value</param>
		/// <param name="ownerElement">The elements that contains the length</param>
		/// <param name="direction">The direction of the length, 0=x-axis, 1=y-axis, 2=no special axis</param>
		public SvgAnimatedLength(SvgElement ownerElement, string propertyName, 
            SvgLengthDirection direction, string defaultValue)
		{
			baseVal = new SvgLength(ownerElement, propertyName, SvgLengthSource.Xml, 
                direction, defaultValue);
		}

		public SvgAnimatedLength(SvgElement ownerElement, string propertyName, 
            SvgLengthDirection direction, string strValue, string defaultValue)
		{
			baseVal = new SvgLength(ownerElement, propertyName, direction, 
                strValue, defaultValue);
		}

		#endregion

        #region ISvgAnimatedLength Interface

		/// <summary>
		/// The base value of the given attribute before applying any animations.
		/// </summary>
		public ISvgLength BaseVal
		{
			get
			{
				return baseVal;
			}
		}
		
		/// <summary>
		/// If the given attribute or property is being animated, contains the current animated value of the attribute or property, and both the object itself and its contents are readonly. If the given attribute or property is not currently being animated, contains the same value as 'baseVal'. 
		/// </summary>
		public ISvgLength AnimVal
		{
			get
			{
				return baseVal;
			}
		}

		#endregion
	}
}
