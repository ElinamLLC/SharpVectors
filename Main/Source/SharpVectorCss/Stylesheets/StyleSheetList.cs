// <developer>niklas@protocol7.com</developer>
// <completed>80</completed>

using System;
using System.Xml;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using SharpVectors.Dom.Css;

namespace SharpVectors.Dom.Stylesheets
{
	/// <summary>
	/// The StyleSheetList interface provides the abstraction of an ordered collection of style sheets.
	/// The items in the StyleSheetList are accessible via an integral index, starting from 0.
	/// </summary>
    public sealed class StyleSheetList : IStyleSheetList
	{
        #region Private Fields

        private List<StyleSheet> styleSheets;
        private CssXmlDocument doc;

        #endregion

		#region Constructors and Destructor

		internal StyleSheetList(CssXmlDocument document)
		{
			doc = document;
			XmlNodeList pis = document.SelectNodes("/processing-instruction()");
			styleSheets = new List<StyleSheet>();

			foreach (XmlProcessingInstruction pi in pis)
			{
				if (Regex.IsMatch(pi.Data, "type=[\"']text\\/css[\"']"))
				{
					styleSheets.Add(new CssStyleSheet(pi, CssStyleSheetType.Author));
				}
				else
				{
					styleSheets.Add(new StyleSheet(pi));
				}

			}

			XmlNodeList styleNodes;
			foreach(string[] name in document.styleElements)
			{
				styleNodes = document.SelectNodes(
					"//*[local-name()='" + name[1] + "' and namespace-uri()='" + name[0] + "'][@type='text/css']");

				foreach(XmlElement elm in styleNodes)
				{
                    styleSheets.Add(new CssStyleSheet(elm, CssStyleSheetType.Author));
				}
			}
		}

		#endregion

		#region Public Methods

		public void AddCssStyleSheet(CssStyleSheet ss)
		{
			styleSheets.Add(ss);
		}

		/// <summary>
		/// Used to find matching style rules in the cascading order
		/// </summary>
		/// <param name="elt">The element to find styles for</param>
		/// <param name="pseudoElt">The pseudo-element to find styles for</param>
		/// <param name="csd">A CssStyleDeclaration that holds the collected styles</param>
		public void GetStylesForElement(XmlElement elt, string pseudoElt, CssCollectedStyleDeclaration csd)
		{
			GetStylesForElement(elt, pseudoElt, csd, doc.Media);
		}

		/// <summary>
		/// Used to find matching style rules in the cascading order
		/// </summary>
		/// <param name="elt">The element to find styles for</param>
		/// <param name="pseudoElt">The pseudo-element to find styles for</param>
		/// <param name="ml">The medialist that the document is using</param>
		/// <param name="csd">A CssStyleDeclaration that holds the collected styles</param>
		internal void GetStylesForElement(XmlElement elt, string pseudoElt, CssCollectedStyleDeclaration csd, MediaList ml)
		{
			foreach (StyleSheet ss in styleSheets)
			{
				ss.GetStylesForElement(elt, pseudoElt, ml, csd);
			}
		}
		
		#endregion

		#region IStyleSheetList Members

		/// <summary>
		/// The number of StyleSheets in the list. The range of valid child stylesheet indices is 0 to length-1 inclusive.
		/// </summary>
		public ulong Length
		{
			get
			{
				return (ulong)styleSheets.Count;
			}
		}

		/// <summary>
		/// Used to retrieve a style sheet by ordinal index. If index is greater than or equal to the number of style sheets in the list, this returns null.
		/// </summary>
		public IStyleSheet this[ulong index]
		{
			get
			{
				return styleSheets[(int)index];
			}
		}

		#endregion
	}
}
