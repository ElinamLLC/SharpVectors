using System;
using System.Xml;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// This implements the <see cref="ICssValue"/> interface, which represents a simple or a complex value.
    /// A <see cref="ICssValue"/> object only occurs in a context of a CSS property.
    /// </summary>
    public class CssValue : ICssValue
    {
        #region Static members

        private readonly static string numberPattern    = @"[\-\+]?[0-9]*\.?[0-9]+";
        public readonly static string LengthUnitPattern = "(?<lengthUnit>in|cm|mm|px|em|ex|pc|pt|%)?";
        public readonly static string AngleUnitPattern  = "(?<angleUnit>deg|rad|grad)?";
        public readonly static string LengthPattern     = @"(?<lengthNumber>" + numberPattern + ")" + LengthUnitPattern;
        public readonly static string AnglePattern      = @"(?<angleNumber>" + numberPattern + ")" + AngleUnitPattern;

        private static string cssPrimValuePattern = @"^(?<primitiveValue>"
            + @"(?<func>(?<funcname>attr|url|counter|rect|rgb|rgba|hsl|hsla|var)\((?<funcvalue>[^\)]+)\))"
            + @"|(?<length>" + LengthPattern + ")"
            + @"|(?<angle>" + AnglePattern + ")"
            + @"|(?<freqTimeNumber>(?<numberValue2>" + numberPattern + ")(?<unit2>Hz|kHz|in|s|ms|%)?)"
            + @"|(?<string>[""'](?<stringvalue>(.|\n)*?)[""'])"
            + @"|(?<colorIdent>([A-Za-z]+)|(\#[A-Fa-f0-9]{6})|(\#[A-Fa-f0-9]{3}))"
            + @")";

        private readonly static Regex _reCssPrimitiveValue = new Regex(cssPrimValuePattern + "$");
        private readonly static Regex _reCssValueList      = new Regex(cssPrimValuePattern + @"(\s*,\s*)+$");

        #endregion

        #region Private Fields

        private bool _readOnly;

        private string _cssText;
        protected CssValueType _cssValueType;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for CssValue
        /// </summary>
        /// <param name="type">The type of value</param>
        /// <param name="cssText">The entire content of the value</param>
        /// <param name="readOnly">Specifies if the instance is read-only</param>
        public CssValue(CssValueType type, string cssText, bool readOnly)
        {
            _cssText      = cssText.Trim();
            _cssValueType = type;
            _readOnly     = readOnly;
        }

        /// <summary>
        /// Only for internal use
        /// </summary>
        protected CssValue()
        {
            _readOnly = true;
        }

        #endregion

        #region Public Methods

        public virtual CssValue GetAbsoluteValue(string propertyName, XmlElement elm)
        {
            return new CssAbsValue(this, propertyName, elm);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Detects what kind of value cssText contains and returns an instance of the correct CssValue class
        /// </summary>
        /// <param name="cssText">The text to parse for a CSS value</param>
        /// <param name="readOnly">Specifies if this instance is read-only</param>
        /// <returns>The correct type of CSS value</returns>
        public static CssValue GetCssValue(string cssText, bool readOnly)
        {
            if (cssText == "inherit")
            {
                // inherit
                return new CssValue(CssValueType.Inherit, cssText, readOnly);
            }
            Match match = _reCssPrimitiveValue.Match(cssText);
            if (match.Success)
            {
                // single primitive value
                return CssPrimitiveValue.Create(match, readOnly);
            }

            match = _reCssValueList.Match(cssText);
            if (match.Success)
            {
                // list of primitive values
                throw new NotImplementedException("Value lists not implemented");
            }
            // custom value
            return new CssValue(CssValueType.Custom, cssText, readOnly);
        }

        #endregion

        #region Public Properties

        public virtual bool ReadOnly
        {
            get {
                return _readOnly;
            }
        }

        #endregion

        #region ICssValue Members

        /// <summary>
        /// A string representation of the current value.
        /// </summary>
        /// <exception cref="DomException">
        /// SYNTAX_ERR: Raised if the specified CSS string value has a syntax error 
        /// (according to the attached property) or is unparsable.
        /// </exception>
        /// <exception cref="DomException">
        /// INVALID_MODIFICATION_ERR: Raised if the specified CSS string value represents a different 
        /// type of values than the values allowed by the CSS property
        /// </exception>
        /// <exception cref="DomException">
        /// NO_MODIFICATION_ALLOWED_ERR: Raised if this value is readonly.
        /// </exception>
        public virtual string CssText
        {
            get {
                return _cssText;
            }
            set {
                if (ReadOnly)
                {
                    throw new DomException(DomExceptionType.InvalidModificationErr, "The CssValue is read-only");
                }
                _cssText = value;
            }
        }

        /// <summary>
        /// A code defining the type of the value as defined above
        /// </summary>
        public virtual CssValueType CssValueType
        {
            get {
                return _cssValueType;
            }
        }

        public virtual bool IsAbsolute
        {
            get {
                return false;
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(_cssText))
            {
                return _cssText;
            }

            return base.ToString();
        }

        #endregion
    }
}
