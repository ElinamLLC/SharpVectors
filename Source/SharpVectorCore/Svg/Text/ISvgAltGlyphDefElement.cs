using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The ISvgAltGlyphDefElement interface corresponds to the 'altGlyphDef' element.
    /// </summary>
    /// <remarks>
    /// *Content model:* Either
    /// <list type="bullet">
    /// <item><term>one or more 'glyphRef' elements, or</term>
    /// <description>In the simplest case, an 'altGlyphDef' contains one or more 'glyphRef' elements. 
    /// Each 'glyphRef' element references a single glyph within a particular font.
    /// </description>
    /// </item>
    /// <item><term>one or more 'altGlyphItem' elements.</term>
    /// <description>In the more complex case, an 'altGlyphDef' contains one or more 'altGlyphItem' elements. 
    /// Each 'altGlyphItem' represents a candidate set of substitute glyphs. Each 'altGlyphItem' contains 
    /// one or more 'glyphRef' elements. Each 'glyphRef' element references a single glyph within a particular font. 
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    public interface ISvgAltGlyphDefElement : ISvgElement
    {
        // **** Extended properties and methods to support the content model ****

        /// <summary>
        /// Gets a value indicating whether this is a simple content model.
        /// </summary>
        /// <value>
        /// Returns <see langword="true"/> if this element defines a simple content model that contains only 'glyphRef'
        /// elements, otherwise returns <see langword="false"/>.
        /// </value>
        bool IsSimple { get; }

        /// <summary>
        /// Gets the <see cref="ISvgGlyphRefElement"/> with the specified name or ID.
        /// </summary>
        /// <param name="name">The name or ID of the required <see cref="ISvgGlyphRefElement"/>.</param>
        /// <returns>
        /// A <see cref="ISvgGlyphRefElement"/> specifying the 'glyphRef' element of the specifiied name or ID.
        /// <para>
        /// This will always return <see langword="null"/>, if the <see cref="IsSimple"/> is <see langword="false"/>.
        /// </para>
        /// </returns>
        ISvgGlyphRefElement GetGlyphRef(string name);

        /// <summary>
        /// Gets the <see cref="ISvgAltGlyphItemElement"/> with the specified name or ID.
        /// </summary>
        /// <param name="name">The name or ID of the required <see cref="ISvgAltGlyphItemElement"/>.</param>
        /// <returns>
        /// A <see cref="ISvgAltGlyphItemElement"/> specifying the 'altGlyphItem' element of the specifiied name or ID.
        /// <para>
        /// This will always return <see langword="null"/>, if the <see cref="IsSimple"/> is <see langword="true"/>.
        /// </para>
        /// </returns>
        ISvgAltGlyphItemElement GetGlyphItem(string name);
    }
}
