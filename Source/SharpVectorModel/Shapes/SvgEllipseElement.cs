using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SvgEllipseElement class corresponds to the 'ellipse' element. 
    /// </summary>
    public sealed class SvgEllipseElement : SvgTransformableElement, ISvgEllipseElement
    {
        #region Private Fields

        private ISvgAnimatedLength _cx;
        private ISvgAnimatedLength _cy;
        private ISvgAnimatedLength _rx;
        private ISvgAnimatedLength _ry;

        private SvgTests _svgTests;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region Constructors and Destructor

        public SvgEllipseElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _externalResourcesRequired = new SvgExternalResourcesRequired(this);
            _svgTests = new SvgTests(this);
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
                    case "cx":
                        _cx = null;
                        Invalidate();
                        return;
                    case "cy":
                        _cy = null;
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
                    case CssConstants.PropColor:
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
                    case CssConstants.PropDisplay:
                    case "image-rendering":
                    case "shape-rendering":
                    case "text-rendering":
                    case CssConstants.PropVisibility:
                        Invalidate();
                        break;
                    case "transform":
                        Invalidate();
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

        #region ISvgEllipseElement Members

        public ISvgAnimatedLength Cx
        {
            get {
                if (_cx == null)
                {
                    _cx = new SvgAnimatedLength(this, "cx", SvgLengthDirection.Horizontal, SvgConstants.ValZero);
                }
                return _cx;
            }
        }

        public ISvgAnimatedLength Cy
        {
            get {
                if (_cy == null)
                {
                    _cy = new SvgAnimatedLength(this, "cy", SvgLengthDirection.Vertical, SvgConstants.ValZero);
                }
                return _cy;
            }
        }

        public ISvgAnimatedLength Rx
        {
            get {
                if (_rx == null)
                {
                    _rx = new SvgAnimatedLength(this, "rx", SvgLengthDirection.Horizontal, "100");
                }
                return _rx;
            }
        }

        public ISvgAnimatedLength Ry
        {
            get {
                if (_ry == null)
                {
                    _ry = new SvgAnimatedLength(this, "ry", SvgLengthDirection.Vertical, "100");
                }
                return _ry;
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

        public void Accept(ISvgElementVisitor visitor)
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
