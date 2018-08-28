using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPatternElement : SvgStyleableElement, ISvgPatternElement
    {
        #region Private Fields

        private ISvgAnimatedEnumeration _patternUnits;
        private ISvgAnimatedEnumeration _patternContentUnits;
        private ISvgAnimatedTransformList _patternTransform;
        private ISvgAnimatedLength _x;
        private ISvgAnimatedLength _y;
        private ISvgAnimatedLength _width;
        private ISvgAnimatedLength _height;

        private SvgUriReference _uriReference;
        private SvgExternalResourcesRequired _externalResourcesRequired;
        private SvgFitToViewBox _fitToViewBox;
        private SvgTests _svgTests;

        #endregion

        #region Constructors and Destructor

        public SvgPatternElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _uriReference = new SvgUriReference(this);
            _externalResourcesRequired = new SvgExternalResourcesRequired(this);
            _fitToViewBox = new SvgFitToViewBox(this);
            _svgTests = new SvgTests(this);
        }

        #endregion

        #region Public Properties

        public XmlNodeList Children
        {
            get {
                XmlNodeList children = SelectNodes("svg:*", OwnerDocument.NamespaceManager);
                if (children.Count > 0)
                {
                    return children;
                }
                // check any eventually referenced gradient
                if (ReferencedElement == null)
                {
                    // return an empty list
                    return children;
                }
                return ReferencedElement.Children;
            }
        }

        #endregion

        #region ISvgElement Members

        /// <summary>
        /// Gets a value indicating whether this SVG element is renderable.
        /// </summary>
        /// <value>
        /// This is <see langword="'true"/> if the element is renderable; otherwise,
        /// it is <see langword="false"/>.
        /// </value>
        public override bool IsRenderable
        {
            get {
                return false;
            }
        }

        /// <summary>
        /// Gets a value providing a hint on the rendering defined by this element.
        /// </summary>
        /// <value>
        /// An enumeration of the <see cref="SvgRenderingHint"/> specifying the rendering hint.
        /// This will always return <see cref="SvgRenderingHint.Containment"/>
        /// </value>
        public override SvgRenderingHint RenderingHint
        {
            get {
                return SvgRenderingHint.Containment;
            }
        }

        #endregion

        #region ISvgPatternElement Members

        public ISvgAnimatedEnumeration PatternUnits
        {
            get {
                if (!HasAttribute("patternUnits") && ReferencedElement != null)
                {
                    return ReferencedElement.PatternUnits;
                }
                if (_patternUnits == null)
                {
                    SvgUnitType type;
                    switch (GetAttribute("patternUnits"))
                    {
                        case "userSpaceOnUse":
                            type = SvgUnitType.UserSpaceOnUse;
                            break;
                        default:
                            type = SvgUnitType.ObjectBoundingBox;
                            break;
                    }
                    _patternUnits = new SvgAnimatedEnumeration((ushort)type);
                }
                return _patternUnits;
            }
        }

        public ISvgAnimatedEnumeration PatternContentUnits
        {
            get {
                if (!HasAttribute("patternContentUnits") && ReferencedElement != null)
                {
                    return ReferencedElement.PatternContentUnits;
                }
                if (_patternContentUnits == null)
                {
                    SvgUnitType type;
                    switch (GetAttribute("patternContentUnits"))
                    {
                        case "objectBoundingBox":
                            type = SvgUnitType.ObjectBoundingBox;
                            break;
                        default:
                            type = SvgUnitType.UserSpaceOnUse;
                            break;
                    }
                    _patternContentUnits = new SvgAnimatedEnumeration((ushort)type);
                }
                return _patternContentUnits;
            }
        }

        public ISvgAnimatedTransformList PatternTransform
        {
            get {
                if (!HasAttribute("patternTransform") && ReferencedElement != null)
                {
                    return ReferencedElement.PatternTransform;
                }
                if (_patternTransform == null)
                {
                    _patternTransform = new SvgAnimatedTransformList(GetAttribute("patternTransform"));
                }
                return _patternTransform;
            }
        }

        public ISvgAnimatedLength X
        {
            get {
                if (!HasAttribute("x") && ReferencedElement != null)
                {
                    return ReferencedElement.X;
                }
                if (_x == null)
                {
                    _x = new SvgAnimatedLength(this, "x", SvgLengthDirection.Horizontal, "0");
                }
                return _x;
            }
        }

        public ISvgAnimatedLength Y
        {
            get {
                if (!HasAttribute("y") && ReferencedElement != null)
                {
                    return ReferencedElement.Y;
                }
                if (_y == null)
                {
                    _y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, "0");
                }
                return _y;
            }
        }

        public ISvgAnimatedLength Width
        {
            get {
                if (!HasAttribute("width") && ReferencedElement != null)
                {
                    return ReferencedElement.Width;
                }
                if (_width == null)
                {
                    _width = new SvgAnimatedLength(this, "width", SvgLengthDirection.Horizontal, "0");
                }
                return _width;
            }
        }

        public ISvgAnimatedLength Height
        {
            get {
                if (!HasAttribute("height") && ReferencedElement != null)
                {
                    return ReferencedElement.Height;
                }
                if (_height == null)
                {
                    _height = new SvgAnimatedLength(this, "height", SvgLengthDirection.Vertical, "0");
                }
                return _height;
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

        public SvgPatternElement ReferencedElement
        {
            get {
                return _uriReference.ReferencedNode as SvgPatternElement;
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

        #region ISvgFitToViewBox Members

        public ISvgAnimatedRect ViewBox
        {
            get {
                return _fitToViewBox.ViewBox;
            }
        }

        public ISvgAnimatedPreserveAspectRatio PreserveAspectRatio
        {
            get {
                return _fitToViewBox.PreserveAspectRatio;
            }
        }

        #endregion

        #region ISvgTests Members

        public ISvgStringList RequiredFeatures
        {
            get { return _svgTests.RequiredFeatures; }
        }

        public ISvgStringList RequiredExtensions
        {
            get { return _svgTests.RequiredExtensions; }
        }

        public ISvgStringList SystemLanguage
        {
            get { return _svgTests.SystemLanguage; }
        }

        public bool HasExtension(string extension)
        {
            return _svgTests.HasExtension(extension);
        }

        #endregion
    }
}
