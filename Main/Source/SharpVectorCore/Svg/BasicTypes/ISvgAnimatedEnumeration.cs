// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Used for attributes whose value must be a constant from 
	/// a particular enumeration and which can be animated.
	/// </summary>
	public interface ISvgAnimatedEnumeration
	{
		/// <summary>
		/// The base value of the given attribute before 
		/// applying any animations.  Inheriting Class should throw
		/// an exception when the value is read only
		/// </summary>
		ushort BaseVal { get; set; }

		/// <summary>
		/// If the given attribute or property is being animated, 
		/// contains the current animated value of the attribute or 
		/// property. If the given attribute or property is not 
		/// currently being animated, contains the same value as 
		/// 'BaseVal'. 
		/// </summary>
		ushort AnimVal { get; }
	}
}