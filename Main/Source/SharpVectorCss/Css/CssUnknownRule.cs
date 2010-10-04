// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSUnknownRule interface represents an at-rule not supported by this user agent.
	/// </summary>
    public sealed class CssUnknownRule : CssRule, ICssUnknownRule
	{	
		#region Static members

		// TODO: should also find blocks
		private static Regex regex = new Regex(@"^@[^;]+;");

		internal static CssRule Parse(ref string css, object parent, bool readOnly,
            IList<string> replacedStrings, CssStyleSheetType origin)
		{
			Match match = regex.Match(css);
			if(match.Success)
			{
				CssUnknownRule rule = new CssUnknownRule(parent, readOnly, replacedStrings, origin);
				css = css.Substring(match.Length);
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
		/// The constructor for CssUnknownRule
		/// </summary>
		internal CssUnknownRule(object parent, bool readOnly,
            IList<string> replacedStrings, CssStyleSheetType origin)
            : base(parent, readOnly, replacedStrings, origin)
		{
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
				return CssRuleType.UnknownRule;
			}
		}
		#endregion
	}
}
