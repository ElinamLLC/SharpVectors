using System;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Summary description for SvgFilterElement.
    /// </summary>
    public sealed class SvgFilterElement : SvgStyleableElement, ISvgFilterElement
    {
        #region Private Fields

        private SvgUriReference _uriReference;
        private SvgExternalResourcesRequired _resourcesRequired;

        #endregion

        #region Constructors

        public SvgFilterElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _uriReference      = new SvgUriReference(this);
            _resourcesRequired = new SvgExternalResourcesRequired(this);
        }

        #endregion

        #region ISvgFilterElement Interface

        public ISvgAnimatedEnumeration FilterUnits
        {
            get {
                return null;
            }
        }

        public ISvgAnimatedEnumeration PrimitiveUnits
        {
            get {
                return null;
            }
        }

        public ISvgAnimatedLength X
        {
            get {
                return null;
            }
        }

        public ISvgAnimatedLength Y
        {
            get {
                return null;
            }
        }

        public ISvgAnimatedLength Width
        {
            get {
                return null;
            }
        }

        public ISvgAnimatedLength Height
        {
            get {
                return null;
            }
        }

        public ISvgAnimatedInteger FilterResX
        {
            get {
                return null;
            }
        }

        public ISvgAnimatedInteger FilterResY
        {
            get {
                return null;
            }
        }

        public void SetFilterRes(ulong filterResX, ulong filterResY)
        {

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
    }
}
