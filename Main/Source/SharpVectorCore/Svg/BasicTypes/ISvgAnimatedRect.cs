// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Used for attributes of type SvgRect which can be animated.
	/// </summary>
	public interface ISvgAnimatedRect
	{
		/// <summary>
		/// The base value of the given attribute before applying 
		/// any animations.
		/// </summary>
		ISvgRect BaseVal { get; }

		/// <summary>
		///     If the given attribute or property is being animated,
		///     contains the current animated value of the attribute
		///     or property, and both the object itself and its 
		///     contents are readonly. If the given attribute or 
		///     property is not currently being animated, contains 
		///     the same value as 'BaseVal'.
		/// </summary>
		ISvgRect AnimVal { get; }
	}
}