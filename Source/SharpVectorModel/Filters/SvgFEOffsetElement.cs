using System;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgFEOffsetElement : SvgFilterPrimitiveStandardAttributes, ISvgFEOffsetElement
    {
        #region Private Fields

        private ISvgAnimatedString _in1;
        private ISvgAnimatedNumber _dx;
        private ISvgAnimatedNumber _dy;

        #endregion

        #region Constructors

        public SvgFEOffsetElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFEOffsetElement Members

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
        /// The amount to offset the input graphic along the x-axis. The offset amount is expressed in the 
        /// coordinate system established by attribute 'primitiveUnits' on the 'filter' element.
        /// </summary>
        /// <value>
        /// <para>dx = "number"</para>
        /// </value>
        /// If the attribute is not specified, then the effect is as if a value of <c>0</c> were specified.
        /// <remarks>
        /// Corresponds to attribute 'dx' on the given 'feOffset' element. 
        /// </remarks>
        public ISvgAnimatedNumber Dx
        {
            get {
                if (_dx == null)
                {
                    _dx = new SvgAnimatedNumber(this.GetAttribute("dx", 0.0));
                }
                return _dx;
            }
        }

        /// <summary>
        /// The amount to offset the input graphic along the y-axis. The offset amount is expressed in the 
        /// coordinate system established by attribute 'primitiveUnits' on the 'filter' element.
        /// </summary>
        /// <value>
        /// <para>dy = "number"</para>
        /// If the attribute is not specified, then the effect is as if a value of <c>0</c> were specified.
        /// </value>
        /// <remarks>
        /// Corresponds to attribute 'dy' on the given 'feOffset' element. 
        /// </remarks>
        public ISvgAnimatedNumber Dy
        {
            get {
                if (_dy == null)
                {
                    _dy = new SvgAnimatedNumber(this.GetAttribute("dy", 0.0));
                }
                return _dy;
            }
        }

        #endregion
    }
}
