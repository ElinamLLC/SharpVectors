using System;
using System.Text;
using System.Xml;
using System.Collections.Generic;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// Used internally for collection of styles for a specific element
    /// </summary>
    public class CssCollectedStyleDeclaration : CssStyleDeclaration
    {
        #region Private Fields

        private XmlElement _element;
        private IDictionary<string, CssCollectedProperty> _collectedStyles;

        #endregion

        #region Contructors

        private CssCollectedStyleDeclaration()
        {
            _collectedStyles = new Dictionary<string, CssCollectedProperty>(StringComparer.OrdinalIgnoreCase);
        }

        public CssCollectedStyleDeclaration(XmlElement elm)
            : this()
        {
            _element = elm;
        }

        #endregion

        #region Public Methods

        public void CollectProperty(string name, int specificity, CssValue cssValue, CssStyleSheetType origin, string priority)
        {
            CssCollectedProperty newProp = new CssCollectedProperty(name, specificity, cssValue, origin, priority);

            if (!_collectedStyles.ContainsKey(name))
            {
                _collectedStyles[name] = newProp;
            }
            else
            {
                CssCollectedProperty existingProp = _collectedStyles[name];
                if (newProp.IsBetterThen(existingProp))
                {
                    _collectedStyles[name] = newProp;
                }
            }
        }

        /// <summary>
        /// Returns the origin type of the collected property
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>The origin type</returns>
        public CssStyleSheetType GetPropertyOrigin(string propertyName)
        {
            if (_collectedStyles.ContainsKey(propertyName))
            {
                CssCollectedProperty scp = _collectedStyles[propertyName];
                return scp.Origin;
            }
            return CssStyleSheetType.Unknown;
        }

        /// <summary>
        /// Used to retrieve the priority of a CSS property (e.g. the "important" qualifier) if the property has been explicitly set in this declaration block.
        /// </summary>
        /// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
        /// <returns>A string representing the priority (e.g. "important") if one exists. The empty string if none exists.</returns>
        public override string GetPropertyPriority(string propertyName)
        {
            return (_collectedStyles.ContainsKey(propertyName)) ?
                _collectedStyles[propertyName].Priority : string.Empty;
        }

        private ICssValue getParentStyle(string propertyName)
        {
            CssXmlDocument doc = _element.OwnerDocument as CssXmlDocument;
            XmlElement parentNode = _element.ParentNode as XmlElement;
            if (doc != null && parentNode != null)
            {
                ICssStyleDeclaration parentCsd = doc.GetComputedStyle(parentNode, string.Empty);
                if (parentCsd == null)
                {
                    return null;
                }
                return parentCsd.GetPropertyCssValue(propertyName);
            }
            return null;
        }

        public override ICssValue GetPropertyCssValue(string propertyName)
        {
            if (_collectedStyles.ContainsKey(propertyName))
            {
                CssCollectedProperty scp = _collectedStyles[propertyName];
                if (scp.CssValue.CssValueType == CssValueType.Inherit)
                {
                    // get style from parent chain
                    return getParentStyle(propertyName);
                }
                return scp.CssValue.GetAbsoluteValue(propertyName, _element);
            }

            // should this property inherit?
            CssXmlDocument doc = (CssXmlDocument)_element.OwnerDocument;

            if (doc.CssPropertyProfile.IsInheritable(propertyName))
            {
                ICssValue parValue = getParentStyle(propertyName);
                if (parValue != null)
                {
                    return parValue;
                }
            }

            string initValue = doc.CssPropertyProfile.GetInitialValue(propertyName);
            if (initValue == null)
            {
                return null;
            }
            return CssValue.GetCssValue(initValue, false).GetAbsoluteValue(propertyName, _element);
        }


        /// <summary>
        /// Used to retrieve the value of a CSS property if it has been explicitly set within this declaration block.
        /// </summary>
        /// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
        /// <returns>Returns the value of the property if it has been explicitly set for this declaration block. Returns the empty string if the property has not been set.</returns>
        public override string GetPropertyValue(string propertyName)
        {
            CssValue value = (CssValue)GetPropertyCssValue(propertyName);
            if (value != null)
            {
                return value.CssText;
            }
            return string.Empty;
        }


        /// <summary>
        /// The number of properties that have been explicitly set in this declaration block. The range of valid indices is 0 to length-1 inclusive.
        /// </summary>
        public override ulong Length
        {
            get {
                return (ulong)_collectedStyles.Count;
            }
        }

        /// <summary>
        /// The parsable textual representation of the declaration block (excluding the surrounding curly braces). Setting this attribute will result in the parsing of the new value and resetting of all the properties in the declaration block including the removal or addition of properties.
        /// </summary>
        /// <exception cref="DomException">SYNTAX_ERR: Raised if the specified CSS string value has a syntax error and is unparsable.</exception>
        /// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this declaration is readonly or a property is readonly.</exception>
        public override string CssText
        {
            get {
                ulong len = Length;
                StringBuilder sb = new StringBuilder();
                for (ulong i = 0; i < len; i++)
                {
                    string propName = this[i];
                    sb.Append(propName);
                    sb.Append(":");
                    sb.Append(GetPropertyValue(propName));
                    string prio = GetPropertyPriority(propName);
                    if (prio.Length > 0)
                    {
                        sb.Append(" !" + prio);
                    }
                    sb.Append(";");
                }

                return sb.ToString();
            }
            set {
                throw new DomException(DomExceptionType.NoModificationAllowedErr);
            }
        }

        /// <summary>
        /// Used to retrieve the properties that have been explicitly set in this declaration block. The order of the properties retrieved using this method does not have to be the order in which they were set. This method can be used to iterate over all properties in this declaration block.
        /// The name of the property at this ordinal position. The empty string if no property exists at this position.
        /// </summary>
        public override string this[ulong index]
        {
            get {
                if (index >= this.Length)
                    return string.Empty;

                var iterator = _collectedStyles.GetEnumerator();
                iterator.MoveNext();

                KeyValuePair<string, CssCollectedProperty> enu = iterator.Current;

                for (ulong i = 0; i < index; i++)
                {
                    iterator.MoveNext();
                    enu = iterator.Current;
                }

                return enu.Key;
            }
        }

        #endregion
    }
}
