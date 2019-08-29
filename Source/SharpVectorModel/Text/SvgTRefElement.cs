using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgTRefElement.
    /// </summary>
    public sealed class SvgTRefElement : SvgTextPositioningElement, ISvgTRefElement
    {
        private SvgUriReference _uriReference;

        public SvgTRefElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _uriReference = new SvgUriReference(this);
        }

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
    }
}
