using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// The CSSStyleRule interface represents a single rule set in a CSS style sheet.
    /// </summary>
    public class CssStyleRule : CssRule, ICssStyleRule
    {
        #region Static members

        internal static readonly string NsPattern           = @"([A-Za-z\*][A-Za-z0-9]*)?\|";
        internal static readonly string AttributeValueCheck = "(?<attname>(" + NsPattern + ")?[_a-zA-Z0-9\\-]+)\\s*(?<eqtype>[\\~\\^\\$\\*\\|]?)=\\s*(\"|\')?(?<attvalue>.*?)(\"|\')?";

        internal static readonly string RegexSelector = "(?<ns>" + NsPattern + ")?" +
            @"(?<type>([A-Za-z\*][A-Za-z0-9]*))?" +
            @"((?<class>\.[A-Za-z][_A-Za-z0-9\-]*)+)?" +
            @"(?<id>\#[A-Za-z][_A-Za-z0-9\-]*)?" +
            @"((?<predicate>\[\s*(" +
            @"(?<attributecheck>(" + NsPattern + ")?[a-zA-Z0-9]+)" +
            @"|" +
            "(?<attributevaluecheck>" + AttributeValueCheck + ")" +
            @")\s*\])+)?" +
            @"((?<pseudoclass>\:[a-z\-]+(\([^\)]+\))?)+)?" +
            @"(?<pseudoelements>(\:\:[a-z\-]+)+)?" +
            @"(?<seperator>(\s*(\+|\>|\~)\s*)|(\s+))?";

        private static readonly string StyleRule = "^((?<selector>(" + RegexSelector + @")+)(\s*,\s*)?)+";
        private static readonly Regex _reStyleRule = new Regex(StyleRule);
        
        #endregion

        #region Private Fields

        private CssXPathSelector[] _xPathSelectors;
        private CssStyleDeclaration _style;

        #endregion

        #region Constructors

        /// <summary>
        /// The constructor for CssStyleRule
        /// </summary>
        /// <param name="match">The Regex match that found the charset rule</param>
        /// <param name="parent">The parent rule or parent stylesheet</param>
        /// <param name="readOnly">True if this instance is readonly</param>
        /// <param name="replacedStrings">
        /// An array of strings that have been replaced in the string used for matching. 
        /// These needs to be put back use the DereplaceStrings method</param>
        /// <param name="origin">The type of CssStyleSheet</param>
        internal CssStyleRule(Match match, object parent, bool readOnly, IList<string> replacedStrings, CssStyleSheetType origin)
            : base(parent, readOnly, replacedStrings, origin)
        {
            //SelectorText = DeReplaceStrings(match.Groups["selectors"].Value.Trim());
            //_Style = new CssStyleDeclaration(match, this, readOnly, Origin);

            Group selectorMatches = match.Groups["selector"];

            int len = selectorMatches.Captures.Count;
            List<CssXPathSelector> sels = new List<CssXPathSelector>();
            for (int i = 0; i < len; i++)
            {
                string str = DeReplaceStrings(selectorMatches.Captures[i].Value.Trim());
                if (str.Length > 0)
                {
                    sels.Add(new CssXPathSelector(str));

                    // Validate selector and record diagnostics
                    ValidateSelectorAndRecordDiagnostics(str, parent);
                }
            }
            _xPathSelectors = sels.ToArray();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Used to find matching style rules in the cascading order
        /// </summary>
        /// <param name="elt">The element to find styles for</param>
        /// <param name="pseudoElt">The pseudo-element to find styles for</param>
        /// <param name="ml">The medialist that the document is using</param>
        /// <param name="csd">A CssStyleDeclaration that holds the collected styles</param>
        protected internal override void GetStylesForElement(XmlElement elt, string pseudoElt, 
            MediaList ml, CssCollectedStyleDeclaration csd)
        {
            XPathNavigator nav = elt.CreateNavigator();
            foreach (CssXPathSelector sel in _xPathSelectors)
            {
                // TODO: deal with pseudoElt
                if (sel != null && sel.Matches(nav))
                {
                    ((CssStyleDeclaration)Style).GetStylesForElement(csd, sel.Specificity);
                    break;
                }
            }
        }

        #endregion

        #region Internal Static Methods

        internal static CssRule Parse(ref string css, object parent, bool readOnly,
            IList<string> replacedStrings, CssStyleSheetType origin)
        {
            Match match = _reStyleRule.Match(css);
            if (match.Success && match.Length > 0)
            {
                CssStyleRule rule = new CssStyleRule(match, parent, readOnly, replacedStrings, origin);

                css = css.Substring(match.Length);

                if (string.IsNullOrWhiteSpace(css))
                {
                    rule._style = CssStyleDeclaration.EmptyCssStyle;
                }
                else
                {
                    rule._style = new CssStyleDeclaration(ref css, rule, readOnly, origin);
                }

                return rule;
            }
            return null;
        }

        #endregion

        #region Implementation of ICssStyleRule

        /// <summary>
        /// The textual representation of the selector for the rule set. The implementation may 
        /// have stripped out insignificant whitespace while parsing the selector.
        /// </summary>
        /// <exception cref="DomException">
        /// <c>SYNTAX_ERR:</c> Raised if the specified CSS string value has a syntax error and is unparsable.</exception>
        /// <exception cref="DomException"><c>NO_MODIFICATION_ALLOWED_ERR:</c> Raised if this rule is readonly</exception>
        public string SelectorText
        {
            get {
                string ret = string.Empty;
                foreach (CssXPathSelector sel in _xPathSelectors)
                {
                    ret += sel.CssSelector + ",";
                }
                return ret.Substring(0, ret.Length - 1);
            }
            set {
                // TODO: invalidate
                throw new NotImplementedException("setting SelectorText");

            }
        }

        /// <summary>
        /// The entire text of the CssStyleRule
        /// </summary>
        public override string CssText
        {
            get {
                return SelectorText + "{" + ((CssStyleDeclaration)Style).CssText + "}";
            }
        }

        /// <summary>
        /// The declaration-block of this rule set.
        /// </summary>
        public ICssStyleDeclaration Style
        {
            get {
                return _style;
            }
        }

        #endregion

        #region Implementation of ICssRule

        /// <summary>
        /// The type of the rule. The expectation is that binding-specific casting methods can be used to cast 
        /// down from an instance of the CSSRule interface to the specific derived interface implied by the type.
        /// </summary>
        public override CssRuleType Type
        {
            get {
                return CssRuleType.StyleRule;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates a selector and records any diagnostics in the parsing context if available.
        /// </summary>
        private static void ValidateSelectorAndRecordDiagnostics(string selector, object parent)
        {
            // Get the parsing context from the parent stylesheet if available
            CssParsingContext context = ExtractParsingContext(parent);
            if (context == null)
            {
                // No diagnostics context enabled, skip validation
                return;
            }

            // Analyze the selector
            var analysis = CssSelectorValidator.Analyze(selector);

            // Record issues based on severity
            if (!analysis.IsValid)
            {
                foreach (var issue in analysis.Issues)
                {
                    context.AddWarning($"Selector '{selector}': {issue}", CssWarningLevel.High);
                }
            }
            else if (analysis.SupportLevel == CssSelectorValidator.SelectorSupportLevel.Modern)
            {
                foreach (var issue in analysis.Issues)
                {
                    context.AddWarning($"Selector '{selector}': {issue}", CssWarningLevel.Medium);
                }
            }
            else if (analysis.Complexity > 15)
            {
                context.AddWarning(
                    $"Selector '{selector}' has high complexity ({analysis.Complexity}), may impact performance",
                    CssWarningLevel.Low);
            }
        }

        /// <summary>
        /// Extracts the parsing context from the parent object.
        /// </summary>
        private static CssParsingContext ExtractParsingContext(object parent)
        {
            // Try to get context from stylesheet
            CssStyleSheet stylesheet = null;

            if (parent is CssStyleSheet)
            {
                stylesheet = (CssStyleSheet)parent;
            }
            else if (parent is CssRule)
            {
                // Need to traverse up to find the stylesheet
                var rule = (CssRule)parent;
                // CssRule has _parentStyleSheet protected field, but we need to use reflection or 
                // access through public API if available
                // For now, we'll try to get it through properties
                var prop = parent.GetType().GetProperty("ParentStyleSheet", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (prop != null)
                {
                    stylesheet = prop.GetValue(parent, null) as CssStyleSheet;
                }
            }

            if (stylesheet != null)
            {
                return stylesheet.ParsingContext;
            }

            return null;
        }

        #endregion
    }
}

