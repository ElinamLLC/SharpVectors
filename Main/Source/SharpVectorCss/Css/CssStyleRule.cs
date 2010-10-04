using System;
using System.Xml;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using SharpVectors.Dom.Stylesheets;
using System.Collections;
using System.Collections.Generic;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSStyleRule interface represents a single rule set in a CSS style sheet.
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>80</completed>
	public class CssStyleRule : CssRule,ICssStyleRule
	{
		#region Static members

		internal static string nsPattern = @"([A-Za-z\*][A-Za-z0-9]*)?\|";
		internal static string attributeValueCheck = "(?<attname>(" + nsPattern + ")?[a-zA-Z0-9]+)\\s*(?<eqtype>[\\~\\^\\$\\*\\|]?)=\\s*(\"|\')?(?<attvalue>.*?)(\"|\')?";
		

		internal static string sSelector = "(?<ns>" + nsPattern + ")?" +
			@"(?<type>([A-Za-z\*][A-Za-z0-9]*))?" +
			@"((?<class>\.[A-Za-z][A-Za-z0-9]*)+)?" +
			@"(?<id>\#[A-Za-z][A-Za-z0-9]*)?" +
			@"((?<predicate>\[\s*(" +
			@"(?<attributecheck>(" + nsPattern + ")?[a-zA-Z0-9]+)" +
			@"|" +
			"(?<attributevaluecheck>" + attributeValueCheck + ")" +
			@")\s*\])+)?" +
			@"((?<pseudoclass>\:[a-z\-]+(\([^\)]+\))?)+)?" +
			@"(?<pseudoelements>(\:\:[a-z\-]+)+)?" +
			@"(?<seperator>(\s*(\+|\>|\~)\s*)|(\s+))?";
		private static string sStyleRule = "^((?<selector>(" + sSelector + @")+)(\s*,\s*)?)+";
		private static Regex regex = new Regex(sStyleRule);

		internal static CssRule Parse(ref string css, object parent, bool readOnly,
            IList<string> replacedStrings, CssStyleSheetType origin)
		{
			Match match = regex.Match(css);
			if(match.Success && match.Length > 0)
			{
				CssStyleRule rule = new CssStyleRule(match, parent, readOnly, replacedStrings, origin);

				css = css.Substring(match.Length);

				rule._Style = new CssStyleDeclaration(ref css, rule, readOnly, origin);

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
		/// The constructor for CssStyleRule
		/// </summary>
		/// <param name="match">The Regex match that found the charset rule</param>
		/// <param name="parent">The parent rule or parent stylesheet</param>
		/// <param name="readOnly">True if this instance is readonly</param>
		/// <param name="replacedStrings">An array of strings that have been replaced in the string used for matching. These needs to be put back use the DereplaceStrings method</param>
		/// <param name="origin">The type of CssStyleSheet</param>
		internal CssStyleRule(Match match, object parent, bool readOnly,
            IList<string> replacedStrings, CssStyleSheetType origin)
            : base(parent, readOnly, replacedStrings, origin)
		{
			//SelectorText = DeReplaceStrings(match.Groups["selectors"].Value.Trim());
			//_Style = new CssStyleDeclaration(match, this, readOnly, Origin);

			Group selectorMatches = match.Groups["selector"];

			int len = selectorMatches.Captures.Count;
			ArrayList sels = new ArrayList();
			for(int i = 0; i<len; i++)
			{
                string str = DeReplaceStrings(selectorMatches.Captures[i].Value.Trim());
				if(str.Length > 0)
				{
					sels.Add(new CssXPathSelector(str));
				}
			}
			XPathSelectors = (CssXPathSelector[])sels.ToArray(typeof(CssXPathSelector));
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
			XPathNavigator nav = elt.CreateNavigator();
			foreach(CssXPathSelector sel in XPathSelectors)
			{
				// TODO: deal with pseudoElt
				if(sel != null && sel.Matches(nav))
				{
					((CssStyleDeclaration)Style).GetStylesForElement(csd, sel.Specificity);
					break;
				}
			}
		}
		#endregion

		#region Private fields
		private CssXPathSelector[] XPathSelectors;
		#endregion

		#region Implementation of ICssStyleRule
		/// <summary>
		/// The textual representation of the selector for the rule set. The implementation may have stripped out insignificant whitespace while parsing the selector.
		/// </summary>
		/// <exception cref="DomException">SYNTAX_ERR: Raised if the specified CSS string value has a syntax error and is unparsable.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this rule is readonly</exception>
		public string SelectorText
		{
			get
			{
				string ret = String.Empty;
				foreach(CssXPathSelector sel in XPathSelectors)
				{
					ret += sel.CssSelector + ",";
				}
				return ret.Substring(0, ret.Length-1);
			}
			set
			{
				// TODO: invalidate
				throw new NotImplementedException("setting SelectorText");

			}
		}

		/// <summary>
		/// The entire text of the CssStyleRule
		/// </summary>
		public override string CssText
		{
			get
			{
				return SelectorText + "{" + ((CssStyleDeclaration)Style).CssText + "}";
			}
		}

		private CssStyleDeclaration _Style;
		/// <summary>
		/// The declaration-block of this rule set.
		/// </summary>
		public ICssStyleDeclaration Style
		{
			get
			{
				return _Style;
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
				return CssRuleType.StyleRule;
			}
		}
		#endregion

	}
}
