// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>
 
namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Used for attributes of basic type 'length' which can be 
	/// animated.
	/// </summary>
	public interface ISvgAnimatedLength
	{
		/// <summary>
		/// The base value of the given attribute before applying 
		/// any animations
		/// </summary>
		ISvgLength BaseVal { get; }

		/// <summary>
		///     If the given attribute or property is being animated,
		///     contains the current animated value of the attribute or property, and both the object itself and its contents are readonly. If the given attribute or property is not currently being animated, contains the same value as 'baseVal'.
		/// </summary>
		ISvgLength AnimVal { get; }
	}
}
