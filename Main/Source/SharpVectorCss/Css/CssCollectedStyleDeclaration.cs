using System;
using System.Xml;
using System.Collections.Generic;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// Used internally for collection of styles for a specific element
	/// </summary>
	public class CssCollectedStyleDeclaration : CssStyleDeclaration
	{
		#region Contructors
		public CssCollectedStyleDeclaration(XmlElement elm)
		{
			_element = elm;
		}
		#endregion

		#region Private fields
		
        private XmlElement _element;
		private Dictionary<string, CssCollectedProperty> collectedStyles =
            new Dictionary<string, CssCollectedProperty>();

		#endregion

		#region Public methods

		public void CollectProperty(string name, int specificity, CssValue cssValue, CssStyleSheetType origin, string priority)
		{
			CssCollectedProperty newProp = new CssCollectedProperty(name, specificity, cssValue, origin, priority);
		
			if (!collectedStyles.ContainsKey(name))
			{
				collectedStyles[name] = newProp;
			}
			else
			{
				CssCollectedProperty existingProp = collectedStyles[name];
				if (newProp.IsBetterThen(existingProp))
				{
					collectedStyles[name] = newProp;
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
			if (collectedStyles.ContainsKey(propertyName))
			{
				CssCollectedProperty scp = collectedStyles[propertyName];
				return scp.Origin;
			}
			else
			{
				return CssStyleSheetType.Unknown;
			}
		}

		/// <summary>
		/// Used to retrieve the priority of a CSS property (e.g. the "important" qualifier) if the property has been explicitly set in this declaration block.
		/// </summary>
		/// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
		/// <returns>A string representing the priority (e.g. "important") if one exists. The empty string if none exists.</returns>
		public override string GetPropertyPriority(string propertyName)
		{
			return (collectedStyles.ContainsKey(propertyName)) ? 
                collectedStyles[propertyName].Priority : String.Empty;
		}

		private ICssValue getParentStyle(string propertyName)
		{
			CssXmlDocument doc = _element.OwnerDocument as CssXmlDocument;
			XmlElement parentNode = _element.ParentNode as XmlElement;
			if(doc != null && parentNode != null)
			{
				ICssStyleDeclaration parentCsd = doc.GetComputedStyle(parentNode, String.Empty);
				if(parentCsd == null)
				{
					return null;
				}
				else
				{
					return parentCsd.GetPropertyCssValue(propertyName);
				}
			}
			else
			{
				return null;
			}
		}

		public override ICssValue GetPropertyCssValue(string propertyName)
		{
			if (collectedStyles.ContainsKey(propertyName))
			{
				CssCollectedProperty scp = collectedStyles[propertyName];
				if (scp.CssValue.CssValueType == CssValueType.Inherit)
				{
					// get style from parent chain
                    return getParentStyle(propertyName);
				}
				else
				{
					return scp.CssValue.GetAbsoluteValue(propertyName, _element);
				}
			}
			else
			{
				// should this property inherit?
				CssXmlDocument doc = (CssXmlDocument)_element.OwnerDocument;

				if (doc.CssPropertyProfile.IsInheritable(propertyName))
				{
					ICssValue parValue = getParentStyle(propertyName);
					if(parValue != null)
					{
						return parValue;
					}
				}

				string initValue = doc.CssPropertyProfile.GetInitialValue(propertyName);
				if (initValue == null)
				{
					return null;
				}
				else
				{
					return CssValue.GetCssValue(initValue, false).GetAbsoluteValue(propertyName, _element);
				}
			}
		}


		/// <summary>
		/// Used to retrieve the value of a CSS property if it has been explicitly set within this declaration block.
		/// </summary>
		/// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
		/// <returns>Returns the value of the property if it has been explicitly set for this declaration block. Returns the empty string if the property has not been set.</returns>
		public override string GetPropertyValue(string propertyName)
		{
			CssValue value = (CssValue)GetPropertyCssValue(propertyName);
			if(value != null)
			{
				return value.CssText;
			}
			else
			{
				return String.Empty;
			}
		}


		/// <summary>
		/// The number of properties that have been explicitly set in this declaration block. The range of valid indices is 0 to length-1 inclusive.
		/// </summary>
		public override ulong Length
		{
			get
			{
				return (ulong)collectedStyles.Count;
			}
		}

		/// <summary>
		/// The parsable textual representation of the declaration block (excluding the surrounding curly braces). Setting this attribute will result in the parsing of the new value and resetting of all the properties in the declaration block including the removal or addition of properties.
		/// </summary>
		/// <exception cref="DomException">SYNTAX_ERR: Raised if the specified CSS string value has a syntax error and is unparsable.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this declaration is readonly or a property is readonly.</exception>
		public override string CssText
		{
			get
			{
				ulong len = Length;
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				for(ulong i = 0; i<len; i++)
				{
					string propName = this[i];
					sb.Append(propName);
					sb.Append(":");
					sb.Append(GetPropertyValue(propName));
					string prio = GetPropertyPriority(propName);
					if(prio.Length > 0)
					{
						sb.Append(" !" + prio);
					}
					sb.Append(";");
				}

				return sb.ToString();
			}
			set
			{
				throw new DomException(DomExceptionType.NoModificationAllowedErr);
			}
		}

		/// <summary>
		/// Used to retrieve the properties that have been explicitly set in this declaration block. The order of the properties retrieved using this method does not have to be the order in which they were set. This method can be used to iterate over all properties in this declaration block.
		/// The name of the property at this ordinal position. The empty string if no property exists at this position.
		/// </summary>
		public override string this[ulong index]
		{
			get
			{
				if (index >= this.Length) 
                    return String.Empty;
				else
				{
                    int ind = (int)index;//Dictionary<string, CssCollectedProperty>
                    IEnumerator<KeyValuePair<string, CssCollectedProperty>> iterator = 
                        collectedStyles.GetEnumerator();
                    iterator.MoveNext();

                    KeyValuePair<string, CssCollectedProperty> enu = iterator.Current;

                    for (int i = 0; i < (int)ind; i++)
                    {
                        iterator.MoveNext();
                        enu = iterator.Current;
                    }

					return enu.Key;
				}
			}
		}
		#endregion
	}
}
