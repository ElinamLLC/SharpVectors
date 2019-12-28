namespace SharpVectors.Dom.Svg
{
	/// <summary>
	///	The basic Filter Blend Modes
	/// </summary>
	public enum SvgFilterBlendMode : ushort
    {
        /// <summary>
        /// The type is not one of predefined types. It is invalid to attempt to define a new 
        /// value of this type or to attempt to switch an existing value to this type.
        /// </summary>
		Unknown    = 0,
        /// <summary>
        /// Corresponds to value normal.
        /// </summary>
		Normal     = 1,
        /// <summary>
        /// Corresponds to value multiply.
        /// </summary>
		Multiply   = 2,
        /// <summary>
        /// Corresponds to value screen.
        /// </summary>
		Screen     = 3,
        /// <summary>
        /// Corresponds to value darken.
        /// </summary>
		Darken     = 4,
        /// <summary>
        /// Corresponds to value lighten.
        /// </summary>
		Lighten    = 5,

        // The following are introduced in SVG 2

        /// <summary>
        /// Corresponds to value overlay.
        /// </summary>
        Overlay    = 6,
        /// <summary>
        /// Corresponds to value color-dodge.
        /// </summary>
        ColorDodge = 7,
        /// <summary>
        /// Corresponds to value color-burn.
        /// </summary>
        ColorBurn  = 8,
        /// <summary>
        /// Corresponds to value hard-light.
        /// </summary>
        HardLight  = 9,
        /// <summary>
        /// Corresponds to value soft-light.
        /// </summary>
        SoftLight  = 10,
        /// <summary>
        /// Corresponds to value difference.
        /// </summary>
        Difference = 11,
        /// <summary>
        /// Corresponds to value exclusion.
        /// </summary>
        Exclusion  = 12,
        /// <summary>
        /// Corresponds to value hue.
        /// </summary>
        Hue        = 13,
        /// <summary>
        /// Corresponds to value saturation.
        /// </summary>
        Saturation = 14,
        /// <summary>
        /// Corresponds to value color.
        /// </summary>
        Color      = 15,
        /// <summary>
        /// Corresponds to value luminosity.
        /// </summary>
        Luminosity = 16
    }
}
