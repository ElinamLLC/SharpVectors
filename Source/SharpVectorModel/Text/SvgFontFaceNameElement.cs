using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgFontFaceNameElement interface corresponds to the 'font-face-name' element. 
    /// </summary>
    public sealed class SvgFontFaceNameElement : SvgElement, ISvgFontFaceNameElement
    {
        #region Constructors and Destructor

        public SvgFontFaceNameElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFontFaceNameElement Properties

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

        // attribute name = "name" <string>
        public string FaceName
        {
            get { return this.GetAttribute("name"); }
            set { this.SetAttribute("name", value); }
        }

        #endregion
    }
}
