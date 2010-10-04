// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Used for attributes of type SvgPreserveAspectRatio which can be 
	/// animated.
	/// </summary>
	public interface ISvgAnimatedPreserveAspectRatio
	{
		/// <summary>
		/// The base value of the given attribute before applying any animations.
		/// </summary>
		ISvgPreserveAspectRatio BaseVal { get; }

		/// <summary>
		///     If the given attribute or property is being animated, contains the current animated value of the attribute or property, and both the object itself and its contents are readonly. If the given attribute or property is not currently being animated, contains the same
		/// </summary>
		ISvgPreserveAspectRatio AnimVal { get; }
	}
}