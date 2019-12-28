using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgFEMorphologyElement : SvgFilterPrimitiveStandardAttributes, ISvgFEMorphologyElement
    {
        #region Private Fields

        private ISvgAnimatedString _in1;
        private ISvgAnimatedEnumeration _operator;

        private ISvgAnimatedNumber _radiusX;
        private ISvgAnimatedNumber _radiusY;

        #endregion

        #region Constructors

        public SvgFEMorphologyElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFEMorphologyElement Members

        public ISvgAnimatedString In
        {
            get {
                return this.In1;
            }
        }

        /// <summary>
        /// Corresponds to attribute <c>in</c> on the given <c>feBlend</c> element. 
        /// </summary>
        /// <value>
        /// <para> in = "SourceGraphic | SourceAlpha | BackgroundImage | BackgroundAlpha | 
        ///             FillPaint | StrokePaint | filter-primitive-reference"</para>
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedString In1
        {
            get {
                if (_in1 == null)
                {
                    _in1 = new SvgAnimatedString(this.GetAttribute("in", string.Empty));
                }
                return _in1;
            }
        }

        /// <summary>
        /// A keyword indicating whether to erode (i.e., thin) or dilate (fatten) the source graphic. 
        /// </summary>
        /// <value>
        /// <para>operator = "erode | dilate"</para>
        /// An enumeration of the type <see cref="SvgFilterMorphologyOperator"/>. 
        /// The default value is <see cref="SvgFilterMorphologyOperator.Erode"/>.
        /// </value>
        /// <remarks>
        /// If attribute 'operator' is not specified, then the effect is as if a value of erode were specified. 
        /// </remarks>
        public ISvgAnimatedEnumeration Operator
        {
            get {
                if (_operator == null)
                {
                    SvgFilterMorphologyOperator compositeOperator = SvgFilterMorphologyOperator.Erode;
                    if (this.HasAttribute("operator"))
                    {
                        compositeOperator = (SvgFilterMorphologyOperator)Enum.Parse(typeof(SvgFilterMorphologyOperator),
                            this.GetAttribute("operator"), true);
                    }
                    _operator = new SvgAnimatedEnumeration((ushort)compositeOperator);
                }
                return _operator;
            }
        }

        /// <summary>
        /// </summary>
        /// <value>
        /// <para></para>
        /// </value>
        /// <remarks>
        /// <para>
        /// The radius (or radii) for the operation. If two <c>number</c>s are provided, the first number represents a x-radius 
        /// and the second value represents a y-radius. If one number is provided, then that value is used for both X and Y. 
        /// The values are in the coordinate system established by attribute 'primitiveUnits' on the 'filter' element.
        /// </para>
        /// <para>
        /// A negative value is an error(see Error processing). A value of zero disables the effect of the given filter 
        /// primitive(i.e., the result is a transparent black image).
        /// </para>
        /// <para>
        /// If the attribute is not specified, then the effect is as if a value of <c>0</c> were specified.
        /// </para>
        /// </remarks>
        public ISvgAnimatedNumber RadiusX
        {
            get {
                if (_radiusX == null || _radiusY == null)
                {
                    this.ParseRadius();
                }
                return _radiusX;
            }
        }

        /// <summary>
        /// </summary>
        /// <value>
        /// <para></para>
        /// </value>
        /// <remarks>
        /// <para>
        /// The radius (or radii) for the operation. If two <c>number</c>s are provided, the first number represents a x-radius 
        /// and the second value represents a y-radius. If one number is provided, then that value is used for both X and Y. 
        /// The values are in the coordinate system established by attribute 'primitiveUnits' on the 'filter' element.
        /// </para>
        /// <para>
        /// A negative value is an error(see Error processing). A value of zero disables the effect of the given filter 
        /// primitive(i.e., the result is a transparent black image).
        /// </para>
        /// <para>
        /// If the attribute is not specified, then the effect is as if a value of <c>0</c> were specified.
        /// </para>
        /// </remarks>
        public ISvgAnimatedNumber RadiusY
        {
            get {
                if (_radiusX == null || _radiusY == null)
                {
                    this.ParseRadius();
                }
                return _radiusY;
            }
        }

        #endregion

        #region Private Methods

        private void ParseRadius()
        {
            if (!this.HasAttribute("radius"))
            {
                _radiusX = new SvgAnimatedNumber(0);
                _radiusY = new SvgAnimatedNumber(0);
            }
            var radius = this.GetAttribute("radius");
            if (string.IsNullOrWhiteSpace(radius))
            {
                _radiusX = new SvgAnimatedNumber(0);
                _radiusY = new SvgAnimatedNumber(0);
            }

            var radiusParts = _reSeparators.Split(radius);
            if (radiusParts == null || radiusParts.Length == 0)
            {
                _radiusX = new SvgAnimatedNumber(0);
                _radiusY = new SvgAnimatedNumber(0);
            }
            if (radiusParts.Length == 1)
            {
                var radiusValue = SvgNumber.ParseNumber(radiusParts[0]);
                _radiusX = new SvgAnimatedNumber(radiusValue);
                _radiusY = new SvgAnimatedNumber(radiusValue);
            }
            else if (radiusParts.Length == 2)
            {
                _radiusX = new SvgAnimatedNumber(SvgNumber.ParseNumber(radiusParts[0]));
                _radiusY = new SvgAnimatedNumber(SvgNumber.ParseNumber(radiusParts[1]));
            }
        }

        #endregion
    }
}
