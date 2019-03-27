using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSFontFaceRule interface represents a @font-face rule in a CSS style sheet. 
    /// The @font-face rule is used to hold a set of font descriptions.
	/// </summary>
	public class CssFontFaceRule : CssRule, ICssFontFaceRule
	{
		#region Private Fields

		private CssStyleDeclaration _style;

		#endregion

		#region Static members

		private static Regex regex = new Regex(@"^@font-face");
		
		/// <summary>
		/// Parses a string containging CSS and creates a CssFontFaceRule instance if found as the first content
		/// </summary>
		internal static CssRule Parse(ref string css, object parent, bool readOnly,
            IList<string> replacedStrings, CssStyleSheetType origin)
		{
			Match match = regex.Match(css);
			if (match.Success)
			{
				CssFontFaceRule rule = new CssFontFaceRule(parent, readOnly, replacedStrings, origin);
				css = css.Substring(match.Length);

				rule._style = new CssStyleDeclaration(ref css, rule, true, origin);

				return rule;
			}

			// didn't match => do nothing
			return null;
		}

		#endregion

		#region Constructors

		/// <summary>
		/// The constructor for CssFontFaceRule
		/// </summary>
		/// <param name="parent">The parent rule or parent stylesheet</param>
		/// <param name="readOnly">True if this instance is readonly</param>
		/// <param name="replacedStrings">An array of strings that have been replaced in the string used for matching. These needs to be put back use the DereplaceStrings method</param>
		/// <param name="origin">The type of CssStyleSheet</param>
		internal CssFontFaceRule(object parent, bool readOnly,
            IList<string> replacedStrings, CssStyleSheetType origin)
            : base(parent, readOnly, replacedStrings, origin)
		{
            // always read-only
            readOnly = true;
		}

		#endregion

		#region Implementation of ICssFontFaceRule

		/// <summary>
		/// The declaration-block of this rule.
		/// </summary>
		public ICssStyleDeclaration Style
		{
			get
			{
				return _style;
			}
		}

        public string FontUrl
        {
            get {
                if (_style != null)
                {
                    return CssStyleDeclaration.GetValidUrlFromCSS(_style.CssText, "src");
                }
                return null;
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
			get
			{
				return CssRuleType.FontFaceRule;
			}
		}

		#endregion
	}
}
