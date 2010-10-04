using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public abstract class SvgPolyElement : SvgTransformableElement, ISharpMarkerHost
    {
        #region Private Fields

        private ISvgPointList points;
        private SvgTests svgTests;
        private SvgExternalResourcesRequired svgExternalResourcesRequired;

        #endregion

        #region Contructors and Destructor

        protected SvgPolyElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
            svgTests = new SvgTests(this);
        }

        #endregion

        #region Public Properties

        public ISvgPointList AnimatedPoints
        {
            get
            {
                return Points;
            }
        }

        public ISvgPointList Points
        {
            get
            {
                if (points == null)
                {
                    points = new SvgPointList(GetAttribute("points"));
                }
                return points;
            }
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
                    case "points":
                        points = null;
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

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get
            {
                return svgExternalResourcesRequired.ExternalResourcesRequired;
            }
        }

        #endregion

        #region ISharpMarkerHost Members

        public virtual SvgPointF[] MarkerPositions
        {
            get
            {
                // moved this code from SvgPointList.  This should eventually migrate into
                // the GDI+ renderer
                SvgPointF[] points = new SvgPointF[Points.NumberOfItems];

                for (uint i = 0; i < Points.NumberOfItems; i++)
                {
                    ISvgPoint point = Points.GetItem(i);
                    points[i] = new SvgPointF(point.X, point.Y);
                }

                return points;
            }
        }

        public double GetStartAngle(int index)
        {
            index--;

            SvgPointF[] positions = MarkerPositions;

            if (index > positions.Length - 1)
            {
                throw new Exception("GetStartAngle: index to large");
            }

            SvgPointF p1 = positions[index];
            SvgPointF p2 = positions[index + 1];

            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;

            double a = (Math.Atan2(dy, dx) * 180 / Math.PI);
            a -= 90;
            a %= 360;
            return a;
        }

        public double GetEndAngle(int index)
        {
            double a = GetStartAngle(index);
            a += 180;
            a %= 360;
            return a;
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
