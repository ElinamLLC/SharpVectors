using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgMissingGlyphElement interface corresponds to the 'missing-glyph' element. 
    /// </summary>
    public sealed class SvgMissingGlyphElement : SvgGlyphElement, ISvgMissingGlyphElement
    {
        #region Constructors and Destructor

        public SvgMissingGlyphElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

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
    }
}

