using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The ISvgAltGlyphElement interface corresponds to the 'altGlyph' element.
    /// </summary>
    public interface ISvgAltGlyphElement : ISvgTextPositioningElement, ISvgUriReference
    {
        /// <summary>
        /// The glyph identifier, this corresponds to attribute 'glyphRef attribute' on the given element.
        /// </summary>
        /// <value>The glyph identifier, the format of which is dependent on the <see cref="Format"/> of the given font. </value>
        /// <remarks>It is read only attribute</remarks>
        string GlyphRef { get; set; }

        /// <summary>
        /// The format of the given font. This corresponds to attribute 'format' on the given element.
        /// </summary>
        /// <value>The format of the given font.</value>
        /// <remarks>It is read only attribute</remarks>
        string Format { get; set; }
    }
}
