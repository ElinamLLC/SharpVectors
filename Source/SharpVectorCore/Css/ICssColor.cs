namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The RGBColor interface is used to represent any RGB color value. This interface reflects the 
    /// values in the underlying style property. Hence, modifications made to the 
	/// CSSPrimitiveValue objects modify the style property. 
	/// </summary>
	public interface ICssColor
	{
        /// <summary>
        /// Gets the name of the color, if available.
        /// </summary>
        string Name { get; }

		/// <summary>
		/// This attribute is used for the alpha value of the RGBA color
		/// </summary>
		ICssPrimitiveValue Alpha { get; }

		/// <summary>
		/// This attribute is used for the red value of the RGB color
		/// </summary>
		ICssPrimitiveValue Red { get; }

		/// <summary>
		/// This attribute is used for the green value of the RGB color.
		/// </summary>
		ICssPrimitiveValue Green { get; }

		/// <summary>
		/// This attribute is used for the blue value of the RGB color
		/// </summary>
		ICssPrimitiveValue Blue { get; }

        /// <summary>
        /// Gets a value which indicates whether the color value has alpha value or not.
        /// </summary>
        bool HasAlpha { get; }

        /// <summary>
        /// Gets a value which indicates whether the color is system-defined color.
        /// </summary>
        bool IsSystemColor { get; }

        /// <summary>
        /// Gets a value which indicates whether the color is defined by custom properties.
        /// </summary>
        bool IsVarColor { get; }
	}
}
