using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The implementation of the ISvgTextPathElement interface corresponds to the 'textPath' element. 
    /// </summary>
    public sealed class SvgTextPathElement : SvgTextContentElement, ISvgTextPathElement
    {
        #region Private Fields

        private SvgUriReference _uriReference;

        #endregion

        #region Constructors and Destructor

        public SvgTextPathElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _uriReference = new SvgUriReference(this);
        }

        #endregion

        #region ISvgTextPathElement Members

        public override ISvgAnimatedLength TextLength
        {
            get {
                return new SvgAnimatedLength(this, "textLength", SvgLengthDirection.Horizontal, "0");
            }
        }

        public override ISvgAnimatedEnumeration LengthAdjust
        {
            get { throw new NotImplementedException(); }
        }

        public ISvgAnimatedLength StartOffset
        {
            get {
                return new SvgAnimatedLength(this, "startOffset", SvgLengthDirection.Horizontal, "0%");
            }
        }

        public ISvgAnimatedEnumeration Method
        {
            get {
                SvgTextPathMethod pathMethod = SvgTextPathMethod.Align;
                if (string.Equals(this.GetAttribute("method"), "stretch", StringComparison.OrdinalIgnoreCase))
                {
                    pathMethod = SvgTextPathMethod.Stretch;
                }
                return new SvgAnimatedEnumeration((ushort)pathMethod);
            }
        }

        public ISvgAnimatedEnumeration Spacing
        {
            get {
                SvgTextPathSpacing pathSpacing = SvgTextPathSpacing.Exact;
                if (string.Equals(this.GetAttribute("spacing"), "auto", StringComparison.OrdinalIgnoreCase))
                {
                    pathSpacing = SvgTextPathSpacing.Auto;
                }
                return new SvgAnimatedEnumeration((ushort)pathSpacing);
            }
        }

        #endregion

        #region Implementation of IElementVisitorTarget

        public void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }

        #endregion

        #region ISvgUriReference Members

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
