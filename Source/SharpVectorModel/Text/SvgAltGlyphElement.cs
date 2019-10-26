using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The ISvgAltGlyphElement interface corresponds to the 'altGlyph' element.
    /// </summary>
    public sealed class SvgAltGlyphElement : SvgTextPositioningElement, ISvgAltGlyphElement
    {
        #region Private Fields

        private SvgUriReference _uriReference;

        #endregion

        #region Constructors and Destructor

        public SvgAltGlyphElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _uriReference = new SvgUriReference(this);
        }

        #endregion

        #region ISvgGlyphRefElement Members

        /// <summary>
        /// Corresponds to attribute 'glyphRef attribute' on the given element.
        /// </summary>
        /// <remarks>It is read only attribute</remarks>
        public string GlyphRef
        {
            get { return this.GetAttribute("glyphRef"); }
            set { this.SetAttribute("glyphRef", value); }
        }

        /// <summary>
        /// Corresponds to attribute 'format' on the given element.
        /// </summary>
        /// <remarks>It is read only attribute</remarks>
        public string Format
        {
            get { return this.GetAttribute("format"); }
            set { this.SetAttribute("format", value); }
        }

        #endregion

        #region ISvgURIReference Members

        public ISvgAnimatedString Href
        {
            get {
                return _uriReference.Href;
            }
        }

        public XmlElement ReferencedElement
        {
            get {
                return _uriReference.ReferencedNode as XmlElement;
            }
        }

        #endregion
    }
}
