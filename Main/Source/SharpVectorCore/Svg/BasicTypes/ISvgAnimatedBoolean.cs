namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Used for attributes of type boolean which can be animated.
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>100</completed>
	public interface ISvgAnimatedBoolean
	{
		/// <summary>
		/// The base value of the given attribute before applying
		/// any animations.  Inheriting class should throw an exception 
		/// if it is readonly.
		/// </summary>
		bool BaseVal{get;set;}
		
		/// <summary>
		/// If the given attribute or property is being animated, 
		/// contains the current animated value of the attribute or 
		/// property. If the given attribute or property is not 
		/// currently being animated, contains the same value as 
		/// 'baseVal'.
		/// </summary>
		bool AnimVal{get;}
	}
}
