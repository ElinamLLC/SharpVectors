using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgColorProfileElement : SvgElement, ISvgColorProfileElement
    {
        #region Private Fields

        private SvgUriReference _svgURIReference;

        #endregion

        #region Constructors and Destructors

        public SvgColorProfileElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _svgURIReference = new SvgUriReference(this);
        }

        #endregion

        #region ISvgColorProfileElement Members

        public string Local
        {
            get
            {
                return String.Empty;
            }
        }

        #endregion

        #region ISvgURIReference Members

        public ISvgAnimatedString Href
        {
            get
            {
                return _svgURIReference.Href;
            }
        }

        public SvgUriReference UriReference
        {
            get
            {
                return _svgURIReference;
            }
        }

        public XmlElement ReferencedElement
        {
            get
            {
                return _svgURIReference.ReferencedNode as XmlElement;
            }
        }

        #endregion
    }
}
