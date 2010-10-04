// <developer>niklas@protocol7.com</developer>
// <completed>80</completed>

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSPageRule interface represents a @page rule within a CSS style sheet. The @page rule is used to specify the dimensions, orientation, margins, etc. of a page box for paged media.
	/// </summary>
    public sealed class CssPageRule : CssRule, ICssPageRule
	{
		#region Static members
		private static Regex regex = new Regex(@"^@font-face");

        internal static CssRule Parse(ref string css, object parent, bool readOnly, 
            IList<string> replacedStrings, CssStyleSheetType origin)
		{
			Match match = regex.Match(css);
			if(match.Success)
			{
				CssPageRule rule = new CssPageRule(match, parent, readOnly, replacedStrings, origin);
				css = css.Substring(match.Length);

				rule.style = new CssStyleDeclaration(ref css, rule, true, origin);

				return rule;
			}
			else
			{
				// didn't match => do nothing
				return null;
			}
		}
		#endregion

		#region Constructors

		/// <summary>
		/// The constructor for CssPageRule
		/// </summary>
		/// <param name="match">The Regex match that found the charset rule</param>
		/// <param name="parent">The parent rule or parent stylesheet</param>
		/// <param name="readOnly">True if this instance is readonly</param>
		/// <param name="replacedStrings">An array of strings that have been replaced in the string used for matching. These needs to be put back use the DereplaceStrings method</param>
		/// <param name="origin">The type of CssStyleSheet</param>
        internal CssPageRule(Match match, object parent, bool readOnly, 
            IList<string> replacedStrings, CssStyleSheetType origin)
            : base(parent, readOnly, replacedStrings, origin)
		{
			// TODO: selectorText = DeReplaceStrings(match.Groups["pageselector"].Value.Trim());
		}

		#endregion

		#region Implementation of ICssPageRule

		private string selectorText;
		/// <summary>
		/// The parsable textual representation of the page selector for the rule.
		/// </summary>
		/// <exception cref="DomException">SYNTAX_ERR: Raised if the specified CSS string value has a syntax error and is unparsable.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this rule is readonly.</exception>
		public string SelectorText
		{
			get
			{
				return selectorText;
			}
			set
			{
				/*    SYNTAX_ERR: Raised if the specified CSS string value has a syntax error and is unparsable.*/
				if (readOnly) 
                    throw new DomException(DomExceptionType.NoModificationAllowedErr);

				selectorText = value;
			}
		}

		private CssStyleDeclaration style;
		/// <summary>
		/// The declaration-block of this rule.
		/// </summary>
		public ICssStyleDeclaration Style
		{
			get
			{
				return style;
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
				return CssRuleType.PageRule;
			}
		}
		#endregion
	}
}
