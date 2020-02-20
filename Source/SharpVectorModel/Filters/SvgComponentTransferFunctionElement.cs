using System;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// This <see langword="abstract"/> defines a base class used by the component transfer function interfaces. 
    /// </summary>
    public abstract class SvgComponentTransferFunctionElement : SvgElement, ISvgComponentTransferFunctionElement
    {
        #region Protected Fields

        protected static readonly Regex _reSeparators = new Regex("[\\s\\,]+");

        protected ISvgAnimatedEnumeration _type;
        protected ISvgAnimatedNumberList _tableValues;
        protected ISvgAnimatedNumber _slope;
        protected ISvgAnimatedNumber _intercept;
        protected ISvgAnimatedNumber _amplitude;
        protected ISvgAnimatedNumber _exponent;
        protected ISvgAnimatedNumber _offset;

        #endregion

        #region Constructors

        protected SvgComponentTransferFunctionElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region ISvgComponentTransferFunctionElement Members

        /// <summary>
        /// Corresponds to attribute 'type' on the given element. 
        /// </summary>
        /// <value>
        /// <para>type = "identity | table | discrete | linear | gamma" </para>
        /// <para>Indicates the type of component transfer function. The type of function determines the applicability 
        /// of the other attributes.</para>
        /// An enumeration of the type <see cref="SvgFilterTransferType"/>. The default value is 
        /// <see cref="SvgFilterTransferType.Identity"/>.
        /// </value>
        public ISvgAnimatedEnumeration Type
        {
            get {
                if (_type == null)
                {
                    SvgFilterTransferType feTransferType = SvgFilterTransferType.Identity;
                    if (this.HasAttribute("type"))
                    {
                        feTransferType = (SvgFilterTransferType)Enum.Parse(typeof(SvgFilterTransferType), 
                            this.GetAttribute("type"), true);
                    }
                    _type = new SvgAnimatedEnumeration((ushort)feTransferType);
                }
                return _type;
            }
        }

        /// <summary>
        /// Corresponds to attribute 'tableValues' on the given element. 
        /// </summary>
        /// <value>
        /// <para>tableValues = "(list of numbers)" </para>
        /// </value>
        public ISvgAnimatedNumberList TableValues
        {
            get {
                if (_tableValues == null)
                {
                    _tableValues = SvgAnimatedNumberList.Empty;
                    if (this.HasAttribute("tableValues"))
                    {
                        _tableValues = new SvgAnimatedNumberList(this.GetAttribute("tableValues"));
                    }
                }
                return _tableValues;
            }
        }

        /// <summary>
        /// When type="linear", the slope of the linear function.
        /// </summary>
        /// <value>
        /// <para>slope = "number"</para>
        /// The initial value for slope is <c>1</c>.
        /// </value>
        public ISvgAnimatedNumber Slope
        {
            get {
                if (_slope == null)
                {
                    _slope = new SvgAnimatedNumber(this.GetAttribute("slope", 1.0));
                }
                return _slope;
            }
        }

        /// <summary>
        /// When type="linear", the intercept of the linear function.
        /// </summary>
        /// <value>
        /// <para>slope = "number"</para>
        /// The initial value for intercept is <c>0</c>.
        /// </value>
        public ISvgAnimatedNumber Intercept
        {
            get {
                if (_intercept == null)
                {
                    _intercept = new SvgAnimatedNumber(this.GetAttribute("intercept", 0.0));
                }
                return _intercept;
            }
        }

        /// <summary>
        /// When type="gamma", the amplitude of the gamma function.
        /// </summary>
        /// <value>
        /// <para>amplitude = "number" </para>
        /// The initial value for amplitude is <c>1</c>.
        /// </value>
        public ISvgAnimatedNumber Amplitude
        {
            get {
                if (_amplitude == null)
                {
                    _amplitude = new SvgAnimatedNumber(this.GetAttribute("amplitude", 1.0));
                }
                return _amplitude;
            }
        }

        /// <summary>
        /// When type="gamma", the exponent of the gamma function.
        /// </summary>
        /// <value>
        /// <para>exponent = "number" </para>
        /// The initial value for exponent is <c>1</c>.
        /// </value>
        public ISvgAnimatedNumber Exponent
        {
            get {
                if (_exponent == null)
                {
                    _exponent = new SvgAnimatedNumber(this.GetAttribute("exponent", 1.0));
                }
                return _exponent;
            }
        }

        /// <summary>
        /// When type="gamma", the offset of the gamma function.
        /// </summary>
        /// <value>
        /// <para>offset = "number" </para>
        /// The initial value for offset is <c>0</c>.
        /// </value>
        public ISvgAnimatedNumber Offset
        {
            get {
                if (_offset == null)
                {
                    _offset = new SvgAnimatedNumber(this.GetAttribute("offset", 0.0));
                }
                return _offset;
            }
        }

        #endregion
    }
}
