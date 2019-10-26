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
    }
}

