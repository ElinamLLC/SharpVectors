using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
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

        private IList<StyleSheet> _styleSheets;
        private CssXmlDocument _cssXmlDoc;

        #endregion

        #region Constructors and Destructor

        internal StyleSheetList(CssXmlDocument document)
        {
            _cssXmlDoc = document;
            XmlNodeList pis = document.SelectNodes("/processing-instruction()");
            _styleSheets = new List<StyleSheet>();

            foreach (XmlProcessingInstruction pi in pis)
            {
                if (Regex.IsMatch(pi.Data, "type=[\"']text\\/css[\"']"))
                {
                    _styleSheets.Add(new CssStyleSheet(pi, CssStyleSheetType.Author));
                }
                else
                {
                    _styleSheets.Add(new StyleSheet(pi));
                }
            }

            XmlNodeList styleNodes;
            foreach (string[] name in document._styleElements)
            {
                //TODO: Fixed style finding - yavor87 committed on Sep 17, 2016
                //styleNodes = document.SelectNodes("//*[local-name()='" + name[1] 
                //    + "' and namespace-uri()='" + name[0] + "'][@type='text/css' or not(@type)]");

                styleNodes = document.SelectNodes(
                    "//*[local-name()='" + name[1] + "' and namespace-uri()='" + name[0] + "']");

                if (styleNodes == null)
                {
                    continue;
                }

                foreach (XmlElement elm in styleNodes)
                {
                    var styleType = elm.GetAttribute("type");

                    // Check for valid 'type' attribute of the style element
                    if (string.IsNullOrWhiteSpace(styleType) || 
                        string.Equals(styleType, "text/css", StringComparison.OrdinalIgnoreCase))
                    {
                        _styleSheets.Add(new CssStyleSheet(elm, CssStyleSheetType.Author));
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public void AddCssStyleSheet(CssStyleSheet ss)
        {
            _styleSheets.Add(ss);
        }

        /// <summary>
        /// Used to find matching style rules in the cascading order
        /// </summary>
        /// <param name="elt">The element to find styles for</param>
        /// <param name="pseudoElt">The pseudo-element to find styles for</param>
        /// <param name="csd">A CssStyleDeclaration that holds the collected styles</param>
        public void GetStylesForElement(XmlElement elt, string pseudoElt, CssCollectedStyleDeclaration csd)
        {
            GetStylesForElement(elt, pseudoElt, csd, _cssXmlDoc.Media);
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
            foreach (StyleSheet ss in _styleSheets)
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
            get {
                return (ulong)_styleSheets.Count;
            }
        }

        /// <summary>
        /// Used to retrieve a style sheet by ordinal index. If index is greater than or equal to the number of style sheets in the list, this returns null.
        /// </summary>
        public IStyleSheet this[ulong index]
        {
            get {
                return _styleSheets[(int)index];
            }
        }

        #endregion

        #region IList<IStyleSheet> Members

        public IStyleSheet this[int index]
        {
            get {
                return _styleSheets[index];
            }
            set {
                _styleSheets[index] = (StyleSheet)value;
            }
        }

        public int Count
        {
            get {
                return _styleSheets.Count;
            }
        }

        public bool IsReadOnly
        {
            get {
                return _styleSheets.IsReadOnly;
            }
        }

        public void Add(IStyleSheet item)
        {
            _styleSheets.Add((StyleSheet)item);
        }

        public void Clear()
        {
            _styleSheets.Clear();
        }

        public bool Contains(IStyleSheet item)
        {
            return _styleSheets.Contains((StyleSheet)item);
        }

        public void CopyTo(IStyleSheet[] array, int arrayIndex)
        {
            _styleSheets.CopyTo((StyleSheet[])array, arrayIndex);
        }

        public IEnumerator<IStyleSheet> GetEnumerator()
        {
            return _styleSheets.GetEnumerator();
        }

        public int IndexOf(IStyleSheet item)
        {
            return _styleSheets.IndexOf((StyleSheet)item);
        }

        public void Insert(int index, IStyleSheet item)
        {
            _styleSheets.Insert(index, (StyleSheet)item);
        }

        public bool Remove(IStyleSheet item)
        {
            return _styleSheets.Remove((StyleSheet)item);
        }

        public void RemoveAt(int index)
        {
            _styleSheets.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _styleSheets.GetEnumerator();
        }

        #endregion
    }
}
