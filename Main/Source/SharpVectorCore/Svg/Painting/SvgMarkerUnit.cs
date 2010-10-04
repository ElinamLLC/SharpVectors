using System;

namespace SharpVectors.Dom.Svg
{
	public enum SvgMarkerUnit
	{
		/// <summary>
		/// The marker unit type is not one of predefined types. It is invalid to attempt to define a new value of this type or to attempt to switch an existing value to this type.
		/// </summary>
		Unknown,

		/// <summary>
		/// The value of attribute markerUnits is 'userSpaceOnUse'.
		/// </summary>
		UserSpaceOnUse,

		/// <summary>
		/// The value of attribute markerUnits is 'strokeWidth'.
		/// </summary>
		StrokeWidth
	}
}
