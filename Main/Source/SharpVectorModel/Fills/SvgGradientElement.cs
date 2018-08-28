using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public enum SvgSpreadMethod
    {
        Pad     = 0,
        Reflect = 1,
        Repeat  = 2,
        None    = 3,
    }

    public abstract class SvgGradientElement : SvgStyleableElement, ISvgGradientElement
    {
        #region Private Fields

        private ISvgAnimatedEnumeration _gradientUnits;
        private ISvgAnimatedEnumeration _spreadMethod;
        private ISvgAnimatedTransformList _gradientTransform;

        private SvgUriReference _uriReference;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region Constructors and Destructor

        protected SvgGradientElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _uriReference = new SvgUriReference(this);
            _externalResourcesRequired = new SvgExternalResourcesRequired(this);
        }

        #endregion

        #region ISvgGradientElement Members

        public ISvgAnimatedEnumeration GradientUnits
        {
            get {
                if (!HasAttribute("gradientUnits") && ReferencedElement != null)
                {
                    return ReferencedElement.GradientUnits;
                }

                if (_gradientUnits == null)
                {
                    SvgUnitType gradUnit;
                    switch (GetAttribute("gradientUnits"))
                    {
                        case "userSpaceOnUse":
                            gradUnit = SvgUnitType.UserSpaceOnUse;
                            break;
                        default:
                            gradUnit = SvgUnitType.ObjectBoundingBox;
                            break;
                    }

                    _gradientUnits = new SvgAnimatedEnumeration((ushort)gradUnit);
                }
                return _gradientUnits;
            }
        }

        public ISvgAnimatedTransformList GradientTransform
        {
            get {
                if (!HasAttribute("gradientTransform") && ReferencedElement != null)
                {
                    return ReferencedElement.GradientTransform;
                }
                if (_gradientTransform == null)
                {
                    _gradientTransform = new SvgAnimatedTransformList(GetAttribute("gradientTransform"));
                }

                return _gradientTransform;
            }
        }

        public ISvgAnimatedEnumeration SpreadMethod
        {
            get {
                if (!HasAttribute("spreadMethod") && ReferencedElement != null)
                {
                    return ReferencedElement.SpreadMethod;
                }
                if (_spreadMethod == null)
                {
                    SvgSpreadMethod spreadMeth;
                    switch (GetAttribute("spreadMethod"))
                    {
                        case "pad":
                            spreadMeth = SvgSpreadMethod.Pad;
                            break;
                        case "reflect":
                            spreadMeth = SvgSpreadMethod.Reflect;
                            break;
                        case "repeat":
                            spreadMeth = SvgSpreadMethod.Repeat;
                            break;
                        default:
                            spreadMeth = SvgSpreadMethod.Pad;
                            break;
                    }

                    _spreadMethod = new SvgAnimatedEnumeration((ushort)spreadMeth);
                }

                return _spreadMethod;
            }
        }

        #endregion

        #region ISvgURIReference Members

        public ISvgAnimatedString Href
        {
            get {
                return _uriReference.Href;
            }
        }

        public SvgGradientElement ReferencedElement
        {
            get {
                return _uriReference.ReferencedNode as SvgGradientElement;
            }
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

        #region Public Properties

        public XmlNodeList Stops
        {
            get {
                XmlNodeList stops = SelectNodes("svg:stop", OwnerDocument.NamespaceManager);
                if (stops.Count > 0)
                {
                    return stops;
                }
                // check any eventually referenced gradient
                if (ReferencedElement == null)
                {
                    // return an empty list
                    return stops;
                }
                return ReferencedElement.Stops;
            }
        }

        #endregion

        #region Update handling

        public override void HandleAttributeChange(XmlAttribute attribute)
        {
            if (attribute.NamespaceURI.Length == 0)
            {
                switch (attribute.LocalName)
                {
                    case "gradientUnits":
                        _gradientUnits = null;
                        break;
                    case "gradientTransform":
                        _gradientTransform = null;
                        break;
                    case "spreadMethod":
                        _spreadMethod = null;
                        break;
                }
            }

            base.HandleAttributeChange(attribute);
        }

        #endregion
    }
}
