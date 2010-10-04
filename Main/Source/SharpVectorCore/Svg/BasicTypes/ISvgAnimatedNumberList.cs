using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Used for attributes which take a list of numbers and 
	/// which can be animated.
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <developer>kevin@kevlindev.com</developer>
	/// <completed>100</completed>
	public interface ISvgAnimatedNumberList
	{
		/// <summary>
		/// The base value of the given attribute before applying 
		/// any animations
		/// </summary>
		ISvgNumberList BaseVal { get; }

		/// <summary>
		///     If the given attribute or property is being animated,
		///     then this attribute contains the current animated
		///     value of the attribute or property, and both the 
		///     object itself and its contents are readonly. If the
		///     given attribute or property is not currently being
		///     animated, then this attribute contains the same 
		///     value as 'BaseVal'.
		/// </summary>
		ISvgNumberList AnimVal { get; }
	}
}