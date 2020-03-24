using System;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// The <see cref="ICssStyleSheet"/> interface is a concrete interface used to represent a CSS style sheet i.e., 
    /// a style sheet whose content type is "text/css".
    /// </summary>
    public class CssStyleSheet : StyleSheet, ICssStyleSheet
    {
        #region Private fields

        //private static readonly Regex _reComment = new Regex(@"(//.*)|(/\*(.|\n)*?\*/)");
        private static readonly Regex _reComment = new Regex(@"(?<!"")\/\*.+?\*\/(?!"")");
        private static readonly Regex _reEscape  = new Regex(@"(""(.|\n)*?[^\\]"")|('(.|\n)*?[^\\]')");

        private readonly CssStyleSheetType _origin;
        private IList<string> _alReplacedStrings = new List<string>();

        private CssRuleList _cssRules;
        private CssRule _ownerRule;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for CssStyleSheet
        /// </summary>
        /// <param name="pi">The XML processing instruction that references the stylesheet</param>
        /// <param name="origin">The type of stylesheet</param>
        internal CssStyleSheet(XmlProcessingInstruction pi, CssStyleSheetType origin)
            : base(pi)
        {
            _origin = origin;
        }

        /// <summary>
        /// Constructor for CssStyleSheet
        /// </summary>
        /// <param name="styleElement">The XML style element that references the stylesheet</param>
        /// <param name="origin">The type of stylesheet</param>
        internal CssStyleSheet(XmlElement styleElement, CssStyleSheetType origin)
            : base(styleElement)
        {
            _origin = origin;
        }

        /// <summary>
        /// Constructor for CssStyleSheet
        /// </summary>
        /// <param name="ownerNode">The node that owns this stylesheet. E.g. used for getting the BaseUri</param>
        /// <param name="href">The URL of the stylesheet</param>
        /// <param name="title">The title of the stylesheet</param>
        /// <param name="media">List of medias for the stylesheet</param>
        /// <param name="ownerRule">The rule (e.g. ImportRule) that referenced this stylesheet</param>
        /// <param name="origin">The type of stylesheet</param>
        public CssStyleSheet(XmlNode ownerNode, string href, string title, string media,
            CssRule ownerRule, CssStyleSheetType origin)
            : base(ownerNode, href, "text/css", title, media)
        {
            _origin    = origin;
            _ownerRule = ownerRule;
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
            if (((MediaList)Media).Matches(ml))
            {
                ((CssRuleList)CssRules).GetStylesForElement(elt, pseudoElt, ml, csd);
            }
        }

        #endregion

        #region Private methods

        private string StringReplaceEvaluator(Match match)
        {
            _alReplacedStrings.Add(match.Value);

            return "\"<<<" + (_alReplacedStrings.Count - 1) + ">>>\"";
        }

        private string PreProcessContent()
        {
            var styleContent = this.SheetContent;
            if (!string.IsNullOrWhiteSpace(styleContent))
            {
                // "escape" strings, eg: "foo" => "<<<number>>>"			
                _alReplacedStrings.Clear();
                string s = _reEscape.Replace(styleContent, new MatchEvaluator(StringReplaceEvaluator));

                // remove dual semicolon, which may affect the parsing...
                s = s.Replace(";;", ";");

                // remove comments
                return _reComment.Replace(s, string.Empty).Trim();
            }
            return string.Empty;
        }

        #endregion

        #region Implementation of ICssStyleSheet

        /// <summary>
        /// Used to delete a rule from the style sheet.
        /// </summary>
        /// <param name="index">The index within the style sheet's rule list of the rule to remove.</param>
        /// <exception cref="DomException">
        /// INDEX_SIZE_ERR: Raised if the specified index does not correspond to a rule in the style sheet's rule list.
        /// </exception>
        /// <exception cref="DomException">
        /// NO_MODIFICATION_ALLOWED_ERR: Raised if this style sheet is readonly.
        /// </exception>
        public void DeleteRule(ulong index)
        {
            ((CssRuleList)CssRules).DeleteRule(index);
        }

        /// <summary>
        /// Used to insert a new rule into the style sheet. The new rule now becomes part of the cascade.
        /// </summary>
        /// <param name="rule">
        /// The parsable text representing the rule. For rule sets this contains both the selector and the 
        /// style declaration. For at-rules, this specifies both the at-identifier and the rule content.
        /// </param>
        /// <param name="index">
        /// The index within the style sheet's rule list of the rule before which to insert the specified rule. 
        /// If the specified index is equal to the length of the style sheet's rule collection, the rule will 
        /// be added to the end of the style sheet.
        /// </param>
        /// <returns>The index within the style sheet's rule collection of the newly inserted rule.</returns>
        /// <exception cref="DomException">
        /// INDEX_SIZE_ERR: Raised if the specified index does not correspond to a rule in the style sheet's rule list.
        /// </exception>
        /// <exception cref="DomException">
        /// NO_MODIFICATION_ALLOWED_ERR: Raised if this style sheet is readonly.
        /// </exception>
        /// <exception cref="DomException">
        /// HIERARCHY_REQUEST_ERR: Raised if the rule cannot be inserted at the specified index 
        /// e.g. if an @import rule is inserted after a standard rule set or other at-rule.
        /// </exception>
        /// <exception cref="DomException">
        /// SYNTAX_ERR: Raised if the specified rule has a syntax error and is unparsable.
        /// </exception>
        public ulong InsertRule(string rule, ulong index)
        {
            throw new NotImplementedException("CssStyleSheet.InsertRule()");
            //return ((CssRuleList)CssRules).InsertRule(rule, index);
        }

        /// <summary>
        /// The list of all CSS rules contained within the style sheet. This includes both rule sets and at-rules.
        /// </summary>
        public ICssRuleList CssRules
        {
            get {
                if (_cssRules == null)
                {
                    string css = PreProcessContent();
                    _cssRules = new CssRuleList(ref css, this, _alReplacedStrings, _origin);
                }

                return _cssRules;
            }
            set {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// If this style sheet comes from an @import rule, the ownerRule attribute will contain 
        /// the CSSImportRule. In that case, the ownerNode attribute in the StyleSheet interface 
        /// will be null. If the style sheet comes from an element or a processing instruction, 
        /// the ownerRule attribute will be null and the ownerNode attribute will contain the Node.
        /// </summary>
        public ICssRule OwnerRule
        {
            get {
                return _ownerRule;
            }
        }

        #endregion
    }
}
