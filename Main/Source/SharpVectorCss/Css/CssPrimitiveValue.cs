// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;
using System.Xml;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// The CSSPrimitiveValue interface represents a single CSS value. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface may be used to determine the value of a specific style 
    /// property currently set in a block or to set a specific style property 
    /// explicitly within the block. An instance of this interface might be 
    /// obtained from the getPropertyCSSValue method of the CSSStyleDeclaration 
    /// interface. A CSSPrimitiveValue object only occurs in a context of a CSS property.
    /// </para>
    /// <para>
    /// Conversions are allowed between absolute values (from millimeters to centimeters, 
    /// from degrees to radians, and so on) but not between relative values. (For example, 
    /// a pixel value cannot be converted to a centimeter value.) Percentage values can't 
    /// be converted since they are relative to the parent value (or another property value). 
    /// There is one exception for color percentage values: since a color percentage value 
    /// is relative to the range 0-255, a color percentage value can be converted to a number; 
    /// (see also the RGBColor interface).
    /// </para>
    /// </remarks>
    public class CssPrimitiveValue : CssValue, ICssPrimitiveValue
    {
        #region Private Fields

        private CssPrimitiveType _primitiveType;

        protected double floatValue;
        private string stringValue;
        private   CssRect rectValue;
        protected CssColor colorValue;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Constructor called by CssValue.GetCssValue()
        /// </summary>
        /// <param name="match">A Regex that matches a CssPrimitiveValue</param>
        /// <param name="readOnly">Specifiec if this instance is read-only</param>
        private CssPrimitiveValue(Match match, bool readOnly)
            : this(match.Value, readOnly)
        {
            if (match.Groups["func"].Success)
            {
                switch (match.Groups["funcname"].Value)
                {
                    case "rect":
                        _primitiveType = CssPrimitiveType.Rect;
                        rectValue = new CssRect(match.Groups["funcvalue"].Value, ReadOnly);
                        break;
                    case "attr":
                        _primitiveType = CssPrimitiveType.Attr;
                        stringValue = match.Groups["funcvalue"].Value;
                        break;
                    case "url":
                        stringValue = match.Groups["funcvalue"].Value;
                        _primitiveType = CssPrimitiveType.Uri;
                        break;
                    case "counter":
                        throw new NotImplementedException("Counters are not implemented");
                    //_primitiveType = CssPrimitiveType.CSS_COUNTER;
                }
            }
            else if (match.Groups["freqTimeNumber"].Success)
            {
                floatValue = Single.Parse(match.Groups["numberValue2"].Value, CssNumber.Format);

                switch (match.Groups["unit2"].Value)
                {
                    case "Hz":
                        _primitiveType = CssPrimitiveType.Hz;
                        break;
                    case "kHz":
                        _primitiveType = CssPrimitiveType.KHz;
                        break;
                    case "in":
                        _primitiveType = CssPrimitiveType.In;
                        break;
                    case "s":
                        _primitiveType = CssPrimitiveType.S;
                        break;
                    case "ms":
                        _primitiveType = CssPrimitiveType.Ms;
                        break;
                    case "%":
                        _primitiveType = CssPrimitiveType.Percentage;
                        break;
                    default:
                        _primitiveType = CssPrimitiveType.Number;
                        break;
                }
            }
            else if (match.Groups["string"].Success)
            {
                stringValue = match.Groups["stringvalue"].Value;
                _primitiveType = CssPrimitiveType.String;
            }
            else if (match.Groups["colorIdent"].Success)
            {
                string val = match.Value;
                stringValue = match.Value;
                _primitiveType = CssPrimitiveType.Ident;
            }
            else
            {
                _primitiveType = CssPrimitiveType.Unknown;
            }
        }

        protected CssPrimitiveValue(string cssText, bool readOnly)
            : base(CssValueType.PrimitiveValue, cssText, readOnly)
        {
            _primitiveType = CssPrimitiveType.Unknown;
            floatValue     = Double.NaN;
        }

        /// <summary>
        /// Only for internal use
        /// </summary>
        protected CssPrimitiveValue()
            : base()
        {
            _primitiveType = CssPrimitiveType.Unknown;
            _cssValueType  = CssValueType.PrimitiveValue;
            floatValue     = Double.NaN;
        }

        #endregion

        #region Public Properties

        public string PrimitiveTypeAsString
        {
            get
            {
                switch (PrimitiveType)
                {
                    case CssPrimitiveType.Percentage:
                        return "%";
                    case CssPrimitiveType.Ems:
                        return "em";
                    case CssPrimitiveType.Exs:
                        return "ex";
                    case CssPrimitiveType.Px:
                        return "px";
                    case CssPrimitiveType.Cm:
                        return "cm";
                    case CssPrimitiveType.Mm:
                        return "mm";
                    case CssPrimitiveType.In:
                        return "in";
                    case CssPrimitiveType.Pt:
                        return "pt";
                    case CssPrimitiveType.Pc:
                        return "pc";
                    case CssPrimitiveType.Deg:
                        return "deg";
                    case CssPrimitiveType.Rad:
                        return "rad";
                    case CssPrimitiveType.Grad:
                        return "grad";
                    case CssPrimitiveType.Ms:
                        return "ms";
                    case CssPrimitiveType.S:
                        return "s";
                    case CssPrimitiveType.Hz:
                        return "hz";
                    case CssPrimitiveType.KHz:
                        return "khz";
                    default:
                        return String.Empty;
                }
            }
        }

        public override string CssText
        {
            get
            {
                if (PrimitiveType == CssPrimitiveType.String)
                {
                    return "\"" + GetStringValue() + "\"";
                }
                else
                {
                    return base.CssText;
                }
            }
            set
            {
                if (ReadOnly)
                {
                    throw new DomException(DomExceptionType.InvalidModificationErr,
                        "CssPrimitiveValue is read-only");
                }
                else
                {
                    OnSetCssText(value);
                }
            }
        }

        #endregion

        #region Public Methods

        public static CssPrimitiveValue Create(Match match, bool readOnly)
        {
            if (match.Groups["length"].Success)
            {
                return new CssPrimitiveLengthValue(match.Groups["lengthNumber"].Value, match.Groups["lengthUnit"].Value, readOnly);
            }
            else if (match.Groups["angle"].Success)
            {
                return new CssPrimitiveAngleValue(match.Groups["angleNumber"].Value, match.Groups["angleUnit"].Value, readOnly);
            }
            else if (match.Groups["funcname"].Success && match.Groups["funcname"].Value == "rgb")
            {
                return new CssPrimitiveRgbValue(match.Groups["func"].Value, readOnly);
            }
            else if (match.Groups["colorIdent"].Success && CssPrimitiveRgbValue.IsColorName(match.Groups["colorIdent"].Value))
            {
                return new CssPrimitiveRgbValue(match.Groups["colorIdent"].Value, readOnly);
            }
            else
            {
                return new CssPrimitiveValue(match, readOnly);
            }
        }

        public override CssValue GetAbsoluteValue(string propertyName, XmlElement elm)
        {
            if (propertyName == "font-size")
            {
                return new CssAbsPrimitiveLengthValue(this, propertyName, elm);
            }
            else
            {
                return new CssAbsPrimitiveValue(this, propertyName, elm);
            }
        }

        #endregion

        #region Protected Methods

        protected virtual void OnSetCssText(string cssText)
        {
        }

        #endregion

        #region ICssPrimitiveValue Members

        /// <summary>
        /// A method to set the float value with a specified unit. If the property attached with this value can not accept the specified unit or the float value, the value will be unchanged and a DOMException will be raised.
        /// </summary>
        /// <param name="unitType">A unit code as defined above. The unit code can only be a float unit type (i.e. CSS_NUMBER, CSS_PERCENTAGE, CSS_EMS, CSS_EXS, CSS_PX, CSS_CM, CSS_MM, CSS_IN, CSS_PT, CSS_PC, CSS_DEG, CSS_RAD, CSS_GRAD, CSS_MS, CSS_S, CSS_HZ, CSS_KHZ, CSS_DIMENSION).</param>
        /// <param name="floatValue">The new float value.</param>
        /// <exception cref="DomException">INVALID_ACCESS_ERR: Raised if the CSS value doesn't contain a float value.</exception>
        /// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this property is readonly.</exception>
        public virtual void SetFloatValue(CssPrimitiveType unitType, double floatValue)
        {
            if (this.ReadOnly)
            {
                throw new DomException(DomExceptionType.NoModificationAllowedErr);
            }
            else
            {
                _primitiveType = unitType;
                SetFloatValue(floatValue);
            }
        }

        protected void SetFloatValue(string floatValue)
        {
            SetFloatValue(Double.Parse(floatValue, CssNumber.Format));
        }

        protected void SetFloatValue(double floatValue)
        {
            this.floatValue = floatValue;
        }

        /// <summary>
        /// This method is used to get a float value in a specified unit. If this CSS value doesn't contain a float value or can't be converted into the specified unit, a DOMException is raised
        /// </summary>
        /// <param name="unitType">A unit code to get the float value. The unit code can only be a float unit type (i.e. CSS_NUMBER, CSS_PERCENTAGE, CSS_EMS, CSS_EXS, CSS_PX, CSS_CM, CSS_MM, CSS_IN, CSS_PT, CSS_PC, CSS_DEG, CSS_RAD, CSS_GRAD, CSS_MS, CSS_S, CSS_HZ, CSS_KHZ, CSS_DIMENSION).</param>
        /// <returns>The float value in the specified unit.</returns>
        /// <exception cref="DomException">INVALID_ACCESS_ERR: Raised if the CSS value doesn't contain a float value.</exception>
        public virtual double GetFloatValue(CssPrimitiveType unitType)
        {
            if (Double.IsNaN(floatValue))
            {
                throw new DomException(DomExceptionType.InvalidAccessErr);
            }
            else
            {
                double ret = Double.NaN;
                switch (PrimitiveType)
                {
                    case CssPrimitiveType.Percentage:
                        if (unitType == CssPrimitiveType.Percentage) ret = floatValue;
                        break;
                    case CssPrimitiveType.Ms:
                        if (unitType == CssPrimitiveType.Ms) ret = floatValue;
                        else if (unitType == CssPrimitiveType.S) ret = floatValue / 1000;
                        break;
                    case CssPrimitiveType.S:
                        if (unitType == CssPrimitiveType.Ms) ret = floatValue * 1000;
                        else if (unitType == CssPrimitiveType.S) ret = floatValue;
                        break;
                    case CssPrimitiveType.Hz:
                        if (unitType == CssPrimitiveType.Hz) ret = floatValue;
                        else if (unitType == CssPrimitiveType.KHz) ret = floatValue / 1000;
                        break;
                    case CssPrimitiveType.KHz:
                        if (unitType == CssPrimitiveType.Hz) ret = floatValue * 1000;
                        else if (unitType == CssPrimitiveType.KHz) ret = floatValue;
                        break;
                    case CssPrimitiveType.Dimension:
                        if (unitType == CssPrimitiveType.Dimension) ret = floatValue;
                        break;
                }
                if (Double.IsNaN(ret))
                {
                    throw new DomException(DomExceptionType.InvalidAccessErr);
                }
                else
                {
                    return ret;
                }
            }
        }

        /// <summary>
        /// A method to set the string value with the specified unit. If the property attached to this value can't accept the specified unit or the string value, the value will be unchanged and a DOMException will be raised.
        /// </summary>
        /// <param name="stringType">A string code as defined above. The string code can only be a string unit type (i.e. CSS_STRING, CSS_URI, CSS_IDENT, and CSS_ATTR).</param>
        /// <param name="stringValue">The new string value</param>
        /// <exception cref="DomException">INVALID_ACCESS_ERR: Raised if the CSS value doesn't contain a string value.</exception>
        /// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this property is readonly.</exception>
        public virtual void SetStringValue(CssPrimitiveType stringType, string stringValue)
        {
            throw new NotImplementedException("SetStringValue");
        }

        /// <summary>
        /// This method is used to get the string value. If the CSS value doesn't contain a string value, a DOMException is raised.
        /// Note: Some properties (like 'font-family' or 'voice-family') convert a whitespace separated list of idents to a string.
        /// </summary>
        /// <returns>The string value in the current unit. The current primitiveType can only be a string unit type (i.e. CSS_STRING, CSS_URI, CSS_IDENT and CSS_ATTR).</returns>
        /// <exception cref="DomException">INVALID_ACCESS_ERR: Raised if the CSS value doesn't contain a string value.</exception>
        public virtual string GetStringValue()
        {
            string ret = null;
            switch (PrimitiveType)
            {
                case CssPrimitiveType.String:
                case CssPrimitiveType.Uri:
                case CssPrimitiveType.Ident:
                case CssPrimitiveType.Attr:
                    ret = stringValue;
                    break;
            }

            if (ret != null) 
                return ret;
            else 
                throw new DomException(DomExceptionType.InvalidAccessErr);
        }

        /// <summary>
        /// This method is used to get the Counter value. If this CSS value doesn't contain a counter value, a DOMException is raised. Modification to the corresponding style property can be achieved using the Counter interface
        /// </summary>
        /// <returns>The Counter value.</returns>
        /// <exception cref="DomException">INVALID_ACCESS_ERR: Raised if the CSS value doesn't contain a Counter value (e.g. this is not CSS_COUNTER).</exception>
        public virtual ICssCounter GetCounterValue()
        {
            throw new NotImplementedException("GetCounterValue");
        }

        /// <summary>
        /// This method is used to get the Rect value. If this CSS value doesn't contain a rect value, a DOMException is raised. Modification to the corresponding style property can be achieved using the Rect interface.
        /// </summary>
        /// <returns>The Rect value.</returns>
        /// <exception cref="DomException">INVALID_ACCESS_ERR: Raised if the CSS value doesn't contain a rect value.</exception>
        public virtual ICssRect GetRectValue()
        {
            if (PrimitiveType == CssPrimitiveType.Rect) 
                return rectValue;
            else 
                throw new DomException(DomExceptionType.InvalidAccessErr);
        }

        /// <summary>
        /// This method is used to get the RGB color. If this CSS value doesn't contain a RGB color value, a DOMException is raised. Modification to the corresponding style property can be achieved using the RGBColor interface.
        /// </summary>
        /// <returns>the RGB color value.</returns>
        /// <exception cref="DomException">INVALID_ACCESS_ERR: Raised if the CSS value doesn't contain a rgb value.</exception>
        public virtual ICssColor GetRgbColorValue()
        {
            if (PrimitiveType == CssPrimitiveType.RgbColor) 
                return colorValue;
            else 
                throw new DomException(DomExceptionType.InvalidAccessErr);
        }

        protected void SetPrimitiveType(CssPrimitiveType type)
        {
            _primitiveType = type;
        }

        /// <summary>
        /// The type of the value as defined by the constants specified above.
        /// </summary>
        public virtual CssPrimitiveType PrimitiveType
        {
            get
            {
                return _primitiveType;
            }
            protected set
            {
                _primitiveType = value;
            }
        }

        #endregion
    }
}
