using System;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This is <see langword="abstract"/> class for filter primitive attributes that are available for all filter primitives:
    /// </summary>
    public abstract class SvgFilterPrimitiveStandardAttributes : SvgStyleableElement, ISvgFilterPrimitiveStandardAttributes
    {
        #region Protected Fields

        protected static readonly Regex _reSeparators = new Regex("[\\s\\,]+");

        protected ISvgAnimatedLength _x;
        protected ISvgAnimatedLength _y;
        protected ISvgAnimatedLength _width;
        protected ISvgAnimatedLength _height;
        protected ISvgAnimatedString _result;

        #endregion

        #region Constructors

        protected SvgFilterPrimitiveStandardAttributes(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgFilterPrimitiveStandardAttributes Members

        /// <summary>
        /// The minimum x coordinate for the subregion which restricts calculation and rendering of the given filter primitive. 
        /// </summary>
        /// <value>
        /// <para>x = "<length-percentage>"</para>
        /// The initial value is <c>0%</c>.
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedLength X
        {
            get {
                if (_x == null)
                {
                    _x = new SvgAnimatedLength(this, "x", SvgLengthDirection.Horizontal, "0%");
                }
                return _x;
            }
        }

        /// <summary>
        /// The minimum y coordinate for the subregion which restricts calculation and rendering of the given filter primitive.
        /// </summary>
        /// <value>
        /// <para>y = "<length-percentage>" </para>
        /// The initial value is <c>0%</c>.
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedLength Y
        {
            get {
                if (_y == null)
                {
                    _y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, "0%");
                }
                return _y;
            }
        }

        /// <summary>
        /// The width of the subregion which restricts calculation and rendering of the given filter primitive.
        /// </summary>
        /// <value>
        /// <para>width = "<length-percentage>" </para>
        /// <para>A negative or zero value disables the effect of the given filter primitive (i.e., the 
        /// result is a transparent black image).</para>
        /// The initial value is <c>100%</c>.
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedLength Width
        {
            get {
                if (_width == null)
                {
                    _width = new SvgAnimatedLength(this, "width", SvgLengthDirection.Viewport, "100%");
                }
                return _width;
            }
        }

        /// <summary>
        /// The height of the subregion which restricts calculation and rendering of the given filter primitive.
        /// </summary>
        /// <value>
        /// <para>height = "<length-percentage>" </para>
        /// <para>A negative or zero value must disable the effect of the given filter primitive (i.e., the 
        /// result is a transparent black image).</para>
        /// The initial value is <c>100%</c>.
        /// </value>
        /// <remarks>
        /// </remarks>
        public ISvgAnimatedLength Height
        {
            get {
                if (_height == null)
                {
                    _height = new SvgAnimatedLength(this, "height", SvgLengthDirection.Viewport, "100%");
                }
                return _height;
            }
        }

        /// <summary>
        /// This is an <c>custom-ident</c> and an assigned name for this filter primitive. 
        /// </summary>
        /// <value>
        /// <para>result = "<filter-primitive-reference>"</para>
        /// </value>
        /// <remarks>
        /// <para>
        /// If supplied, then graphics that result from processing this filter primitive can be referenced by an <c>in</c> 
        /// attribute on a subsequent filter primitive within the same filter element.
        /// </para>
        /// <para>
        /// If no value is provided, the output will only be available for re-use as the implicit input into the 
        /// next filter primitive if that filter primitive provides no value for its in attribute.
        /// </para>
        /// </remarks>
        public ISvgAnimatedString Result
        {
            get {
                if (_result == null)
                {
                    _result = new SvgAnimatedString(this.GetAttribute("result", string.Empty));
                }
                return _result;
            }
        }

        #endregion
    }
}
