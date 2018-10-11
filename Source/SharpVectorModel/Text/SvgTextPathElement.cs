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
                if (this.GetAttribute("method") == "stretch")
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
                if (this.GetAttribute("spacing") == "auto")
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
