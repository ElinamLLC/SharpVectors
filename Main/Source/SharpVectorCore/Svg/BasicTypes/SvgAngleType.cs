// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>
 	
using System;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	///		The basic Angle Value Types
	/// </summary>
	public enum SvgAngleType
	{
		/// <summary>
		///    The unit type is not one of predefined unit types. It is invalid to attempt to define a new value of this type or to attempt to switch an existing value to this type.
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// No unit type was provided (i.e., a unitless value was specified). For angles, a unitless value is treated the same as if degrees were specified.
		/// </summary>
		Unspecified = 1,
		/// <summary>
		///   The unit type was explicitly set to degrees.
		/// </summary>
		Deg = 11,
		/// <summary>
		/// The unit type is radians.
		/// </summary>
		Rad = 12,
		/// <summary>
		/// The unit type is grads.
		/// </summary>
		Grad = 13,
	}
}
