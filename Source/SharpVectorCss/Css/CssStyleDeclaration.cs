using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// <para>
    /// The <see cref="ICssStyleDeclaration"/> interface represents a single CSS declaration block. 
    /// This interface may be used to determine the style properties currently set in a block or 
    /// to set style properties explicitly within the block.
    /// </para>
    /// <para>
    ///	While an implementation may not recognize all CSS properties within a CSS declaration block, 
    ///	it is expected to provide access to all specified properties in the style sheet through the 
    ///	<see cref="ICssStyleDeclaration"/> interface. Furthermore, implementations that support a 
    ///	specific level of CSS should correctly handle CSS shorthand properties for that level. 
    ///	For a further discussion of shorthand properties, see the CSS2Properties interface.
    /// </para>
    /// <para>
    /// This interface is also used to provide a read-only access to the computed values of an element. 
    /// See also the ViewCSS interface.
    /// </para>
    /// <para>
    /// Note: The CSS Object Model doesn't provide an access to the specified or actual values of the CSS cascade
    /// </para>
    /// </summary>
    public class CssStyleDeclaration : ICssStyleDeclaration
    {
        #region Static Members

        [ThreadStatic]
        private static CssStyleDeclaration _emptyCssStyle;

        internal const string UrlName     = "url:Name";
        internal const string UrlMime     = "url:Mime";
        internal const string UrlData     = "url:Data";
        internal const string UrlEncoding = "url:Encoding";

        private static readonly Regex _reComment = new Regex(@"(//.*)|(/\*(.|\n)*?\*/)");
        private static readonly Regex _styleRegex = new Regex(
            @"^(?<name>[A-Za-z\-0-9]+)\s*:(?<value>[^;\}!]+)(!\s?(?<priority>important))?;?");
        // We'll use your regex for extracting the valid URLs
        private static readonly Regex _reUrls = new Regex(@"(?nx)
                    url \s* \( \s*
                        (
                            (?! ['""] )
                            (?<Url> [^\)]+ )
                            (?<! ['""] )
                            |
                            (?<Quote> ['""] )
                            (?<Url> .+? )
                            \k<Quote>
                        )
                    \s* \)");

        private static readonly Regex _reSplitCss = new Regex(@"([^:\s]+)*\s*:\s*([^;]+);");
        private static readonly Regex _reSplitCssOther = new Regex(@"({|;)([^:{;]+:[^;}]+)(;|})");

        private static readonly Regex _reUrlTidy = new Regex(@"(^|{|})(\\s*{[^}]*})");

        private static readonly Regex _reEmbeddedUrl = new Regex(@"^(?<name>[A-Za-z\-0-9]+)\s*:\s*url\(data:(?<mime>[\w/\-\.]+);(?<encoding>\w+),(?<data>[^;\}!]+)(!\s?(?<priority>important))?;?");

        // Enter properties that can validly contain a URL here (in lowercase):
        private static readonly ISet<string> _validUrlProps = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "src", "background", "background-image"
        };

        #endregion

        #region Private Fields

        private bool _readOnly;
        private CssStyleSheetType _origin;
        private IDictionary<string, CssStyleBlock> _styles;
        private ICssRule _parentRule;

        #endregion

        #region Constructors

        /// <summary>
        /// The constructor used internally when collecting styles for a specified element
        /// </summary>
        protected CssStyleDeclaration()
        {
            _origin     = CssStyleSheetType.Collector;
            _readOnly   = true;
            _parentRule = null;
            _styles     = new Dictionary<string, CssStyleBlock>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// The constructor for CssStyleDeclaration
        /// </summary>
        /// <param name="css">The string to parse for CSS</param>
        /// <param name="parentRule">The parent rule or parent stylesheet</param>
        /// <param name="readOnly">True if this instance is readonly</param>
        /// <param name="origin">The type of CssStyleSheet</param>
        public CssStyleDeclaration(ref string css, CssRule parentRule, bool readOnly, CssStyleSheetType origin)
            : this()
        {
            _origin     = origin;
            _readOnly   = readOnly;
            _parentRule = parentRule;

            css         = ParseString(css);
        }

        public CssStyleDeclaration(string css, CssRule parentRule, bool readOnly, CssStyleSheetType origin)
            : this()
        {
            _origin     = origin;
            _readOnly   = readOnly;
            _parentRule = parentRule;

            ParseString(css);
        }

        #endregion

        #region Public Properties

        public bool ReadOnly
        {
            get {
                return _readOnly;
            }
        }

        public CssStyleSheetType Origin
        {
            get {
                return _origin;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
		/// Used to find matching style rules in the cascading order
		/// </summary>
		public void GetStylesForElement(CssCollectedStyleDeclaration csd, int specificity)
        {
            foreach (KeyValuePair<string, CssStyleBlock> de in _styles)
            {
                CssStyleBlock scs = de.Value;
                csd.CollectProperty(scs.Name, specificity,
                    (CssValue)GetPropertyCssValue(scs.Name), scs.Origin, scs.Priority);
            }
        }

        /// <summary>
        /// Parsing CSS in C#: extracting all URLs
        /// </summary>
        /// <param name="cssStr"></param>
        /// <param name="validProperty"></param>
        /// <returns></returns>
        /// <remarks>
        /// https://stackoverflow.com/questions/18262390/parsing-css-in-c-extracting-all-urls
        /// </remarks>
        public static string GetValidUrlFromCSS(string cssStr, string validProperty)
        {
            // First, remove all the comments
            cssStr = _reComment.Replace(cssStr, string.Empty).Trim();
            // Next remove all the the property groups with no selector
            string oldStr;
            do
            {
                oldStr = cssStr;
                cssStr = _reUrlTidy.Replace(cssStr, "$1");
            } while (cssStr != oldStr);
            // Get properties
            var matches = _reSplitCss.Matches(cssStr);
            foreach (Match match in matches)
            {
                // Matches: (0)=src:url(woffs/ZCSB.woff) format("woff"); | (1)=src | (2)=url(woffs/ZCSB.woff) format("woff") 
                if (match.Groups != null && match.Groups.Count == 3)
                {
                    if (string.Equals(validProperty, match.Groups[1].Value, StringComparison.OrdinalIgnoreCase))
                    {
                        // Since this is a valid property, extract the URL (if there is one)
                        MatchCollection validUrlCollection = _reUrls.Matches(match.Groups[2].Value);
                        if (validUrlCollection.Count > 0)
                        {
                            return validUrlCollection[0].Groups["Url"].Value;
                        }
                    }
                }
            }

            // Get properties
            matches = _reSplitCssOther.Matches(cssStr);
            foreach (Match match in matches)
            {
                string matchVal = match.Groups[2].Value;
                string[] matchArr = matchVal.Split(':');
                if (string.Equals(validProperty, matchArr[0].Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    // Since this is a valid property, extract the URL (if there is one)
                    MatchCollection validUrlCollection = _reUrls.Matches(matchVal);
                    if (validUrlCollection.Count > 0)
                    {
                        return validUrlCollection[0].Groups["Url"].Value;
                    }
                }
            }
            return null;
        }

        public static IList<string> GetValidUrlsFromCSS(string cssStr)
        {
            List<string> validUrls = new List<string>();
            // First, remove all the comments
            cssStr = _reComment.Replace(cssStr, string.Empty).Trim();
            // Next remove all the the property groups with no selector
            string oldStr;
            do
            {
                oldStr = cssStr;
                cssStr = _reUrlTidy.Replace(cssStr, "$1");
            } while (cssStr != oldStr);

            // Get properties
            var matches = _reSplitCss.Matches(cssStr);
            foreach (Match match in matches)
            {
                // Matches: (0)=src:url(woffs/ZCSB.woff) format("woff"); | (1)=src | (2)=url(woffs/ZCSB.woff) format("woff") 
                if (match.Groups != null && match.Groups.Count == 3)
                {
                    if (_validUrlProps.Contains(match.Groups[1].Value))
                    {
                        // Since this is a valid property, extract the URL (if there is one)
                        MatchCollection validUrlCollection = _reUrls.Matches(match.Groups[2].Value);
                        if (validUrlCollection.Count > 0)
                        {
                            validUrls.Add(validUrlCollection[0].Groups["Url"].Value);
                        }
                    }
                }
            }
            if (validUrls.Count != 0)
            {
                return validUrls;
            }

            // Get properties
            matches = _reSplitCssOther.Matches(cssStr);
            foreach (Match match in matches)
            {
                string matchVal = match.Groups[2].Value;
                string[] matchArr = matchVal.Split(':');
                if (_validUrlProps.Contains(matchArr[0].Trim()))
                {
                    // Since this is a valid property, extract the URL (if there is one)
                    MatchCollection validUrlCollection = _reUrls.Matches(matchVal);
                    if (validUrlCollection.Count > 0)
                    {
                        validUrls.Add(validUrlCollection[0].Groups["Url"].Value);
                    }
                }
            }

            return validUrls;
        }

        #endregion

        #region Private Methods

        private string ParseString(string cssText)
        {
            if (string.IsNullOrWhiteSpace(cssText))
            {
                return string.Empty;
            }
            bool startedWithABracket = false;

            cssText = cssText.Trim();
            if (cssText.StartsWith("{", StringComparison.OrdinalIgnoreCase))
            {
                cssText = cssText.Substring(1).Trim();
                startedWithABracket = true;
            }

            var quotes = new char[2] { '\'', '"' };

            Match match = _styleRegex.Match(cssText);
            while (match.Success)
            {
                string name  = match.Groups["name"].Value.Trim();
                string value = match.Groups["value"].Value.Trim();
                if (_parentRule != null)
                {
                    value = ((CssRule)_parentRule).DeReplaceStrings(value);
                }
                value = value.Trim(quotes);

                if (value.StartsWith("url(", StringComparison.OrdinalIgnoreCase))
                {
                    if (value.IndexOf("data:", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var matchUrl = _reEmbeddedUrl.Match(cssText);
                        if (matchUrl != null && matchUrl.Groups != null && matchUrl.Groups.Count >= 3)
                        {
                            var nameUrl     = matchUrl.Groups["name"].Value;
                            var mimeUrl     = matchUrl.Groups["mime"].Value;
                            var encodingUrl = matchUrl.Groups["encoding"].Value;
                            var dataUrl     = matchUrl.Groups["data"].Value;

                            if (string.Equals(name, nameUrl, StringComparison.Ordinal) 
                                && !string.IsNullOrWhiteSpace(mimeUrl)
                                && !string.IsNullOrWhiteSpace(encodingUrl)
                                && !string.IsNullOrWhiteSpace(dataUrl))
                            {
                                value = matchUrl.Groups[0].Value.Remove(0, name.Length + 1).TrimEnd(';');
                                match = matchUrl;

                                int foundAt = dataUrl.IndexOf(")", StringComparison.Ordinal);

                                if (!_styles.ContainsKey(UrlName) && foundAt > 0)
                                {
                                    _styles.Add(UrlName,     new CssStyleBlock(UrlName, nameUrl, string.Empty, _origin));
                                    _styles.Add(UrlMime,     new CssStyleBlock(UrlMime, mimeUrl, string.Empty, _origin));
                                    _styles.Add(UrlData,     new CssStyleBlock(UrlData, dataUrl.Substring(0, foundAt), string.Empty, _origin));
                                    _styles.Add(UrlEncoding, new CssStyleBlock(UrlEncoding, encodingUrl, string.Empty, _origin));
                                }
                            }
                        }
                    }
                }
                string prio = match.Groups["priority"].Value;

                CssStyleBlock style = new CssStyleBlock(name, value, prio, _origin);

                bool addStyle = false;
                if (_styles.ContainsKey(name))
                {
                    string existingPrio = _styles[name].Priority;

                    if (existingPrio != "important" || prio == "important")
                    {
                        _styles.Remove(name);
                        addStyle = true;
                    }
                }
                else
                {
                    addStyle = true;
                }

                if (addStyle)
                {
                    _styles.Add(name, style);
                }

                cssText = cssText.Substring(match.Length).Trim();
                match   = _styleRegex.Match(cssText);
            }

            cssText = cssText.Trim();
            if (cssText.StartsWith("}", StringComparison.OrdinalIgnoreCase))
            {
                cssText = cssText.Substring(1);
            }
            else if (startedWithABracket)
            {
                throw new DomException(DomExceptionType.SyntaxErr, "Style declaration ending bracket missing");
            }
            return cssText;
        }

        #endregion

        #region ICssStyleDeclaration Members

        /// <summary>
        /// Used to set a property value and priority within this declaration block
        /// </summary>
        /// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
        /// <param name="value">The new value of the property.</param>
        /// <param name="priority">The new priority of the property (e.g. "important").</param>
        /// <exception cref="DomException">SYNTAX_ERR: Raised if the specified value has a syntax error and is unparsable.</exception>
        /// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this declaration is readonly or the property is readonly.</exception>
        public void SetProperty(string propertyName, string value, string priority)
        {
            if (_readOnly)
                throw new DomException(DomExceptionType.NoModificationAllowedErr);

            _styles[propertyName] = new CssStyleBlock(propertyName, value, priority, _origin);
        }

        /// <summary>
        /// Used to set a property value and priority within this declaration block
        /// </summary>
        /// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
        /// <param name="value">The new value of the property.</param>
        /// <exception cref="DomException">SYNTAX_ERR: Raised if the specified value has a syntax error and is unparsable.</exception>
        /// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this declaration is readonly or the property is readonly.</exception>
        public void SetPropertyValue(string propertyName, string value)
        {
            if (_readOnly)
                throw new DomException(DomExceptionType.NoModificationAllowedErr);
            if (_styles == null || _styles.ContainsKey(propertyName) == false)
            {
                return;
            }

            var styleBlock = _styles[propertyName];
            if (styleBlock != null)
            {
                styleBlock.Value = value;
            }
        }

        /// <summary>
        /// Used to retrieve the priority of a CSS property (e.g. the "important" qualifier) if the property 
        /// has been explicitly set in this declaration block.
        /// </summary>
        /// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
        /// <returns>A string representing the priority (e.g. "important") if one exists. The empty string if none exists.</returns>
        public virtual string GetPropertyPriority(string propertyName)
        {
            return (_styles.ContainsKey(propertyName)) ? _styles[propertyName].Priority : string.Empty;
        }

        /// <summary>
        /// Used to remove a CSS property if it has been explicitly set within this declaration block.
        /// </summary>
        /// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
        /// <returns>Returns the value of the property if it has been explicitly set for this declaration block. 
        /// Returns the empty string if the property has not been set or the property name does not correspond 
        /// to a known CSS property.</returns>
        /// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this declaration is readonly 
        /// or the property is readonly.</exception>
        public string RemoveProperty(string propertyName)
        {
            if (_readOnly)
                throw new DomException(DomExceptionType.NoModificationAllowedErr);

            if (_styles.ContainsKey(propertyName))
            {
                CssStyleBlock s = _styles[propertyName];
                _styles.Remove(propertyName);
                return s.Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// Used to retrieve the object representation of the value of a CSS property if it has been explicitly set 
        /// within this declaration block. This method returns null if the property is a shorthand property. 
        /// Shorthand property values can only be accessed and modified as strings, using the getPropertyValue and 
        /// setProperty methods.
        /// </summary>
        /// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
        /// <returns>Returns the value of the property if it has been explicitly set for this declaration block. 
        /// Returns null if the property has not been set.</returns>
        public virtual ICssValue GetPropertyCssValue(string propertyName)
        {
            if (_styles.ContainsKey(propertyName))
            {
                CssStyleBlock scs = _styles[propertyName];
                if (scs.CssValue == null)
                {
                    scs.CssValue = CssValue.GetCssValue(scs.Value, ReadOnly);
                }
                return scs.CssValue;
            }
            return null;
        }

        /// <summary>
        /// Used to retrieve the value of a CSS property if it has been explicitly set within this declaration block.
        /// </summary>
        /// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
        /// <returns>Returns the value of the property if it has been explicitly set for this declaration block. 
        /// Returns the empty string if the property has not been set.</returns>
        public virtual string GetPropertyValue(string propertyName)
        {
            return (_styles.ContainsKey(propertyName)) ? _styles[propertyName].Value.Trim('\'') : string.Empty;
        }

        /// <summary>
        /// Used to retrieve the value of a CSS property if it has been explicitly set within this declaration block.
        /// </summary>
        /// <param name="propertyNames">The name of the CSS property. See the CSS property index.</param>
        /// <returns>Returns the value of the property if it has been explicitly set for this declaration block. 
        /// Returns the empty string if the property has not been set.</returns>
        public virtual string GetPropertyValue(string[] propertyNames)
        {
            if (propertyNames == null || propertyNames.Length == 0)
            {
                return string.Empty;
            }
            foreach (var propertyName in propertyNames)
            {
                if (_styles.ContainsKey(propertyName))
                    return _styles[propertyName].Value.Trim('\'');
            }
            return string.Empty;
        }

        /// <summary>
        /// The CSS rule that contains this declaration block or null if this CSSStyleDeclaration is not attached to a CSSRule.
        /// </summary>
        public ICssRule ParentRule
        {
            get {
                return _parentRule;
            }
        }

        /// <summary>
        /// The number of properties that have been explicitly set in this declaration block. 
        /// The range of valid indices is 0 to length-1 inclusive.
        /// </summary>
        public virtual ulong Length
        {
            get {
                return (ulong)_styles.Count;
            }
        }

        /// <summary>
        /// The parsable textual representation of the declaration block (excluding the surrounding curly braces). 
        /// Setting this attribute will result in the parsing of the new value and resetting of all the properties 
        /// in the declaration block including the removal or addition of properties.
        /// </summary>
        /// <exception cref="DomException">SYNTAX_ERR: Raised if the specified CSS string value has a syntax error and is unparsable.</exception>
        /// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this declaration is readonly or a property is readonly.</exception>
        public virtual string CssText
        {
            get {
                StringBuilder builder = new StringBuilder();

                IEnumerator<KeyValuePair<string, CssStyleBlock>> enu = _styles.GetEnumerator();
                while (enu.MoveNext())
                {
                    CssStyleBlock style = enu.Current.Value;
                    builder.Append(style.CssText);
                    builder.Append(";");
                }

                return builder.ToString();
            }
            set {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Used to retrieve the properties that have been explicitly set in this declaration block. 
        /// The order of the properties retrieved using this method does not have to be the order in which they were set. 
        /// This method can be used to iterate over all properties in this declaration block.
        /// The name of the property at this ordinal position. The empty string if no property exists at this position.
        /// </summary>
        public virtual string this[ulong index]
        {
            get {
                if (index >= Length)
                    return string.Empty;

                int ind = (int)index;
                IEnumerator<KeyValuePair<string, CssStyleBlock>> iterator = _styles.GetEnumerator();
                iterator.MoveNext();
                KeyValuePair<string, CssStyleBlock> enu = iterator.Current;
                for (int i = 0; i < ind; i++)
                {
                    iterator.MoveNext();
                    enu = iterator.Current;
                }

                return enu.Key;
            }
        }

        #endregion

        #region Internal Properties and Methods

        internal static CssStyleDeclaration EmptyCssStyle
        {
            get {
                if (_emptyCssStyle == null)
                {
                    _emptyCssStyle = new CssStyleDeclaration();
                }

                return _emptyCssStyle;
            }
        }

        internal CssStyleBlock Get(string key)
        {
            if (_styles != null && _styles.Count != 0 && _styles.ContainsKey(key))
            {
                return _styles[key];
            }
            return null;
        }

        internal string GetValue(string key)
        {
            if (_styles != null && _styles.Count != 0 && _styles.ContainsKey(key))
            {
                return _styles[key].Value;
            }
            return null;
        }

        internal bool Contains(string key)
        {
            if (_styles != null && _styles.Count != 0)
            {
                return _styles.ContainsKey(key);
            }
            return false;
        }

        #endregion
    }
}
