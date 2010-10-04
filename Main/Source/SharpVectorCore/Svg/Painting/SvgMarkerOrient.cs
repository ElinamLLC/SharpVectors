using System;

namespace SharpVectors.Dom.Svg
{
	public enum SvgMarkerOrient
	{
		/// <summary>
		/// The marker orientation is not one of predefined types. It is invalid to attempt to define a new value of this type or to attempt to switch an existing value to this type.
		/// </summary>
		Unknown,
		/// <summary>
		/// Attribute orient has value 'auto'.
		/// </summary>
		Auto,
		/// <summary>
		/// Attribute orient has an angle value.
		/// </summary>
		Angle
	}
}
