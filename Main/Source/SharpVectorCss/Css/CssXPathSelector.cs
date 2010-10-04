using System;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace SharpVectors.Dom.Css
{
	#region Public enums
	internal enum XPathSelectorStatus
	{
		Start, Parsed, Compiled, Error
	}
	#endregion

	public sealed class CssXPathSelector
	{
		#region Static Fields

		internal static Regex reSelector = new Regex(CssStyleRule.sSelector);
		
        #endregion

        #region Internal Fields

        internal XPathSelectorStatus Status = XPathSelectorStatus.Start;
        internal string CssSelector;

        #endregion

        #region Private Fields

        private int _specificity;
        private string sXpath;
        private XPathExpression xpath;
        private IDictionary<string, string> _nsTable;

        #endregion

		#region Constructors and Destructor

		public CssXPathSelector(string selector)
            : this(selector, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase))
		{
		}

        public CssXPathSelector(string selector, IDictionary<string, string> namespaceTable)
		{
			CssSelector = selector.Trim();
			_nsTable = namespaceTable;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Only used for testing!
		/// </summary>
		public string XPath
		{
			get
			{
				if (Status == XPathSelectorStatus.Start)
				{
					GetXPath(null);
				}
                return sXpath;
			}
		}

		public int Specificity
		{
			get
			{
				if (Status == XPathSelectorStatus.Start)
				{
					GetXPath(null);
				}
				if (Status != XPathSelectorStatus.Error) 
                    return _specificity;
				else 
                    return 0;
			}
		}

		#endregion

		#region Private Methods

		private void AddSpecificity(int a, int b, int c)
		{
			_specificity += a*100 + b*10 + c;
		}

		private string NsToXPath(Match match)
		{
			string r = String.Empty;
			Group g = match.Groups["ns"];

			if (g != null && g.Success)
			{
				string prefix = g.Value.TrimEnd(new char[]{'|'});

				if (prefix.Length == 0)
				{
					// a element in no namespace
					r += "[namespace-uri()='']";
				}
				else if (prefix == "*")
				{
					// do nothing, any or no namespace is okey
				}
				else if (_nsTable.ContainsKey(prefix))
				{
					r += "[namespace-uri()='" + _nsTable[prefix] + "']";
				}
				else
				{
					//undeclared namespace => invalid CSS selector
					r += "[false]";
				}
			}
			else if (_nsTable.ContainsKey(String.Empty))
			{
				//if no default namespace has been specified, this is equivalent to *|E. Otherwise it is equivalent to ns|E where ns is the default namespace.

				r += "[namespace-uri()='" + _nsTable[String.Empty] + "']";
			}
			return r;
		}

		private string TypeToXPath(Match match)
		{
			string r = String.Empty;
			Group g = match.Groups["type"];
			string s = g.Value;
			if(!g.Success || s=="*") r = String.Empty;
			else
			{
				r = "[local-name()='" + s + "']";
				AddSpecificity(0, 0, 1);
			}

			return r;
		}

		private string ClassToXPath(Match match)
		{
			string r = String.Empty;
			Group g = match.Groups["class"];

			foreach(Capture c in g.Captures)
			{
				r += "[contains(concat(' ',@class,' '),' " + c.Value.Substring(1) + " ')]";
				AddSpecificity(0, 1, 0);
			}
			return r;
		}

		private string IdToXPath(Match match)
		{
			string r = String.Empty;
			Group g = match.Groups["id"];
			if(g.Success)
			{
				// r = "[id('" + g.Value.Substring(1) + "')]";
				r = "[@id='" + g.Value.Substring(1) + "']";
				AddSpecificity(1, 0, 0);
			}
			return r;
		}

		private string GetAttributeMatch(string attSelector)
		{
			string fullAttName = attSelector.Trim();
			int pipePos = fullAttName.IndexOf("|");
			string attMatch = String.Empty;

			if(pipePos == -1 || pipePos == 0)
			{
				// att or |att => should be in the undeclared namespace
				string attName = fullAttName.Substring(pipePos+1);
				attMatch = "@" + attName;
			}
			else if(fullAttName.StartsWith("*|"))
			{
				// *|att => in any namespace (undeclared or declared)
				attMatch = "@*[local-name()='" + fullAttName.Substring(2) + "']";
			}
			else
			{
				// ns|att => must macht a declared namespace
				string ns = fullAttName.Substring(0, pipePos);
				string attName = fullAttName.Substring(pipePos+1);
				if (_nsTable.ContainsKey(ns))
				{
					attMatch = "@" + ns + ":" + attName;
				}
				else
				{
					// undeclared namespace => selector should fail
					attMatch = "false";
				}
			}
			return attMatch;
		}

		private string PredicatesToXPath(Match match)
		{
			string r = String.Empty;
			Group g = match.Groups["attributecheck"];
			
			foreach(Capture c in g.Captures)
			{
				r += "[" + GetAttributeMatch(c.Value) + "]";
				AddSpecificity(0, 1, 0);
			}

			g = match.Groups["attributevaluecheck"];
			Regex reAttributeValueCheck = new Regex("^" + CssStyleRule.attributeValueCheck + "?$");
	

			foreach(Capture c in g.Captures)
			{
				Match valueCheckMatch = reAttributeValueCheck.Match(c.Value);
				
				string attName = valueCheckMatch.Groups["attname"].Value;
				string attMatch = GetAttributeMatch(attName);
				string eq = valueCheckMatch.Groups["eqtype"].Value;	// ~,^,$,*,|,nothing
				string attValue = valueCheckMatch.Groups["attvalue"].Value;

				switch(eq)
				{
					case "":
						// [foo="bar"] => [@foo='bar']
						r += "[" + attMatch + "='" + attValue + "']";
						break;
					case "~":
						// [foo~="bar"] 
						// an E element whose "foo" attribute value is a list of space-separated values, one of which is exactly equal to "bar"
						r += "[contains(concat(' '," + attMatch + ",' '),' " + attValue + " ')]";
						break;
					case "^":
						// [foo^="bar"]  
						// an E element whose "foo" attribute value begins exactly with the string "bar"
						r += "[starts-with(" + attMatch + ",'" + attValue + "')]";
						break;
					case "$":
						// [foo$="bar"]  
						// an E element whose "foo" attribute value ends exactly with the string "bar"
						int a = attValue.Length - 1;

						r += "[substring(" + attMatch + ",string-length(" + attMatch + ")-" + a + ")='" + attValue + "']";
						break;
					case "*":
						// [foo*="bar"]  
						// an E element whose "foo" attribute value contains the substring "bar"
						r += "[contains(" + attMatch + ",'" + attValue + "')]";
						break;
					case "|":
						// [hreflang|="en"]  
						// an E element whose "hreflang" attribute has a hyphen-separated list of values beginning (from the left) with "en"
						r += "[" + attMatch + "='" + attValue + "' or starts-with(" + attMatch + ",'" + attValue + "-')]";
						break;
				}
				AddSpecificity(0, 1, 0);
			}

			return r;
		}

		private string PseudoClassesToXPath(Match match, XPathNavigator nav)
		{
			int specificityA = 0;
			int specificityB = 1;
			int specificityC = 0;
			string r = String.Empty;
			Group g = match.Groups["pseudoclass"];

			foreach(Capture c in g.Captures)
			{
				Regex reLang = new Regex(@"^lang\(([A-Za-z\-]+)\)$");
				Regex reContains = new Regex("^contains\\((\"|\')?(?<stringvalue>.*?)(\"|\')?\\)$");

				string s = @"^(?<type>(nth-child)|(nth-last-child)|(nth-of-type)|(nth-last-of-type))\(\s*";
				s += @"(?<exp>(odd)|(even)|(((?<a>[\+-]?\d*)n)?(?<b>[\+-]?\d+)?))";
				s += @"\s*\)$";
				Regex reNth = new Regex(s);

				string p = c.Value.Substring(1);

				if(p == "root")
				{
					r += "[not(parent::*)]";
				}
				else if(p.StartsWith("not"))
				{
					string expr = p.Substring(4, p.Length-5);
					CssXPathSelector sel = new CssXPathSelector(expr, _nsTable);

					string xpath = sel.XPath;
					if(xpath != null && xpath.Length>3)
					{
						// remove *[ and ending ]
						xpath = xpath.Substring(2, xpath.Length-3);

						r += "[not(" + xpath + ")]";

						int specificity = sel.Specificity;

						// specificity = 123
						specificityA = (int)Math.Floor((double) specificity / 100);
						specificity -= specificityA*100;
						// specificity = 23
						specificityB = (int)Math.Floor((double) (specificity) / 10);

						specificity -= specificityB * 10;
						// specificity = 3
						specificityC = specificity;
					}
				}
				else if(p == "first-child")
				{
					r += "[count(preceding-sibling::*)=0]";
				}
				else if(p == "last-child")
				{
					r += "[count(following-sibling::*)=0]";
				}
				else if(p == "only-child")
				{
					r += "[count(../*)=1]";
				}
				else if(p == "only-of-type")
				{
					r += "[false]";
				}
				else if(p == "empty")
				{
					r += "[not(child::*) and not(text())]";
				}
				else if(p == "target")
				{
					r += "[false]";
				}
				else if(p == "first-of-type")
				{
					r += "[false]";
					//r += "[.=(../*[local-name='roffe'][position()=1])]";
				}
				else if(reLang.IsMatch(p))
				{
					r += "[lang('" + reLang.Match(p).Groups[1].Value + "')]";
				}
				else if(reContains.IsMatch(p))
				{
					r += "[contains(string(.),'" + reContains.Match(p).Groups["stringvalue"].Value + "')]";
				}
				else if(reNth.IsMatch(p))
				{
					Match m = reNth.Match(p);
					string type = m.Groups["type"].Value;
					string exp = m.Groups["exp"].Value;
					int a = 0;
					int b = 0;
					if(exp == "odd")
					{
						a = 2;
						b = 1;
					}
					else if(exp == "even")
					{
						a = 2;
						b = 0;
					}
					else
					{
						string v = m.Groups["a"].Value;

						if(v.Length == 0) a = 1;
						else if(v.Equals("-")) a = -1;
						else a = Int32.Parse(v);

						if(m.Groups["b"].Success) b = Int32.Parse(m.Groups["b"].Value);
					}


					if(type.Equals("nth-child") || type.Equals("nth-last-child"))
					{
						string axis;
						if(type.Equals("nth-child")) axis = "preceding-sibling";
						else axis = "following-sibling";

						if(a == 0)
						{
							r += "[count(" + axis + "::*)+1=" + b + "]";
						}
						else
						{
							r += "[((count(" + axis + "::*)+1-" + b + ") mod " + a + "=0)and((count(" + axis + "::*)+1-" + b + ") div " + a + ">=0)]";
						}
					}
				}
				AddSpecificity(specificityA, specificityB, specificityC);
			}
			return r;
		}

		private void SeperatorToXPath(Match match, StringBuilder xpath, string cur)
		{
			Group g = match.Groups["seperator"];
			if(g.Success)
			{
				string s = g.Value.Trim();
				if(s.Length == 0) cur += "//*";
				else if(s == ">") cur += "/*";
				else if(s == "+" || s == "~")
				{
					xpath.Append("[preceding-sibling::*");
					if(s == "+")
					{
						xpath.Append("[position()=1]");
					}
					xpath.Append(cur);
					xpath.Append("]");
					cur = String.Empty;
				}
			}
			xpath.Append(cur);
		}

		#endregion

		#region Internal Methods

		internal void GetXPath(XPathNavigator nav)
		{
			this._specificity = 0;
			StringBuilder xpath = new StringBuilder("*");
			
			Match match = reSelector.Match(CssSelector);
			while(match.Success)
			{
				if(match.Success && match.Value.Length > 0)
				{
					string x = String.Empty;
					x += NsToXPath(match);
					x += TypeToXPath(match);
					x += ClassToXPath(match);
					x += IdToXPath(match);
					x += PredicatesToXPath(match);
					x += PseudoClassesToXPath(match, nav);
					SeperatorToXPath(match, xpath, x);
					

				}
				match = match.NextMatch();
			}
			if(nav != null) Status = XPathSelectorStatus.Parsed;
			sXpath = xpath.ToString();
		}

		private XmlNamespaceManager GetNSManager()
		{
			XmlNamespaceManager nsman = new XmlNamespaceManager(new NameTable());

            foreach (KeyValuePair<string, string> dicEnum in _nsTable)
            {
                nsman.AddNamespace(dicEnum.Key, dicEnum.Value);
            }
            //IDictionaryEnumerator dicEnum = _nsTable.GetEnumerator();
            //while(dicEnum.MoveNext())
            //{
            //    nsman.AddNamespace((string)dicEnum.Key, (string)dicEnum.Value);
            //}

			return nsman;

		}

		internal void Compile(XPathNavigator nav)
		{
			if(Status == XPathSelectorStatus.Start)
			{
				GetXPath(nav);
			}
			if(Status == XPathSelectorStatus.Parsed)
			{
				xpath = nav.Compile(sXpath);
				xpath.SetContext(GetNSManager());
				
				Status = XPathSelectorStatus.Compiled;
			}
		}

		public bool Matches(XPathNavigator nav)
		{
			if(Status != XPathSelectorStatus.Compiled)
			{
				Compile(nav);
			}
			if(Status == XPathSelectorStatus.Compiled)
			{
				try
				{
					return nav.Matches(xpath);
				}
				catch
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		#endregion
	}
}
