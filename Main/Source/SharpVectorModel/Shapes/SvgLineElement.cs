// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

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

        private ISvgAnimatedLength x1;
        private ISvgAnimatedLength y1;
        private ISvgAnimatedLength x2;
        private ISvgAnimatedLength y2;

        private SvgTests svgTests;
        private SvgExternalResourcesRequired svgExternalResourcesRequired;

        #endregion

        #region Constructors and Destructor

        public SvgLineElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgTests = new SvgTests(this);
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
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
                        x1 = null;
                        Invalidate();
                        return;
                    case "y1":
                        y1 = null;
                        Invalidate();
                        return;
                    case "x2":
                        x2 = null;
                        Invalidate();
                        return;
                    case "y2":
                        y2 = null;
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
            get
            {
                return SvgRenderingHint.Shape;
            }
        }

        #endregion

        #region ISvgLineElement Members

        public ISvgAnimatedLength X1
        {
            get
            {
                if (x1 == null)
                {
                    x1 = new SvgAnimatedLength(this, "x1", SvgLengthDirection.Horizontal, "0");
                }
                return x1;
            }
        }

        public ISvgAnimatedLength Y1
        {
            get
            {
                if (y1 == null)
                {
                    y1 = new SvgAnimatedLength(this, "y1", SvgLengthDirection.Vertical, "0");
                }
                return y1;
            }

        }

        public ISvgAnimatedLength X2
        {
            get
            {
                if (x2 == null)
                {
                    x2 = new SvgAnimatedLength(this, "x2", SvgLengthDirection.Horizontal, "0");
                }
                return x2;
            }
        }

        public ISvgAnimatedLength Y2
        {
            get
            {
                if (y2 == null)
                {
                    y2 = new SvgAnimatedLength(this, "y2", SvgLengthDirection.Vertical, "0");
                }
                return y2;
            }
        }

        #endregion

        #region ISvgExternalResourcesRequired Members
        
        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get
            {
                return svgExternalResourcesRequired.ExternalResourcesRequired;
            }
        }

        #endregion

        #region ISvgTests Members

        public ISvgStringList RequiredFeatures
        {
            get { return svgTests.RequiredFeatures; }
        }

        public ISvgStringList RequiredExtensions
        {
            get { return svgTests.RequiredExtensions; }
        }

        public ISvgStringList SystemLanguage
        {
            get { return svgTests.SystemLanguage; }
        }

        public bool HasExtension(string extension)
        {
            return svgTests.HasExtension(extension);
        }

        #endregion

        #region ISharpMarkerHost Members

        public SvgPointF[] MarkerPositions
        {
            get
            {
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

        #endregion
    }
}
