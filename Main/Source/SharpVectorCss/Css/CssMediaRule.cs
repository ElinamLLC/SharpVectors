// <developer>niklas@protocol7.com</developer>
// <completed>80</completed>

using System;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSMediaRule interface represents a @media rule in a CSS style sheet. A @media rule can be used to delimit style rules for specific media types.
	/// </summary>
	public class CssMediaRule : CssRule, ICssMediaRule
	{
		#region Static members

		private static Regex regex = new Regex(@"^@media\s(?<medianames>([a-z]+(\s*,\s*)?)+)");
		
		internal static CssRule Parse(ref string css, object parent, bool readOnly, string[] replacedStrings, CssStyleSheetType origin)
		{
			Match match = regex.Match(css);
			if(match.Success)
			{
				CssMediaRule rule = new CssMediaRule(match, parent, readOnly, replacedStrings, origin);

				css = css.Substring(match.Length);

				rule.cssRules = new CssRuleList(ref css, rule, replacedStrings, origin);

				return rule;
			}
			else
			{
				return null;
			}
		}
		
		internal static CssRule Parse(ref string css, object parent, bool readOnly, IList<string> replacedStrings, CssStyleSheetType origin)
		{
			Match match = regex.Match(css);
			if(match.Success)
			{
				CssMediaRule rule = new CssMediaRule(match, parent, readOnly, replacedStrings, origin);

				css = css.Substring(match.Length);

				rule.cssRules = new CssRuleList(ref css, rule, replacedStrings, origin);

				return rule;
			}
			else
			{
				return null;
			}
		} 

		#endregion

		#region Constructors
		/// <summary>
		/// The constructor for CssMediaRule
		/// </summary>
		/// <param name="match">The Regex match that found the charset rule</param>
		/// <param name="parent">The parent rule or parent stylesheet</param>
		/// <param name="readOnly">True if this instance is readonly</param>
		/// <param name="replacedStrings">An array of strings that have been replaced in the string used for matching. These needs to be put back use the DereplaceStrings method</param>
		/// <param name="origin">The type of CssStyleSheet</param>
		internal CssMediaRule(Match match, object parent, bool readOnly, 
            IList<string> replacedStrings, CssStyleSheetType origin) 
            : this(match.Groups["medianames"].Value, parent, readOnly, replacedStrings, origin)
		{
		}

		public CssMediaRule(string cssText, object parent, bool readOnly,
            IList<string> replacedStrings, CssStyleSheetType origin) 
            : base(parent, readOnly, replacedStrings, origin)
		{
			media = new MediaList(cssText);
		}

		#endregion

		#region Public methods
		/// <summary>
		/// Used to find matching style rules in the cascading order
		/// </summary>
		/// <param name="elt">The element to find styles for</param>
		/// <param name="pseudoElt">The pseudo-element to find styles for</param>
		/// <param name="ml">The medialist that the document is using</param>
		/// <param name="csd">A CssStyleDeclaration that holds the collected styles</param>
		protected internal override void GetStylesForElement(XmlElement elt, string pseudoElt, MediaList ml, CssCollectedStyleDeclaration csd)
		{
			if(media.Matches(ml))
			{
				((CssRuleList)CssRules).GetStylesForElement(elt, pseudoElt, ml, csd);
			}
		}
		#endregion

		#region Implementation of ICssMediaRule

		/// <summary>
		/// Used to delete a rule from the media block.
		/// </summary>
		/// <param name="index">The index within the media block's rule collection of the rule to remove.</param>
		/// <exception cref="DomException">INDEX_SIZE_ERR: Raised if the specified index does not correspond to a rule in the media rule list.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this media rule is readonly.</exception>
		public void DeleteRule(ulong index)
		{
			cssRules.DeleteRule(index);
		}

		/// <summary>
		/// Used to insert a new rule into the media block
		/// </summary>
		/// <param name="rule">The parsable text representing the rule. For rule sets this contains both the selector and the style declaration. For at-rules, this specifies both the at-identifier and the rule content.</param>
		/// <param name="index">The index within the media block's rule collection of the rule before which to insert the specified rule. If the specified index is equal to the length of the media blocks's rule collection, the rule will be added to the end of the media block.</param>
		/// <returns>The index within the media block's rule collection of the newly inserted rule.</returns>
		/// <exception cref="DomException">HIERARCHY_REQUEST_ERR: Raised if the rule cannot be inserted at the specified index, e.g., if an @import rule is inserted after a standard rule set or other at-rule.</exception>
		/// <exception cref="DomException">INDEX_SIZE_ERR: Raised if the specified index is not a valid insertion point.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this media rule is readonly</exception>
		/// <exception cref="DomException">SYNTAX_ERR: Raised if the specified rule has a syntax error and is unparsable</exception>
		public ulong InsertRule(string rule, ulong index)
		{
			throw new NotImplementedException("CssMediaRule.InsertRule()");
			//return cssRules.InsertRule(rule, index);
		}

		private CssRuleList cssRules;
		/// <summary>
		/// A list of all CSS rules contained within the media block.
		/// </summary>
		public ICssRuleList CssRules
		{
			get
			{
				return cssRules;
			}
		}

		private MediaList media;
		/// <summary>
		/// A list of media types for this rule
		/// </summary>
		public IMediaList Media
		{
			get
			{
				return media;
			}
		}
		#endregion

		#region Implementation of ICssRule
		/// <summary>
		/// The type of the rule. The expectation is that binding-specific casting methods can be used to cast down from an instance of the CSSRule interface to the specific derived interface implied by the type.
		/// </summary>
		public override CssRuleType Type
		{
			get
			{
				return CssRuleType.MediaRule;
			}
		}
		#endregion
	}
}
