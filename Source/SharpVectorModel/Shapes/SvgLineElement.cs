using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SVGLineElement interface corresponds to the 'line' element.  
    /// </summary>
    public sealed class SvgLineElement : SvgTransformableElement, ISvgLineElement, ISharpMarkerHost
    {
        #region Private Fields

        private ISvgAnimatedLength _x1;
        private ISvgAnimatedLength _y1;
        private ISvgAnimatedLength _x2;
        private ISvgAnimatedLength _y2;

        private SvgTests _svgTests;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region Constructors and Destructor

        public SvgLineElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _svgTests                  = new SvgTests(this);
            _externalResourcesRequired = new SvgExternalResourcesRequired(this);
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
                    case "x1":
                        _x1 = null;
                        Invalidate();
                        return;
                    case "y1":
                        _y1 = null;
                        Invalidate();
                        return;
                    case "x2":
                        _x2 = null;
                        Invalidate();
                        return;
                    case "y2":
                        _y2 = null;
                        Invalidate();
                        return;
                    case "marker-start":
                    case "marker-mid":
                    case "marker-end":
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

        #region ISvgLineElement Members

        public ISvgAnimatedLength X1
        {
            get {
                if (_x1 == null)
                {
                    _x1 = new SvgAnimatedLength(this, "x1", SvgLengthDirection.Horizontal, "0");
                }
                return _x1;
            }
        }

        public ISvgAnimatedLength Y1
        {
            get {
                if (_y1 == null)
                {
                    _y1 = new SvgAnimatedLength(this, "y1", SvgLengthDirection.Vertical, "0");
                }
                return _y1;
            }
        }

        public ISvgAnimatedLength X2
        {
            get {
                if (_x2 == null)
                {
                    _x2 = new SvgAnimatedLength(this, "x2", SvgLengthDirection.Horizontal, "0");
                }
                return _x2;
            }
        }

        public ISvgAnimatedLength Y2
        {
            get {
                if (_y2 == null)
                {
                    _y2 = new SvgAnimatedLength(this, "y2", SvgLengthDirection.Vertical, "0");
                }
                return _y2;
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

        #region ISharpMarkerHost Members

        public SvgPointF[] MarkerPositions
        {
            get {
                return new SvgPointF[]
                {
                    new SvgPointF(X1.AnimVal.Value, Y1.AnimVal.Value),
                    new SvgPointF(X2.AnimVal.Value, Y2.AnimVal.Value)
                };
            }
        }

        public double GetStartAngle(int index)
        {
            return 0;
        }

        public double GetEndAngle(int index)
        {
            return 0;
        }

        public ISvgMarker GetMarker(int index)
        {
            SvgPointF position = SvgPointF.Empty;
            if (index == 0)
            {
                position = new SvgPointF(X1.AnimVal.Value, Y1.AnimVal.Value);
            }
            else if (index == 1)
            {
                position = new SvgPointF(X2.AnimVal.Value, Y2.AnimVal.Value);
            }

            return new SvgMarker(index, position);
        }

        public bool IsClosed
        {
            get {
                return true;
            }
        }

        public bool MayHaveCurves
        {
            get {
                return false;
            }
        }

        #endregion
    }
}
