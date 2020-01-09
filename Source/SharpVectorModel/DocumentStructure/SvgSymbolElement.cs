using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgSymbolElement : SvgStyleableElement, ISvgSymbolElement
    {
        #region Private Fields

        private ISvgAnimatedLength _x;
        private ISvgAnimatedLength _y;
        private ISvgAnimatedLength _width;
        private ISvgAnimatedLength _height;

        private SvgFitToViewBox _fitToViewBox;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region Constructors and Destructor

        public SvgSymbolElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _externalResourcesRequired = new SvgExternalResourcesRequired(this);
            _fitToViewBox = new SvgFitToViewBox(this);
        }

        #endregion

        #region ISvgSymbolElement Members

        /// <summary>
        /// Corresponds to attribute x on the given 'svg' element.
        /// </summary>
        public ISvgAnimatedLength X
        {
            get {
                if (_x == null)
                {
                    _x = new SvgAnimatedLength(this, "x", SvgLengthDirection.Horizontal, "0px");
                }
                return _x;
            }
        }

        /// <summary>
        /// Corresponds to attribute y on the given 'svg' element.
        /// </summary>
        public ISvgAnimatedLength Y
        {
            get {
                if (_y == null)
                {
                    _y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, "0px");
                }
                return _y;
            }
        }

        private string WidthAsString
        {
            get {
                SvgWindow ownerWindow = (SvgWindow)this.OwnerDocument.Window;
                if (ownerWindow.ParentWindow == null)
                {
                    return GetAttribute("width").Trim();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Corresponds to attribute width on the given 'svg' element.
        /// </summary>
        public ISvgAnimatedLength Width
        {
            get {
                if (_width == null)
                {
                    _width = new SvgAnimatedLength(this, "width", SvgLengthDirection.Horizontal, WidthAsString, "100%");
                }
                return _width;
            }
            // Making it possible to reset this cached value internally...
            internal set {
                _width = value;
            }
        }

        private string HeightAsString
        {
            get {
                SvgWindow ownerWindow = (SvgWindow)this.OwnerDocument.Window;
                if (ownerWindow.ParentWindow == null)
                {
                    return GetAttribute("height").Trim();
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Corresponds to attribute height on the given 'svg' element.
        /// </summary>
        public ISvgAnimatedLength Height
        {
            get {
                if (_height == null)
                {
                    _height = new SvgAnimatedLength(this, "height", SvgLengthDirection.Vertical, HeightAsString, "100%");
                }
                return _height;
            }
            // Making it possible to reset this cached value internally...
            internal set {
                _height = value;
            }
        }

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
                XmlNode parentNode = this.ParentNode;
                if (parentNode != null && string.Equals(parentNode.LocalName,
                    "use", StringComparison.Ordinal))
                {
                    return true;
                }

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

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get {
                return _externalResourcesRequired.ExternalResourcesRequired;
            }
        }

        #endregion

        #region Implementation of IElementVisitorTarget

        public void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
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
    }
}
