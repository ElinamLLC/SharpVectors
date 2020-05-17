using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The <see cref="ISvgFontFaceFormatElement"/> interface corresponds to the <c>font-face-format'</c>' element. 
    /// </summary>
    public sealed class SvgFontFaceFormatElement : SvgElement, ISvgFontFaceFormatElement
    {
        #region Constructors and Destructor

        public SvgFontFaceFormatElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFontFaceFormatElement Properties

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

        // attribute name = "string" <string>
        public string String
        {
            get { return this.GetAttribute("string"); }
            set { this.SetAttribute("string", value); }
        }

        #endregion
    }
}

