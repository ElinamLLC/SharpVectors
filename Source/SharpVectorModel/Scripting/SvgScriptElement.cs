using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SVGScriptElement interface corresponds to the 'script' element.
    /// </summary>
    public sealed class SvgScriptElement : SvgElement, ISvgScriptElement
    {
        #region Private Fields

        private SvgUriReference _uriReference;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region Constructors

        public SvgScriptElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _uriReference              = new SvgUriReference(this);
            _externalResourcesRequired = new SvgExternalResourcesRequired(this);
        }

        #endregion

        #region ISvgURIReference Members

        public string Type
        {
            get {
                return GetAttribute("type");
            }
            set {
                SetAttribute("type", value);
            }
        }

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
                return _externalResourcesRequired.ExternalResourcesRequired;
            }
        }

        #endregion
    }
}
