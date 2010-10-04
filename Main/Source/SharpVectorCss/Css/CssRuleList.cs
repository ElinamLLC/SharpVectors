// <developer>niklas@protocol7.com</developer>
// <completed>80</completed>

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSRuleList interface provides the abstraction of an ordered collection of CSS rules.
	/// The items in the CSSRuleList are accessible via an integral index, starting from 0.
	/// </summary>
    public sealed class CssRuleList : ICssRuleList
	{
		#region Static members
		/* can take two kind of structures:
		 * rule{}
		 * rule{}
		 * or:
		 * {
		 *	rule{}
		 *	rule{}
		 * }
		 * */
        private void Parse(ref string css, object parent, bool readOnly, IList<string> replacedStrings, CssStyleSheetType origin)
		{
			bool withBrackets = false;
			css = css.Trim();
			if(css.StartsWith("{"))
			{
				withBrackets = true;
				css = css.Substring(1);
			}

			while(true)
			{
				css = css.Trim();
				if(css.Length == 0)
				{
					if(withBrackets)
					{
						throw new DomException(DomExceptionType.SyntaxErr, "Style block missing ending bracket");
					}
					break;
				}
				else if(css.StartsWith("}"))
				{
					// end of block;
					css = css.Substring(1);
					break;
				}
				else if(css.StartsWith("@"))
				{
					#region Parse at-rules
					// @-rule
					CssRule rule;

					// creates and parses a CssMediaRule or return null
					rule = CssMediaRule.Parse(ref css, parent, readOnly, replacedStrings, origin);
					if(rule == null)
					{
						// create ImportRule
                        rule = CssImportRule.Parse(ref css, parent, readOnly, replacedStrings, origin);

						if(rule == null)
						{
							// create CharSetRule
							rule = CssCharsetRule.Parse(ref css, parent, readOnly, replacedStrings, origin);

							if(rule == null)
							{
                                rule = CssFontFaceRule.Parse(ref css, parent, readOnly, replacedStrings, origin);

								if(rule == null)
								{
                                    rule = CssPageRule.Parse(ref css, parent, readOnly, replacedStrings, origin);

									if(rule == null)
									{
                                        rule = CssUnknownRule.Parse(ref css, parent, readOnly, replacedStrings, origin);
									}
								}
							}
						}
					}
					InsertRule(rule);
					#endregion
				}
				else
				{
					// must be a selector or error
					CssRule rule = CssStyleRule.Parse(ref css, parent, readOnly, replacedStrings, origin);
					if(rule != null)
					{
						InsertRule(rule);
					}
					else
					{
                        // this is an unknown rule format, possibly a new kind of selector. Try to find the end of it to skip it

						int startBracket = css.IndexOf("{");
						int endBracket = css.IndexOf("}");
						int endSemiColon = css.IndexOf(";");
						int endRule;

						if(endSemiColon > 0 && endSemiColon < startBracket)
						{
							endRule = endSemiColon;
						}
						else
						{
							endRule = endBracket;
						}


						if(endRule > -1)
						{
							css = css.Substring(endRule+1);
						}
						else
						{
							throw new DomException(DomExceptionType.SyntaxErr, "Can not parse the CSS file");
						}
					}

				//}
				}  
			}
		}
		#endregion
		
		#region Constructor

		/// <summary>
		/// Constructor for CssRuleList
		/// </summary>
		/// <param name="parent">The parent rule or parent stylesheet</param>
		/// <param name="cssText">The CSS text containing the rules that will be in this list</param>
		/// <param name="replacedStrings">An array of strings that have been replaced in the string used for matching. These needs to be put back use the DereplaceStrings method</param>
		/// <param name="origin">The type of CssStyleSheet</param>
		internal CssRuleList(ref string cssText, object parent,
            IList<string> replacedStrings, CssStyleSheetType origin) 
            : this(ref cssText, parent, replacedStrings, false, origin)
		{
		}

		/// <summary>
		/// Constructor for CssRuleList
		/// </summary>
		/// <param name="parent">The parent rule or parent stylesheet</param>
		/// <param name="cssText">The CSS text containing the rules that will be in this list</param>
		/// <param name="readOnly">True if this instance is readonly</param>
		/// <param name="replacedStrings">An array of strings that have been replaced in the string used for matching. These needs to be put back use the DereplaceStrings method</param>
		/// <param name="origin">The type of CssStyleSheet</param>
		public CssRuleList(ref string cssText, object parent,
            IList<string> replacedStrings, bool readOnly, CssStyleSheetType origin)
		{
			Origin = origin;
			ReadOnly = readOnly;
			if(parent is CssRule || parent is CssStyleSheet)
			{
				Parent = parent;
			}
			else
			{
				throw new Exception("The CssRuleList constructor can only take a CssRule or CssStyleSheet as it's first argument " + parent.GetType());
			}

			Parse(ref cssText, parent, readOnly, replacedStrings, origin);
			//AppendRules(cssText, replacedStrings);
		}

		#endregion

		#region Private fields
		private CssStyleSheetType Origin;
		private object Parent;
        private List<CssRule> cssRules = new List<CssRule>();
		private bool ReadOnly;

		#endregion

		#region Public methods
		/// <summary>
		/// Used to find matching style rules in the cascading order
		/// </summary>
		/// <param name="elt">The element to find styles for</param>
		/// <param name="pseudoElt">The pseudo-element to find styles for</param>
		/// <param name="ml">The medialist that the document is using</param>
		/// <param name="csd">A CssStyleDeclaration that holds the collected styles</param>
		internal void GetStylesForElement(XmlElement elt, string pseudoElt, MediaList ml, CssCollectedStyleDeclaration csd)
		{
			ulong len = Length;
			for(ulong i = 0; i<len; i++)
			{
                CssRule csr = (CssRule)this[i];
				csr.GetStylesForElement(elt, pseudoElt, ml, csd);
			}
		}

		internal ulong InsertRule(CssRule rule, ulong index)
		{
			/* TODO:
			 * HIERARCHY_REQUEST_ERR: Raised if the rule cannot be inserted at the specified index e.g. if an @import rule is inserted after a standard rule set or other at-rule.
			 * SYNTAX_ERR: Raised if the specified rule has a syntax error and is unparsable
			*/
			
			if(ReadOnly) throw new DomException(DomExceptionType.NoModificationAllowedErr);
			
			if(index > Length || index<0) throw new DomException(DomExceptionType.IndexSizeErr);
	
			if(index == Length)
			{
				cssRules.Add(rule);
			}
			else
			{
				cssRules.Insert((int) index, rule);
			}

			return index;
		}

		internal ulong InsertRule(CssRule rule)
		{
			return InsertRule(rule, Length);
		}

		
		/// <summary>
		/// Deletes a rule from the list
		/// </summary>
		/// <param name="index">The index of the rule to delete</param>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR</exception>
		/// <exception cref="DomException">INDEX_SIZE_ERR</exception>
		internal void DeleteRule(ulong index)
		{
			if(ReadOnly) throw new DomException(DomExceptionType.InvalidModificationErr);
			
			if(index >= Length || index<0) throw new DomException(DomExceptionType.IndexSizeErr);

			cssRules.RemoveAt((int)index);
		}

		#endregion

		#region Implementation of ICssRuleList
		/// <summary>
		/// The number of CSSRules in the list. The range of valid child rule indices is 0 to length-1 inclusive
		/// </summary>
		public ulong Length
		{
			get
			{
				return (ulong)cssRules.Count;
			}
		}

		/// <summary>
		///     Used to retrieve a CSS rule by ordinal index. The order in this collection represents the order of the rules in the CSS style sheet. If index is greater than or equal to the number of rules in the list, this returns null
		/// </summary>
		public ICssRule this[ulong index]
		{
			get
			{
				return (index<Length) ? cssRules[(int)index] : null;
			}
			set
			{
			}
		}
		#endregion
	}
}
