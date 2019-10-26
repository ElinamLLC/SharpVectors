using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The ISvgGlyphRefElement interface corresponds to the 'glyphRef' element. 
    /// </summary>
    public sealed class SvgGlyphRefElement : SvgStyleableElement, ISvgGlyphRefElement
    {
        #region Private Fields

        private SvgUriReference _uriReference;

        #endregion

        #region Constructors and Destructor

        public SvgGlyphRefElement(string prefix, string localname, string ns, SvgDocument doc)
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

        /// <summary>
        /// Corresponds to attribute 'x' on the given element.
        /// </summary>
        /// <remarks>It is read only attribute</remarks>
        public float X
        {
            get { return this.GetAttribute("x", 0.0f); }
            set { this.SetAttribute("x", value); }
        }

        /// <summary>
        /// Corresponds to attribute 'y' on the given element.
        /// </summary>
        /// <remarks>It is read only attribute</remarks>
        public float Y
        {
            get { return this.GetAttribute("y", 0.0f); }
            set { this.SetAttribute("y", value); }
        }

        /// <summary>
        /// Corresponds to attribute 'dx' on the given element.
        /// </summary>
        /// <remarks>It is read only attribute</remarks>
        public float Dx
        {
            get { return this.GetAttribute("dx", 0.0f); }
            set { this.SetAttribute("dx", value); }
        }

        /// <summary>
        /// Corresponds to attribute 'dy' on the given element.
        /// </summary>
        /// <remarks>It is read only attribute</remarks>
        public float Dy
        {
            get { return this.GetAttribute("dy", 0.0f); }
            set { this.SetAttribute("dy", value); }
        }

        #endregion

        #region ISvgURIReference Members

        public ISvgAnimatedString Href
        {
            get
            {
                return _uriReference.Href;
            }
        }

        public XmlElement ReferencedElement
        {
            get
            {
                return _uriReference.ReferencedNode as XmlElement;
            }
        }

        #endregion
    }
}

