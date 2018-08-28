namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Used for attributes of type DOMString which can be animated.
	/// </summary>
	public interface ISvgAnimatedString
	{
		/// <summary>
		/// The base value of the given attribute before applying 
		/// any animations.  Inheriting class should throw an
		/// exception if this value is read only.
		/// </summary>
		string BaseVal { get; set; }	

		/// <summary>
		///     If the given attribute or property is being animated,
		///     contains the current animated value of the attribute
		///     or property. If the given attribute or property is 
		///     not currently being animated, contains the same 
		///     value as 'BaseVal'.
		/// </summary>
		string AnimVal { get; }
	}
}