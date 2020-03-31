using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgMarkerElement : SvgStyleableElement, ISvgMarkerElement
    {
        #region Private Fields

        private ISvgAnimatedLength _refY;
        private ISvgAnimatedLength _refX;
        private ISvgAnimatedEnumeration _markerUnits;
        private ISvgAnimatedLength _markerWidth;
        private ISvgAnimatedLength _markerHeight;
        private ISvgAnimatedEnumeration _orientType;
        private ISvgAnimatedAngle _orientAngle;
        private SvgFitToViewBox _fitToViewBox;
        private SvgExternalResourcesRequired _externalResourcesRequired;

        #endregion

        #region Constructors and Destructor

        public SvgMarkerElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _externalResourcesRequired = new SvgExternalResourcesRequired(this);
            _fitToViewBox = new SvgFitToViewBox(this);
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
        /// This will always return <see cref="SvgRenderingHint.Marker"/>
        /// </value>
        public override SvgRenderingHint RenderingHint
        {
            get {
                return SvgRenderingHint.Marker;
            }
        }

        #endregion

        #region ISvgMarkerElement Members

        /// <summary>
        ///  Sets the value of attribute orient to 'auto'.
        /// </summary>
        public void SetOrientToAuto()
        {
            _orientType = null;
            SetAttribute("orient", "auto");
        }

        /// <summary>
        ///  Sets the value of attribute orient to the given angle.
        /// </summary>
        /// <param name="angle"> The angle value to use for attribute orient.</param>
        public void SetOrientToAngle(ISvgAngle angle)
        {
            _orientType = null;
            SetAttribute("orient", angle.ValueAsString);
            _orientAngle = new SvgAnimatedAngle(angle);
        }

        /// <summary>
        /// Corresponds to attribute refX on the given 'marker' element.
        /// </summary>
        public ISvgAnimatedLength RefX
        {
            get {
                if (_refX == null)
                {
                    _refX = new SvgAnimatedLength(this, "refX", SvgLengthDirection.Horizontal, "0");
                }
                return _refX;
            }
        }

        /// <summary>
        /// Corresponds to attribute refY on the given 'marker' element.
        /// </summary>
        public ISvgAnimatedLength RefY
        {
            get {
                if (_refY == null)
                {
                    _refY = new SvgAnimatedLength(this, "refY", SvgLengthDirection.Vertical, "0");
                }
                return _refY;
            }
        }

        /// <summary>
        /// Corresponds to attribute markerUnits on the given 'marker' element.
        /// </summary>
        public ISvgAnimatedEnumeration MarkerUnits
        {
            get {
                if (_markerUnits == null)
                {
                    SvgMarkerUnit type = SvgMarkerUnit.Unknown;
                    switch (GetAttribute("markerUnits"))
                    {
                        case "userSpaceOnUse":
                            type = SvgMarkerUnit.UserSpaceOnUse;
                            break;
                        case "":
                        case "strokeWidth":
                            type = SvgMarkerUnit.StrokeWidth;
                            break;
                    }
                    _markerUnits = new SvgAnimatedEnumeration((ushort)type);
                }
                return _markerUnits;
            }
        }

        /// <summary>
        /// Corresponds to attribute markerWidth on the given 'marker' element
        /// </summary>
        public ISvgAnimatedLength MarkerWidth
        {
            get {
                if (_markerWidth == null)
                {
                    _markerWidth = new SvgAnimatedLength(this, "markerWidth", SvgLengthDirection.Horizontal, "3");
                }
                return _markerWidth;
            }
        }

        /// <summary>
        /// Corresponds to attribute markerHeight on the given 'marker' element.
        /// </summary>
        public ISvgAnimatedLength MarkerHeight
        {
            get {
                if (_markerHeight == null)
                {
                    _markerHeight = new SvgAnimatedLength(this, "markerHeight", SvgLengthDirection.Vertical, "3");
                }
                return _markerHeight;
            }
        }

        /// <summary>
        /// Corresponds to attribute orient on the given 'marker' element. One of the Marker Orientation Types defined above.
        /// </summary>
        public ISvgAnimatedEnumeration OrientType
        {
            get {
                if (_orientType == null)
                {
                    string orientAttr = GetAttribute("orient");
                    if (orientAttr.Equals("auto", StringComparison.OrdinalIgnoreCase))
                    {
                        _orientType = new SvgAnimatedEnumeration((ushort)SvgMarkerOrient.Auto);
                    }
                    else if (orientAttr.Equals("none", StringComparison.OrdinalIgnoreCase))
                    {
                        _orientType = new SvgAnimatedEnumeration((ushort)SvgMarkerOrient.Unknown);
                    }
                    else if (orientAttr.Equals("auto-start-reverse", StringComparison.OrdinalIgnoreCase))
                    {
                        _orientType = new SvgAnimatedEnumeration((ushort)SvgMarkerOrient.AutoStartReverse);
                    }
                    else
                    {
                        _orientType = new SvgAnimatedEnumeration((ushort)SvgMarkerOrient.Angle);
                    }
                }
                return _orientType;
            }
        }

        /// <summary>
        /// Corresponds to attribute orient on the given 'marker' element. If markerUnits is SVG_MARKER_ORIENT_ANGLE, the angle value for attribute orient; otherwise, it will be set to zero.
        /// </summary>
        public ISvgAnimatedAngle OrientAngle
        {
            get {
                if (_orientAngle == null)
                {
                    if (OrientType.AnimVal.Equals((ushort)SvgMarkerOrient.Angle))
                    {
                        _orientAngle = new SvgAnimatedAngle(GetAttribute("orient"), "0");
                    }
                    else if (OrientType.AnimVal.Equals((ushort)SvgMarkerOrient.AutoStartReverse))
                    {
                        _orientAngle = new SvgAnimatedAngle("180", "0");
                    }
                    else
                    {
                        _orientAngle = new SvgAnimatedAngle("0", "0");
                    }
                }
                return _orientAngle;
            }
        }

        public bool IsSizeDefined
        {
            get {
                return this.HasAttribute("markerHeight") && this.HasAttribute("markerWidth");
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

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get {
                return _externalResourcesRequired.ExternalResourcesRequired;
            }
        }

        #endregion
    }
}
