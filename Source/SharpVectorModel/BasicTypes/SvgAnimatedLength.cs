using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Used for attributes of basic type 'length' which can be animated. 
	/// </summary>
    public sealed class SvgAnimatedLength : ISvgAnimatedLength
    {
        #region Private Fields

        private SvgLength _baseVal;

        #endregion

        #region Constructors

        /// <summary>
		/// Used for attributes of basic type 'length' which can be animated. 
		/// </summary>
		/// <param name="ownerElement">The elements that contains the length</param>
		/// <param name="direction">The direction of the length, 0=x-axis, 1=y-axis, 2=no special axis</param>
		/// <param name="defaultValue">String to parse for the value</param>
		public SvgAnimatedLength(SvgElement ownerElement, string propertyName, 
            SvgLengthDirection direction, string defaultValue)
		{
			_baseVal = new SvgLength(ownerElement, propertyName, SvgLengthSource.Xml, direction, defaultValue);
		}

		public SvgAnimatedLength(SvgElement ownerElement, string propertyName, 
            SvgLengthDirection direction, string strValue, string defaultValue)
		{
			_baseVal = new SvgLength(ownerElement, propertyName, direction, strValue, defaultValue);
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
				return _baseVal;
			}
		}
		
		/// <summary>
		/// If the given attribute or property is being animated, contains the current animated value of the attribute or property, and both the object itself and its contents are readonly. If the given attribute or property is not currently being animated, contains the same value as 'baseVal'. 
		/// </summary>
		public ISvgLength AnimVal
		{
			get
			{
				return _baseVal;
			}
		}

		#endregion
	}
}
