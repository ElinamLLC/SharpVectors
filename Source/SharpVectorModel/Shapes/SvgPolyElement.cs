using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public abstract class SvgPolyElement : SvgTransformableElement, ISharpMarkerHost
    {
        #region Private Fields

        private ISvgPointList _points;
        private SvgTests _svgTests;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region Contructors and Destructor

        protected SvgPolyElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _externalResourcesRequired = new SvgExternalResourcesRequired(this);
            _svgTests                  = new SvgTests(this);
        }

        #endregion

        #region Public Properties

        public ISvgPointList AnimatedPoints
        {
            get {
                return Points;
            }
        }

        public ISvgPointList Points
        {
            get {
                if (_points == null)
                {
                    _points = new SvgPointList(GetAttribute("points"));
                }
                return _points;
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
                        _points = null;
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

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get {
                return _externalResourcesRequired.ExternalResourcesRequired;
            }
        }

        #endregion

        #region ISharpMarkerHost Members

        public virtual SvgPointF[] MarkerPositions
        {
            get {
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
            if (index < 0)
            {
                index = 1;
            }

            SvgPointF[] positions = this.MarkerPositions;

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

        public ISvgMarker GetMarker(int index)
        {
            return new SvgMarker(index, this.MarkerPositions[index]);
        }

        public virtual bool IsClosed
        {
            get {
                return false;
            }
        }

        public bool MayHaveCurves
        {
            get {
                return false;
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
