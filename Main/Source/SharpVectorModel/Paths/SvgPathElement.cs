using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPathElement : SvgTransformableElement, ISvgPathElement, ISharpMarkerHost
    {
        #region Private Fields

        private ISvgAnimatedNumber _pathLength;
        private SvgTests svgTests;

        private ISvgPathSegList pathSegList;

        #endregion

        #region Constructors and Destructor

        public SvgPathElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgTests = new SvgTests(this);
        }

        #endregion

        #region Public Properties

        public string PathScript
        {
            get
            {
                return this.GetAttribute("d");
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
                    case "d":
                        pathSegList = null;
                        Invalidate();
                        return;
                    case "pathLength":
                        _pathLength = null;
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

        #region ISharpMarkerHost Members

        public SvgPointF[] MarkerPositions
        {
            get
            {
                return ((SvgPathSegList)PathSegList).Points;
            }
        }

        public double GetStartAngle(int index)
        {
            return ((SvgPathSegList)PathSegList).GetStartAngle(index);
        }

        public double GetEndAngle(int index)
        {
            return ((SvgPathSegList)PathSegList).GetEndAngle(index);
        }

        #endregion

        #region ISvgPathElement Members

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ISvgPathSegList PathSegList
        {
            get
            {
                if (pathSegList == null)
                {
                    pathSegList = new SvgPathSegList(this.GetAttribute("d"), false);
                }
                return pathSegList;
            }
        }

        public ISvgPathSegList NormalizedPathSegList
        {
            get { throw new NotImplementedException(); }
        }

        public ISvgPathSegList AnimatedPathSegList
        {
            get { return PathSegList; }
        }

        public ISvgPathSegList AnimatedNormalizedPathSegList
        {
            get { return NormalizedPathSegList; }
        }

        public ISvgAnimatedNumber PathLength
        {
            get
            {
                if (_pathLength == null)
                {
                    _pathLength = new SvgAnimatedNumber(GetAttribute("pathLength"));
                }
                return _pathLength;
            }
        }

        public double GetTotalLength()
        {
            return ((SvgPathSegList)PathSegList).GetTotalLength();
        }

        public ISvgPoint GetPointAtLength(double distance)
        {
            throw new NotImplementedException();
        }


        public int GetPathSegAtLength(double distance)
        {
            return ((SvgPathSegList)PathSegList).GetPathSegAtLength(distance);
        }

        #region Create methods

        public ISvgPathSegClosePath CreateSvgPathSegClosePath()
        {
            return new SvgPathSegClosePath();
        }

        public ISvgPathSegMovetoAbs CreateSvgPathSegMovetoAbs(double x, double y)
        {
            return new SvgPathSegMovetoAbs(x, y);
        }

        public ISvgPathSegMovetoRel CreateSvgPathSegMovetoRel(double x, double y)
        {
            return new SvgPathSegMovetoRel(x, y);
        }

        public ISvgPathSegLinetoAbs CreateSvgPathSegLinetoAbs(double x, double y)
        {
            return new SvgPathSegLinetoAbs(x, y);
        }

        public ISvgPathSegLinetoRel CreateSvgPathSegLinetoRel(double x, double y)
        {
            return new SvgPathSegLinetoRel(x, y);
        }

        public ISvgPathSegCurvetoCubicAbs CreateSvgPathSegCurvetoCubicAbs(
            double x, double y, double x1, double y1, double x2, double y2)
        {
            return new SvgPathSegCurvetoCubicAbs(x, y, x1, y1, x2, y2);
        }

        public ISvgPathSegCurvetoCubicRel CreateSvgPathSegCurvetoCubicRel(
            double x, double y, double x1, double y1, double x2, double y2)
        {
            return new SvgPathSegCurvetoCubicRel(x, y, x1, y1, x2, y2);
        }


        public ISvgPathSegCurvetoQuadraticAbs CreateSvgPathSegCurvetoQuadraticAbs(
            double x, double y, double x1, double y1)
        {
            return new SvgPathSegCurvetoQuadraticAbs(x, y, x1, y1);
        }


        public ISvgPathSegCurvetoQuadraticRel CreateSvgPathSegCurvetoQuadraticRel(
            double x, double y, double x1, double y1)
        {
            return new SvgPathSegCurvetoQuadraticRel(x, y, x1, y1);
        }                                                       

        public ISvgPathSegArcAbs CreateSvgPathSegArcAbs(double x, double y,
            double r1, double r2, double angle, bool largeArcFlag, bool sweepFlag)
        {
            return new SvgPathSegArcAbs(x, y, r1, r2, angle, largeArcFlag, sweepFlag);
        }                                                       

        public ISvgPathSegArcRel CreateSvgPathSegArcRel(double x, double y, 
            double r1, double r2, double angle, bool largeArcFlag, bool sweepFlag)
        {
            return new SvgPathSegArcRel(x, y, r1, r2, angle, largeArcFlag, sweepFlag);
        }                                                       

        public ISvgPathSegLinetoHorizontalAbs CreateSvgPathSegLinetoHorizontalAbs(double x)
        {
            return new SvgPathSegLinetoHorizontalAbs(x);
        }

        public ISvgPathSegLinetoHorizontalRel CreateSvgPathSegLinetoHorizontalRel(double x)
        {
            return new SvgPathSegLinetoHorizontalRel(x);
        }

        public ISvgPathSegLinetoVerticalAbs CreateSvgPathSegLinetoVerticalAbs(double y)
        {
            return new SvgPathSegLinetoVerticalAbs(y);
        }

        public ISvgPathSegLinetoVerticalRel CreateSvgPathSegLinetoVerticalRel(double y)
        {
            return new SvgPathSegLinetoVerticalRel(y);
        }

        public ISvgPathSegCurvetoCubicSmoothAbs CreateSvgPathSegCurvetoCubicSmoothAbs(
            double x, double y, double x2, double y2)
        {
            return new SvgPathSegCurvetoCubicSmoothAbs(x, y, x2, y2);
        }

        public ISvgPathSegCurvetoCubicSmoothRel CreateSvgPathSegCurvetoCubicSmoothRel(
            double x, double y, double x2, double y2)
        {
            return new SvgPathSegCurvetoCubicSmoothRel(x, y, x2, y2);
        }

        public ISvgPathSegCurvetoQuadraticSmoothAbs CreateSvgPathSegCurvetoQuadraticSmoothAbs(
            double x, double y)
        {
            return new SvgPathSegCurvetoQuadraticSmoothAbs(x, y);
        }

        public ISvgPathSegCurvetoQuadraticSmoothRel CreateSvgPathSegCurvetoQuadraticSmoothRel(
            double x, double y)
        {
            return new SvgPathSegCurvetoQuadraticSmoothRel(x, y);
        }
        
        #endregion

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
