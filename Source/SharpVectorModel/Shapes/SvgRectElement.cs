using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SVGRectElement interface corresponds to the 'rect' element. 
    /// </summary>
    public sealed class SvgRectElement : SvgTransformableElement, ISvgRectElement
    {
        #region Private Fields

        private ISvgAnimatedLength _x;
        private ISvgAnimatedLength _y;
        private ISvgAnimatedLength _rx;
        private ISvgAnimatedLength _ry;
        private ISvgAnimatedLength _width;
        private ISvgAnimatedLength _height;

        private SvgTests _svgTests;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region Constructors and Destructor

        public SvgRectElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _externalResourcesRequired = new SvgExternalResourcesRequired(this);
            _svgTests                  = new SvgTests(this);
        }

        #endregion

        #region Public Methods

        public void Invalidate()
        {
        }

        public override void HandleAttributeChange(XmlAttribute attribute)
        {
            if (attribute.NamespaceURI.Length == 0)
            {
                switch (attribute.LocalName)
                {
                    case "x":
                        _x = null;
                        Invalidate();
                        return;
                    case "y":
                        _y = null;
                        Invalidate();
                        return;
                    case "width":
                        _width = null;
                        Invalidate();
                        return;
                    case "height":
                        _height = null;
                        Invalidate();
                        return;
                    case "rx":
                        _rx = null;
                        Invalidate();
                        return;
                    case "ry":
                        _ry = null;
                        Invalidate();
                        return;
                    // Color.attrib, Paint.attrib 
                    case "color":
                    case "fill":
                    case "fill-rule":
                    case "stroke":
                    case "stroke-dasharray":
                    case "stroke-dashoffset":
                    case "stroke-linecap":
                    case "stroke-linejoin":
                    case "stroke-miterlimit":
                    case "stroke-width":
                    // Opacity.attrib
                    case "opacity":
                    case "stroke-opacity":
                    case "fill-opacity":
                    // Graphics.attrib
                    case "display":
                    case "image-rendering":
                    case "shape-rendering":
                    case "text-rendering":
                    case "visibility":
                        Invalidate();
                        break;
                    case "transform":
                        Invalidate();
                        break;
                    case "onclick":
                        break;
                }

                base.HandleAttributeChange(attribute);
            }
        }

        #endregion

        #region ISvgElement Members

        /// <summary>
        /// Gets a value providing a hint on the rendering defined by this element.
        /// </summary>
        /// <value>
        /// An enumeration of the <see cref="SvgRenderingHint"/> specifying the rendering hint.
        /// This will always return <see cref="SvgRenderingHint.Shape"/>
        /// </value>
        public override SvgRenderingHint RenderingHint
        {
            get {
                return SvgRenderingHint.Shape;
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

        #region ISvgRectElement Members

        public ISvgAnimatedLength Width
        {
            get {
                if (_width == null)
                {
                    _width = new SvgAnimatedLength(this, "width", SvgLengthDirection.Horizontal, "100");
                }
                return _width;
            }
        }

        public ISvgAnimatedLength Height
        {
            get {
                if (_height == null)
                {
                    _height = new SvgAnimatedLength(this, "height", SvgLengthDirection.Vertical, "100");
                }
                return _height;
            }
        }

        public ISvgAnimatedLength X
        {
            get {
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
                if (_y == null)
                {
                    _y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, "0");
                }
                return _y;
            }
        }

        public ISvgAnimatedLength Rx
        {
            get {
                if (_rx == null)
                {
                    _rx = new SvgAnimatedLength(this, "rx", SvgLengthDirection.Horizontal, "0");
                }
                return _rx;
            }
        }

        public ISvgAnimatedLength Ry
        {
            get {
                if (_ry == null)
                {
                    _ry = new SvgAnimatedLength(this, "ry", SvgLengthDirection.Vertical, "0");
                }
                return _ry;
            }
        }

        #endregion

        #region Implementation of IElementVisitorTarget

        public void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
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
