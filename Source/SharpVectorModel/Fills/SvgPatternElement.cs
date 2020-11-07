using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgPatternElement : SvgStyleableElement, ISvgPatternElement
    {
        #region Private Fields

        private ISvgAnimatedEnumeration _patternUnits;
        private ISvgAnimatedEnumeration _patternContentUnits;
        private ISvgAnimatedTransformList _patternTransform;
        private ISvgAnimatedLength _x;
        private ISvgAnimatedLength _y;
        private ISvgAnimatedLength _width;
        private ISvgAnimatedLength _height;

        private SvgUriReference _uriReference;
        private SvgExternalResourcesRequired _externalResRequired;
        private SvgFitToViewBox _fitToViewBox;
        private SvgTests _svgTests;

        private ISvgViewSpec _currentView;
        private ISvgMatrix _cachedViewBoxTransform;

        private ISvgRect _patternBounds;

        #endregion

        #region Constructors and Destructor

        public SvgPatternElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _uriReference        = new SvgUriReference(this);
            _externalResRequired = new SvgExternalResourcesRequired(this);
            _fitToViewBox        = new SvgFitToViewBox(this);
            _svgTests            = new SvgTests(this);
        }

        #endregion

        #region Public Properties

        public XmlNodeList Children
        {
            get {
                XmlNodeList children = SelectNodes("svg:*", OwnerDocument.NamespaceManager);
                if (children.Count > 0)
                {
                    return children;
                }
                // check any eventually referenced gradient
                if (ReferencedElement == null)
                {
                    // return an empty list
                    return children;
                }
                return ReferencedElement.Children;
            }
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
        /// This will always return <see cref="SvgRenderingHint.Containment"/>
        /// </value>
        public override SvgRenderingHint RenderingHint
        {
            get {
                return SvgRenderingHint.Containment;
            }
        }

        #endregion

        #region ISvgPatternElement Members

        public ISvgAnimatedEnumeration PatternUnits
        {
            get {                
                if (!this.HasAttribute(SvgConstants.AttrPatternUnits) && ReferencedElement != null)
                {
                    return ReferencedElement.PatternUnits;
                }
                if (_patternUnits == null)
                {
                    SvgUnitType type = SvgUnitType.ObjectBoundingBox;
                    if (string.Equals(this.GetAttribute(SvgConstants.AttrPatternUnits), 
                        SvgConstants.ValUserSpaceOnUse, StringComparison.Ordinal))
                    {
                        type = SvgUnitType.UserSpaceOnUse;
                    }
                    _patternUnits = new SvgAnimatedEnumeration((ushort)type);
                }
                return _patternUnits;
            }
        }

        public ISvgAnimatedEnumeration PatternContentUnits
        {
            get {                
                if (!this.HasAttribute(SvgConstants.AttrPatternContentUnits) && ReferencedElement != null)
                {
                    return ReferencedElement.PatternContentUnits;
                }
                if (_patternContentUnits == null)
                {                    
                    SvgUnitType type = SvgUnitType.UserSpaceOnUse;
                    
                    if (string.Equals(this.GetAttribute(SvgConstants.AttrPatternContentUnits), 
                        SvgConstants.ValObjectBoundingBox, StringComparison.Ordinal))
                    {
                        type = SvgUnitType.ObjectBoundingBox;
                    }
                    _patternContentUnits = new SvgAnimatedEnumeration((ushort)type);
                }
                return _patternContentUnits;
            }
        }

        public ISvgAnimatedTransformList PatternTransform
        {
            get {                
                if (!this.HasAttribute(SvgConstants.AttrPatternTransform) && ReferencedElement != null)
                {
                    return ReferencedElement.PatternTransform;
                }
                if (_patternTransform == null)
                {
                    _patternTransform = new SvgAnimatedTransformList(this.GetAttribute(SvgConstants.AttrPatternTransform));
                }
                return _patternTransform;
            }
        }

        public ISvgAnimatedLength X
        {
            get {
                if (!this.HasAttribute("x") && ReferencedElement != null)
                {
                    return ReferencedElement.X;
                }
                if (_x == null)
                {
                    _x = new SvgAnimatedLength(this, "x", SvgLengthDirection.Horizontal, SvgConstants.ValZero);
                }
                return _x;
            }
        }

        public ISvgAnimatedLength Y
        {
            get {
                if (!this.HasAttribute("y") && ReferencedElement != null)
                {
                    return ReferencedElement.Y;
                }
                if (_y == null)
                {
                    _y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, SvgConstants.ValZero);
                }
                return _y;
            }
        }

        public ISvgAnimatedLength Width
        {
            get {
                if (!this.HasAttribute("width") && ReferencedElement != null)
                {
                    return ReferencedElement.Width;
                }
                if (_width == null)
                {
                    _width = new SvgAnimatedLength(this, "width", SvgLengthDirection.Horizontal, SvgConstants.ValZero);
                }
                return _width;
            }
        }

        public ISvgAnimatedLength Height
        {
            get {
                if (!this.HasAttribute("height") && ReferencedElement != null)
                {
                    return ReferencedElement.Height;
                }
                if (_height == null)
                {
                    _height = new SvgAnimatedLength(this, "height", SvgLengthDirection.Vertical, SvgConstants.ValZero);
                }
                return _height;
            }
        }

        public ISvgRect PatternBounds 
        { 
            get {
                return _patternBounds;
            }
            set {
                if (_patternBounds != null && value != null)
                {
                    if (!_patternBounds.Equals(value))
                    {
                        _cachedViewBoxTransform = null;
                    }
                }
                _patternBounds = value;
            }
        }

        #endregion

        #region ISvgURIReference Members

        public ISvgAnimatedString Href
        {
            get {
                return _uriReference.Href;
            }
        }

        public SvgPatternElement ReferencedElement
        {
            get {
                return _uriReference.ReferencedNode as SvgPatternElement;
            }
        }

        #endregion

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get {
                return _externalResRequired.ExternalResourcesRequired;
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

        /// <summary>
        /// This function is super useful, calculates out the transformation matrix 
        /// (i.e., scale and translate) of the viewport to user space.
        /// </summary>
        /// <returns>A Matrix which has the translate and scale portions set.</returns>
        public ISvgMatrix ViewBoxTransform
        {
            // TODO: This needs to be cached... need to handle changes to
            //   parent width or height or viewbox changes (in the case of percents)
            //   x,y,width,height,viewBox,preserveAspectRatio changes
            get {
                if (_cachedViewBoxTransform == null)
                {
                    ISvgMatrix matrix = new SvgMatrix();

                    double x = 0;
                    double y = 0;
                    double w = 0;
                    double h = 0;

                    var widthLength   = this.Width.AnimVal;
                    var heightLength  = this.Height.AnimVal;
                    double attrWidth  = widthLength.Value;
                    double attrHeight = heightLength.Value;
                    if (this.HasAttribute(SvgConstants.AttrX) && this.HasAttribute(SvgConstants.AttrY))
                    {
                        // X and Y on the root <svg> have no meaning
                        ISvgLength xLength = this.X.AnimVal;
                        ISvgLength yLength = this.Y.AnimVal;
                        double xValue = xLength.Value;
                        double yValue = yLength.Value;

                        if (xLength.UnitType == SvgLengthType.Percentage)
                        {
                            xValue = xLength.ValueInSpecifiedUnits / 100.0;
                        }
                        if (yLength.UnitType == SvgLengthType.Percentage)
                        {
                            yValue = yLength.ValueInSpecifiedUnits / 100.0;
                        }
                        matrix = matrix.Translate(xValue, yValue);
                    }

                    // Apply the viewBox viewport
                    if (this.HasAttribute(SvgConstants.AttrViewBox))
                    {
                        ISvgRect r = CurrentView.ViewBox.AnimVal;
                        x += -r.X;
                        y += -r.Y;
                        w = r.Width;
                        h = r.Height;
                        if (w < 0 || h < 0)
                            throw new SvgException(SvgExceptionType.SvgInvalidValueErr, "Negative values are not permitted for viewbox width or height");
                    }
                    else
                    {
                        // This will result in a 1/1 scale.
                        w = attrWidth;
                        h = attrHeight;
                    }

                    double x_ratio = 1;
                    double y_ratio = 1;
                    if (w > 0 && h > 0)
                    {
                        x_ratio = attrWidth / w;
                        y_ratio = attrHeight / h;
                    }

                    ISvgPreserveAspectRatio par = this.CurrentView.PreserveAspectRatio.AnimVal;
                    if (par.Align == SvgPreserveAspectRatioType.None)
                    {
                        matrix = matrix.ScaleNonUniform(x_ratio, y_ratio);
                    }
                    else
                    {
                        // uniform scaling
                        if (par.MeetOrSlice == SvgMeetOrSlice.Meet)
                            x_ratio = Math.Min(x_ratio, y_ratio);
                        else
                            x_ratio = Math.Max(x_ratio, y_ratio);

                        double x_trans = 0;
                        double x_diff = attrWidth - (x_ratio * w);
                        double y_trans = 0;
                        double y_diff = attrHeight - (x_ratio * h);

                        if (par.Align == SvgPreserveAspectRatioType.XMidYMax ||
                          par.Align == SvgPreserveAspectRatioType.XMidYMid ||
                          par.Align == SvgPreserveAspectRatioType.XMidYMin)
                        {
                            // align to the Middle X
                            x_trans = x_diff / 2;
                        }
                        else if (par.Align == SvgPreserveAspectRatioType.XMaxYMax ||
                          par.Align == SvgPreserveAspectRatioType.XMaxYMid ||
                          par.Align == SvgPreserveAspectRatioType.XMaxYMin)
                        {
                            // align to the right X
                            x_trans = x_diff;
                        }

                        if (par.Align == SvgPreserveAspectRatioType.XMaxYMid ||
                          par.Align == SvgPreserveAspectRatioType.XMidYMid ||
                          par.Align == SvgPreserveAspectRatioType.XMinYMid)
                        {
                            // align to the middle Y
                            y_trans = y_diff / 2;
                        }
                        else if (par.Align == SvgPreserveAspectRatioType.XMaxYMax ||
                          par.Align == SvgPreserveAspectRatioType.XMidYMax ||
                          par.Align == SvgPreserveAspectRatioType.XMinYMax)
                        {
                            // align to the bottom Y
                            y_trans = y_diff;
                        }

                        matrix = matrix.Translate(x_trans, y_trans);
                        matrix = matrix.Scale(x_ratio);
                    }
                    // Translate for min-x and min-y
                    matrix = matrix.Translate(x, y);

                    // Set the cache
                    _cachedViewBoxTransform = matrix;
                }
                return _cachedViewBoxTransform;
            }
        }

        /// <summary>
        /// The definition of the initial view (i.e., before magnification and panning) of the current innermost SVG document fragment. The meaning depends on the situation:
        /// * If the initial view was a "standard" view, then:
        ///  o the values for viewBox, preserveAspectRatio and zoomAndPan within currentView will match the values for the corresponding DOM attributes that are on SVGSVGElement directly
        ///  o the values for transform and viewTarget within currentView will be null
        /// * If the initial view was a link into a 'view' element, then:
        ///  o the values for viewBox, preserveAspectRatio and zoomAndPan within currentView will correspond to the corresponding attributes for the given 'view' element
        ///  o the values for transform and viewTarget within currentView will be null
        /// * If the initial view was a link into another element (i.e., other than a 'view'), then:
        ///  o the values for viewBox, preserveAspectRatio and zoomAndPan within currentView will match the values for the corresponding DOM attributes that are on SVGSVGElement directly for the closest ancestor 'svg' element
        ///  o the values for transform within currentView will be null
        ///  o the viewTarget within currentView will represent the target of the link
        /// * If the initial view was a link into the SVG document fragment using an SVG view specification fragment identifier (i.e., #svgView(...)), then:
        ///  o the values for viewBox, preserveAspectRatio, zoomAndPan, transform and viewTarget within currentView will correspond to the values from the SVG view specification fragment identifier
        /// The object itself and its contents are both readonly. 
        /// </summary>
        public ISvgViewSpec CurrentView
        {
            get {
                if (_currentView == null)
                    _currentView = new SvgViewSpec(this) as ISvgViewSpec;
                // For now, we only return the "standard" view.
                return _currentView;
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
