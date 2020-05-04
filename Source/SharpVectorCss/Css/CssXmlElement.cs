using System;
using System.Xml;
using System.Text.RegularExpressions;

using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Dom.Css
{
    public delegate void NodeChangeHandler(object src, XmlNodeChangedEventArgs args);
    public delegate void CssChangeHandler();

    public class CssXmlElement : Element, IElementCssInlineStyle
    {
        #region Private Fields

        private static readonly Regex _reComment = new Regex(@"(//.*)|(/\*(.|\n)*?\*/)");

        protected bool _isImported;
        protected CssXmlElement _importNode;
        protected CssXmlDocument _importDocument;

        protected ICssStyleDeclaration _cachedCSD;

        private ICssStyleDeclaration _style;

        #endregion

        #region Constructors

        public CssXmlElement(string prefix, string localname, string ns, CssXmlDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region Style attribute

        public bool Imported
        {
            get {
                return _isImported;
            }
            set {
                _isImported = value;
            }
        }

        public ICssStyleDeclaration Style
        {
            get {
                if (_style == null)
                {
                    string styleValue = GetAttribute("style", string.Empty);

                    if (!string.IsNullOrWhiteSpace(styleValue))
                    {
                        // remove comments
                        styleValue = _reComment.Replace(styleValue, string.Empty).Trim();
                    }

                    if (string.IsNullOrWhiteSpace(styleValue))
                    {
                        _style = CssStyleDeclaration.EmptyCssStyle;
                    }
                    else
                    {
                        _style = new CssStyleDeclaration(styleValue, null, false, CssStyleSheetType.Inline);
                    }

                }
                return _style;
            }
        }

        #endregion

        #region GetComputedStyle

        public virtual ICssStyleDeclaration GetComputedStyle(string pseudoElt)
        {
            CssXmlDocument ownerDoc = this.OwnerDocument;

            if (_isImported && _importDocument != null && _importNode != null)
            {
                ownerDoc = _importDocument;
            }

            if (_cachedCSD == null)
            {
                CssCollectedStyleDeclaration csd = new CssCollectedStyleDeclaration(this);
                MediaList currentMedia = ownerDoc.Media;

                if (ownerDoc.UserAgentStyleSheet != null)
                {
                    ownerDoc.UserAgentStyleSheet.GetStylesForElement(this, pseudoElt, currentMedia, csd);
                }
                ((StyleSheetList)ownerDoc.StyleSheets).GetStylesForElement(this, pseudoElt, csd);

                ((CssStyleDeclaration)Style).GetStylesForElement(csd, 0);

                if (ownerDoc.UserStyleSheet != null)
                {
                    ownerDoc.UserStyleSheet.GetStylesForElement(this, pseudoElt, currentMedia, csd);
                }

                _cachedCSD = csd;
            }
            return _cachedCSD;
        }

        public virtual string GetComputedStringValue(string propertyName, string pseudoElt)
        {
            ICssStyleDeclaration csd = GetComputedStyle(pseudoElt);
            return csd.GetPropertyValue(propertyName);

        }

        public virtual ICssValue GetComputedCssValue(string propertyName, string pseudoElt)
        {
            ICssStyleDeclaration csd = GetComputedStyle(pseudoElt);
            return csd.GetPropertyCssValue(propertyName);

        }

        #endregion

        #region OwnerDocument

        public new CssXmlDocument OwnerDocument
        {
            get {
                return (CssXmlDocument)base.OwnerDocument;
            }
        }

        #endregion

        #region Supports

        public override bool Supports(string feature, string version)
        {
            var comparer = StringComparison.OrdinalIgnoreCase;

            if (string.Equals(feature, "StyleSheets", comparer) || string.Equals(feature, "CSS", comparer))
            {
                return (string.Equals(version, "2.0", comparer) || string.Equals(version, "3.0", comparer));
            }
            return base.Supports(feature, version);
        }

        #endregion

        #region Update handling

        public virtual void CssInvalidate()
        {
            // TODO: why is this being called during load?
            foreach (XmlNode child in ChildNodes)
            {
                if (child is CssXmlElement)
                {
                    ((CssXmlElement)child).CssInvalidate();
                }
            }

            // Kill the cache
            _cachedCSD = null;

            // Notify
            FireCssChange();
        }

        /// <summary>
        /// Called when this element is changing in one of the following ways
        /// <list type="">
        /// <item>Text child added/removed/changed</item>
        /// <item>Element moved in the tree</item>
        /// </list>
        /// </summary>
        public virtual void ElementChange(Object src, XmlNodeChangedEventArgs args)
        {
            // Invalidate the CSS, the cascade for the CSS heirarchy will need to be recomputed
            CssInvalidate();

            // Notify any listeners
            FireElementChange(src, args);
            FireParentNodeChange(src, args, false);
            FireChildNodeChange(src, args, false);
        }

        /// <summary>
        /// Called when any parent element is changing. If an element is moved the CSS heirarchy for that element 
        /// will need to change.
        /// </summary>
        public virtual void ParentNodeChange(Object src, XmlNodeChangedEventArgs args)
        {
            FireParentNodeChange(src, args, true);
        }

        /// <summary>
        /// Called when any attribute is changing. This is typically triggered by calls to 
        /// setAttribute() and should only be called from the CssXmlDocument.
        /// </summary>
        /// <see cref="CssXmlDocument"/>
        public virtual void AttributeChange(Object src, XmlNodeChangedEventArgs args)
        {
            // Invalidate the CSS, the cascade for the CSS heirarchy will need to be recomputed
            // We do this before and after the change because we need to invalidate the old and new locations
            CssInvalidate();

            XmlAttribute attribute = src as XmlAttribute;

            if (attribute != null)
            {
                HandleAttributeChange(attribute);
            }

            // Notify any listeners
            FireAttributeChange(src, args);
            FireParentNodeChange(src, args, false);
            FireChildNodeChange(src, args, false);

            // Invalidate the CSS, the cascade for the CSS heirarchy will need to be recomputed
            CssInvalidate();
        }

        /// <summary>
        /// This function allows each element to handle it's own behaviors for
        /// attribute changing. By default, the cached computed style is invalidated
        /// because most attributes refer to style properties.
        /// </summary>
        /// <param name="attribute">The attribute that is changing.</param>
        public virtual void HandleAttributeChange(XmlAttribute attribute)
        {
            if (attribute.NamespaceURI.Length == 0)
            {
                switch (attribute.LocalName)
                {
                    case "style":
                        _style = null;
                        break;
                }
            }
        }

        /// <summary>
        /// Called when any child node is changing. If an element is moved the CSS heirarchy for that element 
        /// will need to change. This is mainly useful when one of the child nodes parent is a 
        /// referenced node (for example in a &lt;use&gt; element.
        /// </summary>
        public virtual void ChildNodeChange(object src, XmlNodeChangedEventArgs args)
        {
            FireChildNodeChange(src, args, true);
        }

        protected void FireCssChange()
        {
            if (cssChangeHandler != null)
            {
                cssChangeHandler();
            }
        }

        protected void FireAttributeChange(object src, XmlNodeChangedEventArgs args)
        {
            if (attributeChangeHandler != null)
            {
                attributeChangeHandler(src, args);
            }
        }

        protected void FireElementChange(object src, XmlNodeChangedEventArgs args)
        {
            if (elementChangeHandler != null)
            {
                elementChangeHandler(src, args);
            }
        }

        protected void FireParentNodeChange(object src, XmlNodeChangedEventArgs args, bool fireEvent)
        {
            if (fireEvent && parentNodeChangeHandler != null)
            {
                parentNodeChangeHandler(src, args);
            }

            foreach (XmlNode child in ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Element) continue;

                CssXmlElement cssChild = child as CssXmlElement;

                if (cssChild != null)
                {
                    cssChild.ParentNodeChange(src, args);
                }
            }
        }

        protected void FireChildNodeChange(object src, XmlNodeChangedEventArgs args, bool fireEvent)
        {
            if (fireEvent && childNodeChangeHandler != null)
            {
                childNodeChangeHandler(src, args);
            }

            CssXmlElement cssParent = ParentNode as CssXmlElement;

            if (cssParent != null)
            {
                cssParent.ChildNodeChange(src, args);
            }
        }

        public virtual event NodeChangeHandler attributeChangeHandler;
        public virtual event NodeChangeHandler elementChangeHandler;
        public virtual event NodeChangeHandler parentNodeChangeHandler;
        public virtual event NodeChangeHandler childNodeChangeHandler;
        public virtual event CssChangeHandler cssChangeHandler;

        #endregion
    }
}