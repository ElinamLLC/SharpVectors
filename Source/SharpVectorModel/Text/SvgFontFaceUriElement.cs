using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgFontFaceUriElement interface corresponds to the 'font-face-uri' element. 
    /// </summary>
    public sealed class SvgFontFaceUriElement : SvgElement, ISvgFontFaceUriElement
    {
        #region Constructors and Destructor

        public SvgFontFaceUriElement(string prefix, string localname, string ns, SvgDocument doc)
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

