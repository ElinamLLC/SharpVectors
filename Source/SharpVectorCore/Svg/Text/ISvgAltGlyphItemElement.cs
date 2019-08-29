using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The ISvgAltGlyphItemElement interface corresponds to the 'altGlyphItem' element.
    /// </summary>
    /// <remarks>
    /// <para>*Content model:* One or more 'glyphRef' elements.</para>
    /// <para>The 'altGlyphItem' element defines a candidate set of possible glyph substitutions. 
    /// The first 'altGlyphItem' element whose referenced glyphs are all available is chosen. 
    /// Its glyphs are rendered instead of the character(s) that are inside of the referencing 'altGlyph' element.</para>
    /// </remarks>
    public interface ISvgAltGlyphItemElement : ISvgElement
    {
        // **** Extended properties and methods to support the content model ****

        /// <summary>
        /// Gets the <see cref="ISvgGlyphRefElement"/> with the specified name or ID.
        /// </summary>
        /// <param name="name">The name or ID of the required <see cref="ISvgGlyphRefElement"/>.</param>
        /// <returns>
        /// A <see cref="ISvgGlyphRefElement"/> specifying the 'glyphRef' element of the specifiied name or ID.
        /// </returns>
        ISvgGlyphRefElement GetGlyphRef(string name);
    }
}
