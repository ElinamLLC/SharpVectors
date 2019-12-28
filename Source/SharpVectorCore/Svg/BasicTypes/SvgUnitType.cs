namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This defines a commonly used set of constants and is a base interface used by
    /// <see cref="ISvgGradientElement"/>, <see cref="ISvgPatternElement"/>, <see cref="ISvgClipPathElement"/>,
    /// <see cref="ISvgMaskElement"/> and <see cref="ISvgFilterElement"/>.
    /// </summary>
	public enum SvgUnitType : ushort
	{
        /// <summary>
        /// The type is not one of predefined types. It is invalid to attempt to define a 
        /// new value of this type or to attempt to switch an existing value to this type. 
        /// </summary>
		Unknown           = 0,
        /// <summary>
        /// Corresponds to value <c>'userSpaceOnUse'</c>. 
        /// </summary>
		UserSpaceOnUse = 1,
        /// <summary>
        /// Corresponds to value <c>'objectBoundingBox'</c>.
        /// </summary>
		ObjectBoundingBox = 2
	}
}