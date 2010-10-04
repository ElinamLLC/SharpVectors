// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;
using System.Xml;

using SharpVectors.Dom.Events;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// The SVGRectElement interface corresponds to the 'rect' element. 
    /// </summary>
    public sealed class SvgRectElement : SvgTransformableElement, ISvgRectElement
    {
        #region Private Fields

        private ISvgAnimatedLength x;
        private ISvgAnimatedLength y;
        private ISvgAnimatedLength rx;
        private ISvgAnimatedLength ry;
        private ISvgAnimatedLength width;
        private ISvgAnimatedLength height;

        private SvgTests svgTests;
        private SvgExternalResourcesRequired svgExternalResourcesRequired;

        #endregion

        #region Constructors and Destructor

        public SvgRectElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
            svgTests = new SvgTests(this);
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
                        x = null;
                        Invalidate();
                        return;
                    case "y":
                        y = null;
                        Invalidate();
                        return;
                    case "width":
                        width = null;
                        Invalidate();
                        return;
                    case "height":
                        height = null;
                        Invalidate();
                        return;
                    case "rx":
                        rx = null;
                        Invalidate();
                        return;
                    case "ry":
                        ry = null;
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
            get
            {
                return SvgRenderingHint.Shape;
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

        #region ISvgRectElement Members

        public ISvgAnimatedLength Width
        {
            get
            {
                if (width == null)
                {
                    width = new SvgAnimatedLength(this, "width", SvgLengthDirection.Horizontal, "100");
                }
                return width;
            }
        }

        public ISvgAnimatedLength Height
        {
            get
            {
                if (height == null)
                {
                    height = new SvgAnimatedLength(this, "height", SvgLengthDirection.Vertical, "100");
                }
                return height;
            }

        }

        public ISvgAnimatedLength X
        {
            get
            {
                if (x == null)
                {
                    x = new SvgAnimatedLength(this, "x", SvgLengthDirection.Horizontal, "0");
                }
                return x;
            }

        }

        public ISvgAnimatedLength Y
        {
            get
            {
                if (y == null)
                {
                    y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, "0");
                }
                return y;
            }

        }

        public ISvgAnimatedLength Rx
        {
            get
            {
                if (rx == null)
                {
                    rx = new SvgAnimatedLength(this, "rx", SvgLengthDirection.Horizontal, "0");
                }
                return rx;
            }

        }

        public ISvgAnimatedLength Ry
        {
            get
            {
                if (ry == null)
                {
                    ry = new SvgAnimatedLength(this, "ry", SvgLengthDirection.Vertical, "0");
                }
                return ry;
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
    }
}
