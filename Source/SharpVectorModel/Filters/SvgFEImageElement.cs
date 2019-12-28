using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgFEImageElement : SvgFilterPrimitiveStandardAttributes, ISvgFEImageElement
    {
        #region Private Fields

        private SvgUriReference _uriReference;
        private SvgExternalResourcesRequired _resourcesRequired;

        private ISvgAnimatedPreserveAspectRatio _preserveAspectRatio;

        #endregion

        #region Constructors

        public SvgFEImageElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _uriReference      = new SvgUriReference(this);
            _resourcesRequired = new SvgExternalResourcesRequired(this);
        }

        #endregion

        #region Implementation of ISvgURIReference

        public ISvgAnimatedString Href
        {
            get {
                return _uriReference.Href;
            }
        }

        #endregion

        #region Implementation of ISvgExternalResourcesRequired

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get {
                return _resourcesRequired.ExternalResourcesRequired;
            }
        }

        #endregion

        #region ISvgFEImageElement Members

        public ISvgAnimatedPreserveAspectRatio PreserveAspectRatio
        {
            get {
                if (_preserveAspectRatio == null)
                {
                    _preserveAspectRatio = new SvgAnimatedPreserveAspectRatio(this.GetAttribute("preserveAspectRatio"), this);
                }
                return _preserveAspectRatio;
            }
        }

        #endregion
    }
}
