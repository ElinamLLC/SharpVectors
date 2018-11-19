using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgAnimateMPathElement : SvgElement, ISvgAnimateMPathElement
    {
        #region Protected Fields

        private SvgUriReference _uriReference;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region Constructors and Destructors

        public SvgAnimateMPathElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _uriReference              = new SvgUriReference(this);
            _externalResourcesRequired = new SvgExternalResourcesRequired(this);
        }

        #endregion

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get {
                return _externalResourcesRequired.ExternalResourcesRequired;
            }
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
