using System;
using System.Text.RegularExpressions;

using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Svg
{
    /// <summary>
    /// Defines the direction of a SvgLength
    /// </summary>
    public enum SvgLengthDirection
    {
        Vertical,
        Horizontal,
        Viewport
    }

    /// <summary>
    /// Defines the origin type of the SvgLength value
    /// </summary>
    public enum SvgLengthSource
    {
        Css,
        Xml,
        String
    }

    public sealed class SvgLength : ISvgLength
    {
        #region Private Fields

        private static readonly Regex ReUnit = new Regex(CssValue.LengthUnitPattern + "$");

        private string _defaultValue;
        private string _propertyName;

        private SvgLengthSource _source;
        private SvgElement _ownerElement;
        private SvgLengthDirection _direction;
        private CssAbsPrimitiveLengthValue _cssLength;

        #endregion

        #region Constructors

        public SvgLength(SvgElement ownerElement, string propertyName, SvgLengthSource source,
            SvgLengthDirection direction, string defaultValue)
        {
            _ownerElement = ownerElement;
            _propertyName = propertyName;
            _direction = direction;
            _defaultValue = defaultValue;
            _source = source;

            if (_source == SvgLengthSource.Xml || _source == SvgLengthSource.Css)
            {
                GetCssXmlValue();
            }
        }

        public SvgLength(SvgElement ownerElement, string propertyName, SvgLengthDirection direction, string baseVal)
            : this(ownerElement, propertyName, direction, baseVal, string.Empty)
        {
        }

        public SvgLength(SvgElement ownerElement, string propertyName, SvgLengthDirection direction,
            string baseVal, string defaultValue)
            : this(ownerElement, propertyName, SvgLengthSource.String, direction, defaultValue)
        {
            if (baseVal == null || baseVal.Length == 0)
            {
                baseVal = defaultValue;
            }

            baseVal = SvgNumber.ScientificToDec(baseVal);
            _cssLength = new CssAbsPrimitiveLengthValue(new CssPrimitiveLengthValue(baseVal, false),
                propertyName, ownerElement);
        }


        /// <summary>
        /// Creates a SvgLength value
        /// </summary>
        /// <param name="baseVal">String to be parsed into a length</param>
        /// <param name="ownerElement">The associated element</param>
        /// <param name="direction">Direction of the length, used for percentages</param>
        public SvgLength(string propertyName, string baseVal, SvgElement ownerElement, SvgLengthDirection direction)
        {
            _ownerElement = ownerElement;
            _direction = direction;

            baseVal = SvgNumber.ScientificToDec(baseVal);

            _cssLength = new CssAbsPrimitiveLengthValue(new CssPrimitiveLengthValue(baseVal, false),
                propertyName, ownerElement);
        }

        public SvgLength(string propertyName, string baseVal, string defaultValue,
            SvgElement ownerElement, SvgLengthDirection direction)
        {
            _ownerElement = ownerElement;
            _direction = direction;

            if (baseVal == null || baseVal.Length == 0)
            {
                baseVal = defaultValue;
            }

            baseVal = SvgNumber.ScientificToDec(baseVal);

            _cssLength = new CssAbsPrimitiveLengthValue(new CssPrimitiveLengthValue(baseVal, false),
                propertyName, ownerElement);
        }

        public SvgLength(string propertyName, SvgStyleableElement ownerElement, SvgLengthDirection direction, string defaultValue)
        {
            _ownerElement = ownerElement;
            _direction = direction;

            string baseVal = ownerElement.GetPropertyValue(propertyName);
            if (baseVal == null || baseVal == "")
            {
                baseVal = defaultValue;
            }

            baseVal = SvgNumber.ScientificToDec(baseVal);

            _cssLength = new CssAbsPrimitiveLengthValue(new CssPrimitiveLengthValue(baseVal, false), propertyName, ownerElement);
        }

        #endregion

        #region Public Properties

        public string PropertyName
        {
            get {
                return _propertyName;
            }
        }

        /// <summary>
        /// The type of the value as specified by one of the constants specified above. 
        /// </summary>
        public SvgLengthType UnitType
        {
            get {
                return (SvgLengthType)_cssLength.PrimitiveType;
            }
        }

        /// <summary>
        /// The value as an floating point value, in user units. Setting this attribute will cause 
        /// valueInSpecifiedUnits and valueAsString to be updated automatically to reflect this setting. 
        /// </summary>
        /// <exception cref="DomException"> NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change 
        /// the value of a readonly attribute.</exception>
        public double Value
        {
            get {
                double ret = 0;
                switch (UnitType)
                {
                    case SvgLengthType.Number:
                    case SvgLengthType.Px:
                    case SvgLengthType.Cm:
                    case SvgLengthType.Mm:
                    case SvgLengthType.In:
                    case SvgLengthType.Pt:
                    case SvgLengthType.Pc:
                    case SvgLengthType.Ems:
                    case SvgLengthType.Exs:
                        ret = _cssLength.GetFloatValue(CssPrimitiveType.Px);
                        break;
                    case SvgLengthType.Percentage:
                        double valueInSpecifiedUnits = _cssLength.GetFloatValue(CssPrimitiveType.Percentage);
                        // if (_ownerElement is SvgGradientElement)
                        if (string.Equals(_ownerElement.LocalName, "radialGradient", StringComparison.Ordinal)
                            || string.Equals(_ownerElement.LocalName, "linearGradient", StringComparison.Ordinal))
                        {
                            ret = valueInSpecifiedUnits / 100F;
                        }
                        else
                        {
                            double w = 0;
                            double h = 0;
                            if (_ownerElement.ViewportElement != null)
                            {
                                ISvgFitToViewBox ftv = (ISvgFitToViewBox)_ownerElement.ViewportElement;
                                w = ftv.ViewBox.AnimVal.Width;
                                h = ftv.ViewBox.AnimVal.Height;
                            }
                            else
                            {
                                w = _ownerElement.OwnerDocument.Window.InnerWidth;
                                h = _ownerElement.OwnerDocument.Window.InnerHeight;
                            }

                            if (_direction == SvgLengthDirection.Horizontal)
                            {
                                ret = valueInSpecifiedUnits * w / 100;
                            }
                            else if (_direction == SvgLengthDirection.Vertical)
                            {
                                ret = valueInSpecifiedUnits * h / 100;
                            }
                            else
                            {
                                ret = Math.Sqrt(w * w + h * h) / Math.Sqrt(2) * valueInSpecifiedUnits / 100;
                            }
                        }
                        break;
                    case SvgLengthType.Unknown:
                        throw new SvgException(SvgExceptionType.SvgInvalidValueErr, "Bad length unit");
                }
                if (double.IsNaN(ret))
                {
                    ret = 10;
                }
                return ret;
            }
            set {
                CssPrimitiveType oldType = _cssLength.PrimitiveType;
                _cssLength.SetFloatValue(CssPrimitiveType.Px, value);
                ConvertToSpecifiedUnits((SvgLengthType)oldType);
            }
        }

        /// <summary>
        /// The value as an floating point value, in the units expressed by unitType. Setting this 
        /// attribute will cause value and valueAsString to be updated automatically to reflect this setting.
        /// </summary>
        /// <exception cref="DomException"> NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change 
        /// the value of a readonly attribute.</exception>
        public double ValueInSpecifiedUnits
        {
            get {
                return _cssLength.GetFloatValue(_cssLength.PrimitiveType);
            }
            set {
                _cssLength.SetFloatValue(_cssLength.PrimitiveType, value);
            }
        }

        /// <summary>
        /// The value as a string value, in the units expressed by unitType. Setting this attribute will 
        /// cause value and valueInSpecifiedUnits to be updated automatically to reflect this setting.
        /// </summary>
        /// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change the 
        /// value of a readonly attribute.</exception>
        public string ValueAsString
        {
            get {
                return _cssLength.CssText;
            }
            set {
                _cssLength = new CssAbsPrimitiveLengthValue(new CssPrimitiveLengthValue(value, false), "", _ownerElement);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reset the value as a number with an associated unitType, thereby replacing the values for 
        /// all of the attributes on the object.
        /// </summary>
        /// <param name="unitType">The unitType for the value (e.g., MM). </param>
        /// <param name="valueInSpecifiedUnits">The new value</param>
        public void NewValueSpecifiedUnits(SvgLengthType unitType, double valueInSpecifiedUnits)
        {
            _cssLength.SetFloatValue((CssPrimitiveType)unitType, valueInSpecifiedUnits);
        }

        /// <summary>
        /// Preserve the same underlying stored value, but reset the stored unit identifier to the given unitType. 
        /// Object attributes unitType, valueAsSpecified and valueAsString might be modified as a result of this 
        /// method. For example, if the original value were "0.5cm" and the method was invoked to convert to 
        /// millimeters, then the unitType would be changed to MM, valueAsSpecified would be changed to the 
        /// numeric value 5 and valueAsString would be changed to "5mm". 
        /// </summary>
        /// <param name="unitType">The unitType to switch to (e.g., MM).</param>
        public void ConvertToSpecifiedUnits(SvgLengthType unitType)
        {
            double newValue = _cssLength.GetFloatValue((CssPrimitiveType)unitType);
            _cssLength.SetFloatValue((CssPrimitiveType)unitType, newValue);
        }

        #endregion

        #region Private Methods

        private void GetCssXmlValue()
        {
            if (_source == SvgLengthSource.Css)
            {
                ICssStyleDeclaration csd = _ownerElement.GetComputedStyle(string.Empty);
                CssPrimitiveLengthValue cssValue = csd.GetPropertyCssValue(_propertyName) as CssPrimitiveLengthValue;

                if (cssValue != null)
                {
                    _cssLength = new CssAbsPrimitiveLengthValue(cssValue, _propertyName, _ownerElement);
                }
                else
                {
                    throw new DomException(DomExceptionType.SyntaxErr, "Not a length value");
                }
            }
            else
            {
                string baseVal = _ownerElement.GetAttribute(_propertyName);

                if (baseVal == null || baseVal.Length == 0)
                {
                    baseVal = _defaultValue;
                }
                baseVal = SvgNumber.ScientificToDec(baseVal);
                _cssLength = new CssAbsPrimitiveLengthValue(new CssPrimitiveLengthValue(baseVal, false),
                    _propertyName, _ownerElement);
            }
        }

        #endregion
    }
}
