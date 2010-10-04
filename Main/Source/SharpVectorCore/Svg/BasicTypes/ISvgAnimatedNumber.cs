using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Used for attributes of basic type 'number' which can be 
	/// animated.
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <developer>kevin@kevlindev.com</developer>
	/// <completed>100</completed>
	public interface ISvgAnimatedNumber 
	{
		/// <summary>
		/// The base value of the given attribute before applying 
		/// any animations.  Inheriting class should throw an
		/// exception if the value is read only.
		/// </summary>
		double BaseVal { get; set; }

		/// <summary>
		///     If the given attribute or property is being animated,
		///     contains the current animated value of the attribute
		///     or property, and both the object itself and its 
		///     contents are readonly. If the given attribute or 
		///     property is not currently being animated, contains 
		///     the same value as 'BaseVal'.
		/// </summary>
		double AnimVal { get; }
	}
}