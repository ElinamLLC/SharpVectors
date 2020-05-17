using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This implements the <see cref="ISvgAltGlyphItemElement"/> interface corresponds to the 'altGlyphItem' element.
    /// </summary>
    /// <remarks>
    /// <para>*Content model:* One or more 'glyphRef' elements.</para>
    /// <para>The 'altGlyphItem' element defines a candidate set of possible glyph substitutions. 
    /// The first 'altGlyphItem' element whose referenced glyphs are all available is chosen. 
    /// Its glyphs are rendered instead of the character(s) that are inside of the referencing 'altGlyph' element.</para>
    /// </remarks>
    public sealed class SvgAltGlyphItemElement : SvgElement, ISvgAltGlyphItemElement
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgAltGlyphItemElement"/> class with the specified parameters.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="localname"></param>
        /// <param name="ns"></param>
        /// <param name="doc"></param>
        public SvgAltGlyphItemElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgAltGlyphItemElement Members

        /// <summary>
        /// Gets a value indicating whether this SVG element is renderable.
        /// </summary>
        /// <value>
        /// This is <see langword="'true"/> if the element is renderable; otherwise,
        /// it is <see langword="false"/>.
        /// </value>
        public override bool IsRenderable
        {
            get {
                return false;
            }
        }

        /// <summary>
        /// Gets the <see cref="ISvgGlyphRefElement"/> with the specified name or ID.
        /// </summary>
        /// <param name="name">The name or ID of the required <see cref="ISvgGlyphRefElement"/>.</param>
        /// <returns>
        /// A <see cref="ISvgGlyphRefElement"/> specifying the 'glyphRef' element of the specifiied name or ID.
        /// </returns>
        public ISvgGlyphRefElement GetGlyphRef(string name)
        {
            if (this.HasChildNodes)
            {
                var xpath = string.Format("./glyphRef[@id='{0}']", name);
                return this.SelectSingleNode(xpath) as SvgGlyphRefElement;
            }
            return null;
        }

        #endregion
    }
}
