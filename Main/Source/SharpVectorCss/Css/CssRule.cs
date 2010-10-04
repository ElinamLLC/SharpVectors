using System;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Dom.Css
{
	public abstract class CssRule : ICssRule
	{
		#region Constructors

		protected CssRule(object parent, bool readOnly, 
            IList<string> replacedStrings, CssStyleSheetType origin)
		{
			if(parent is CssRule)
			{
				_ParentRule = (CssRule)parent;
			}
			else if(parent is CssStyleSheet)
			{
				_ParentStyleSheet = (CssStyleSheet)parent;
			}
			else
			{
				throw new Exception("The CssRule constructor can only take a CssRule or CssStyleSheet as it's second argument " + parent.GetType());
			}
			this.origin = origin;
            this.replacedStrings = replacedStrings;
            this.readOnly = readOnly;
		}

		#endregion

		#region Private and protected fields
		/// <summary>
		/// The origin stylesheet type of this rule
		/// </summary>
		protected CssStyleSheetType origin;
		private IList<string> replacedStrings;
		/// <summary>
		/// Specifies the read/write state of the instance
		/// </summary>
		protected bool readOnly;

		#endregion

		#region Private and internal methods
		private string StringReplaceEvaluator(Match match)
		{
            int i = Convert.ToInt32(match.Groups["number"].Value);
            string r = replacedStrings[i];
			if(!match.Groups["quote"].Success) r = r.Trim(new char[2]{'\'', '"'});
			return r;
		}

		internal string DeReplaceStrings(string s)
		{
			Regex re = new Regex(@"(?<quote>"")?<<<(?<number>[0-9]+)>>>""?");
			return re.Replace(s, new MatchEvaluator(StringReplaceEvaluator));
		}

		/// <summary>
		/// Used to find matching style rules in the cascading order
		/// </summary>
		/// <param name="elt">The element to find styles for</param>
		/// <param name="pseudoElt">The pseudo-element to find styles for</param>
		/// <param name="ml">The medialist that the document is using</param>
		/// <param name="csd">A CssStyleDeclaration that holds the collected styles</param>
		protected internal virtual void GetStylesForElement(XmlElement elt, 
            string pseudoElt, MediaList ml, CssCollectedStyleDeclaration csd)
		{
		}

		#endregion

		#region Public methods
		/// <summary>
		/// Finds the owner node of this rule
		/// </summary>
		/// <returns>The owner XmlNode</returns>
		public XmlNode ResolveOwnerNode()
		{
			if(ParentRule != null)
			{
				return ((CssRule)ParentRule).ResolveOwnerNode();
			}
			else
			{
				return ((StyleSheet)ParentStyleSheet).ResolveOwnerNode();
			}
		}

		#endregion
		
		#region Implementation of ICssRule
		private CssStyleSheet _ParentStyleSheet = null;
		/// <summary>
		/// The style sheet that contains this rule
		/// </summary>
		public ICssStyleSheet ParentStyleSheet
		{
			get
			{
				return _ParentStyleSheet;
			}
		}

		private CssRule _ParentRule = null;
		/// <summary>
		/// If this rule is contained inside another rule (e.g. a style rule inside an @media block), this is the containing rule. If this rule is not nested inside any other rules, this returns null
		/// </summary>
		public ICssRule ParentRule
		{
			get
			{
				return _ParentRule;
			}
		}

		/// <summary>
		/// The type of the rule, as defined above. The expectation is that binding-specific casting methods can be used to cast down from an instance of the CSSRule interface to the specific derived interface implied by the type.
		/// </summary>
		public abstract CssRuleType Type
		{
			get;
		}

		/// <summary>
		/// The parsable textual representation of the rule. This reflects the current state of the rule and not its initial value.
		/// </summary>
		public virtual string CssText
		{
			get
			{
				throw new NotImplementedException("CssText");
				//return _CssText;
			}
			set
			{
                throw new NotImplementedException("CssText");
			}
		}
		#endregion
	}
}
