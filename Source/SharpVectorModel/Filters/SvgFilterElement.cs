using System;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// An implementation of the <see cref="ISvgFilterElement"/> interface, which corresponds to the <c>'filter'</c> element.
    /// </summary>
    /// <remarks>
    /// <para>A filter effect is a graphical operation that is applied to an element as it is drawn into the document. 
    /// It is an image-based effect, in that it takes zero or more images as input, a number of parameters specific to the 
    /// effect, and then produces an image as output. The output image is either rendered into the document instead of the 
    /// original element, used as an input image to another filter effect, or provided as a CSS image value.
    /// </para>
    /// <para>Filter Region</para>
    /// <para>A filter element can define a filter region on the canvas to which a given filter effect 
    /// applies and can provide a resolution for any intermediate continuous tone images used to process 
    /// any raster-based filter primitives.</para>
    /// <para>The filter element has the following attributes which work together to define the filter region:</para>
    /// <para><see cref="FilterUnits"/>, <see cref="X"/>, <see cref="Y"/>, <see cref="Width"/>, <see cref="Height"/></para>
    /// </remarks>
    public sealed class SvgFilterElement : SvgStyleableElement, ISvgFilterElement
    {
        #region Public Fields

        /// <summary>
        /// This keyword represents the graphics elements that were the original input into the filter element. 
        /// </summary>
        public const string SourceGraphic   = "SourceGraphic";
        /// <summary>
        /// This keyword represents the graphics elements that were the original input into the filter element. 
        /// </summary>
        public const string SourceAlpha     = "SourceAlpha";
        /// <summary>
        /// This keyword represents the back drop defined by the current isolation group behind the filter region 
        /// at the time that the filter element was invoked.
        /// </summary>
        public const string BackgroundImage = "BackgroundImage";
        /// <summary>
        /// Same as BackgroundImage except only the alpha channel is used. See SourceAlpha and the isolation property.
        /// </summary>
        public const string BackgroundAlpha = "BackgroundAlpha";
        /// <summary>
        /// This keyword represents the value of the fill property on the target element for the filter effect. 
        /// </summary>
        public const string FillPaint       = "FillPaint";
        /// <summary>
        /// This keyword represents the value of the stroke property on the target element for the filter effect.
        /// </summary>
        public const string StrokePaint     = "StrokePaint";

        #endregion

        #region Private Fields

        private static readonly Regex _reSeparators = new Regex("[\\s\\,]+");

        private ISvgAnimatedLength _x;
        private ISvgAnimatedLength _y;
        private ISvgAnimatedLength _width;
        private ISvgAnimatedLength _height;

        private ISvgAnimatedEnumeration _filterUnits;
        private ISvgAnimatedEnumeration _primitiveUnits;

        private ISvgAnimatedInteger _filterResX;
        private ISvgAnimatedInteger _filterResY;

        private SvgUriReference _uriReference;
        private SvgExternalResourcesRequired _resourcesRequired;

        #endregion

        #region Constructors

        public SvgFilterElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            _uriReference      = new SvgUriReference(this);
            _resourcesRequired = new SvgExternalResourcesRequired(this);
        }

        #endregion

        #region ISvgFilterElement Interface

        /// <summary>
        /// Gets a value definining the coordinate system for attributes <see cref="X"/>, <see cref="Y"/>, 
        /// <see cref="Width"/>, <see cref="Height"/>.
        /// </summary>
        /// <value>
        /// <para>filterUnits = "userSpaceOnUse | objectBoundingBox"</para>
        /// <para>If the value is equal to <see cref="SvgUnitType.UserSpaceOnUse"/>, x, y, width, height represent values 
        /// in the current user coordinate system in place at the time when the filter element is referenced (i.e., the 
        /// user coordinate system for the element referencing the filter element via a filter property).
        /// </para>
        /// <para>If the value is equal to <see cref="SvgUnitType.ObjectBoundingBox"/>, then x, y, width, height represent 
        /// fractions or percentages of the bounding box on the referencing element (see object bounding box units).</para>
        /// <para>The initial value for filter-units is <see cref="SvgUnitType.ObjectBoundingBox"/>.</para>
        /// </value>
        /// <remarks></remarks>
        public ISvgAnimatedEnumeration FilterUnits
        {
            get {
                if (_filterUnits == null)
                {
                    SvgUnitType filterUnits = SvgUnitType.ObjectBoundingBox;
                    if (string.Equals(this.GetAttribute("filterUnits"), "userSpaceOnUse", StringComparison.Ordinal))
                        filterUnits = SvgUnitType.UserSpaceOnUse;

                    _filterUnits = new SvgAnimatedEnumeration((ushort)filterUnits);
                }
                return _filterUnits;
            }
        }

        /// <summary>
        /// Gets a value specifying the coordinate system for the various length values within the filter primitives 
        /// and for the attributes that define the filter primitive subregion.
        /// </summary>
        /// <value>
        /// <para>primitiveUnits = "userSpaceOnUse | objectBoundingBox" </para>
        /// <para>If the value is equal to <see cref="SvgUnitType.UserSpaceOnUse"/>, any length values within the filter 
        /// definitions represent values in the current local coordinate system in place at the time when the filter element 
        /// is referenced (i.e., the user coordinate system for the element referencing the filter element via a filter property).
        /// </para>
        /// <para>If the value is equal to <see cref="SvgUnitType.ObjectBoundingBox"/>, then any length values within the filter 
        /// definitions represent fractions or percentages of the bounding box on the referencing element (see object bounding 
        /// box units). Note that if only one number was specified in a <c>number-optional-number</c> value this number is expanded 
        /// out before the primitive-units computation takes place.
        /// </para>
        /// <para>The initial value for primitive-units is <see cref="SvgUnitType.UserSpaceOnUse"/>.</para>
        /// </value>
        /// <remarks></remarks>
        public ISvgAnimatedEnumeration PrimitiveUnits
        {
            get {
                if (_primitiveUnits == null)
                {
                    SvgUnitType primitiveUnits = SvgUnitType.UserSpaceOnUse;
                    if (string.Equals(this.GetAttribute("primitiveUnits"), "objectBoundingBox", StringComparison.Ordinal))
                        primitiveUnits = SvgUnitType.ObjectBoundingBox;
                    _primitiveUnits = new SvgAnimatedEnumeration((ushort)primitiveUnits);
                }
                return _primitiveUnits;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// <para></para>
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedLength X
        {
            get {
                if (_x == null)
                {
                    _x = new SvgAnimatedLength(this, "x", SvgLengthDirection.Horizontal, "-10%");
                }
                return _x;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedLength Y
        {
            get {
                if (_y == null)
                {
                    _y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, "-10%");
                }
                return _y;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedLength Width
        {
            get {
                if (_width == null)
                {
                    _width = new SvgAnimatedLength(this, "width", SvgLengthDirection.Viewport, "120%");
                }
                return _width;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedLength Height
        {
            get {
                if (_height == null)
                {
                    _height = new SvgAnimatedLength(this, "height", SvgLengthDirection.Viewport, "120%");
                }
                return _height;
            }
        }

        /// <summary>
        /// The contains the X component of attribute <c>'filterRes'</c>. 
        /// The <c>'filterRes'</c> indicates the width and height of the intermediate images in pixels of a filter primitive.
        /// </summary>
        /// <value>This value takes one or two values, the first one outlining the resolution in horizontal direction, 
        /// the second one in vertical direction. If only one value is specified, it is used for both directions.</value>
        /// <remarks>The <c>'filterRes'</c> attribute was removed from the <c>SVG 2</c> specification.</remarks>
        public ISvgAnimatedInteger FilterResX
        {
            get {
                if (_filterResX == null || _filterResY == null)
                {
                    this.ParseFilterRes();
                }
                return _filterResX;
            }
        }

        /// <summary>
        /// The contains the X component of attribute <c>'filterRes'</c>. 
        /// The <c>'filterRes'</c> indicates the width and height of the intermediate images in pixels of a filter primitive.
        /// </summary>
        /// <value>This value takes one or two values, the first one outlining the resolution in horizontal direction, 
        /// the second one in vertical direction. If only one value is specified, it is used for both directions.</value>
        /// <remarks>The <c>'filterRes'</c> attribute was removed from the <c>SVG 2</c> specification.</remarks>
        public ISvgAnimatedInteger FilterResY
        {
            get {
                if (_filterResX == null || _filterResY == null)
                {
                    this.ParseFilterRes();
                }
                return _filterResY;
            }
        }

        /// <summary>
        /// Sets the values for attribute <c>'filterRes'</c>.
        /// </summary>
        /// <param name="filterResX">The X component of attribute <c>'filterRes'</c>.</param>
        /// <param name="filterResY">The Y component of attribute <c>'filterRes'</c>. </param>
        public void SetFilterRes(ulong filterResX, ulong filterResY)
        {
            _filterResX = new SvgAnimatedInteger(filterResX);
            _filterResY = new SvgAnimatedInteger(filterResY);

            this.SetAttribute("filterRes", string.Format("{0} {1}", filterResX, filterResY));
        }

        #endregion

        #region Implementation of ISvgURIReference

        public ISvgAnimatedString Href
        {
            get {
                return _uriReference.Href;
            }
        }

        #endregion

        #region Implementation of ISvgExternalResourcesRequired

        public ISvgAnimatedBoolean ExternalResourcesRequired
        {
            get {
                return _resourcesRequired.ExternalResourcesRequired;
            }
        }

        #endregion

        #region Private Methods

        // <filter id = "edgeFilter" >
        //     < feConvolveMatrix filterRes="100 100" style="color-interpolation-filters:sRGB" 
        //            order="3" kernelMatrix="0 -1 0   -1 4 -1   0 -1 0" preserveAlpha="true"/> 
        // </filter>        
        private void ParseFilterRes()
        {
            if (!this.HasAttribute("filterRes"))
            {
                return;
            }
            var filterRes = this.GetAttribute("filterRes");
            if (string.IsNullOrWhiteSpace(filterRes))
            {
                return;
            }

            var filterResParts = _reSeparators.Split(filterRes);
            if (filterResParts == null || filterResParts.Length == 0)
            {
                return;
            }
            if (filterResParts.Length == 1)
            {
                var filterResValue = SvgNumber.ParseNumber(filterResParts[0]);
                _filterResX = new SvgAnimatedInteger(filterResValue);
                _filterResY = new SvgAnimatedInteger(filterResValue);
            }
            else if (filterResParts.Length == 2)
            {
                _filterResX = new SvgAnimatedInteger(SvgNumber.ParseNumber(filterResParts[0]));
                _filterResY = new SvgAnimatedInteger(SvgNumber.ParseNumber(filterResParts[1]));
            }
        }

        #endregion
    }
}
