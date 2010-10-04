// <developer>niklas@protocol7.com</developer>
// <completed>80</completed>

using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSStyleDeclaration interface represents a single CSS declaration block. This interface may be used to determine the style properties currently set in a block or to set style properties explicitly within the block.
	///	While an implementation may not recognize all CSS properties within a CSS declaration block, it is expected to provide access to all specified properties in the style sheet through the CSSStyleDeclaration interface. Furthermore, implementations that support a specific level of CSS should correctly handle CSS shorthand properties for that level. For a further discussion of shorthand properties, see the CSS2Properties interface.
	/// This interface is also used to provide a read-only access to the computed values of an element. See also the ViewCSS interface.
	/// Note: The CSS Object Model doesn't provide an access to the specified or actual values of the CSS cascade
	/// </summary>
	public class CssStyleDeclaration : ICssStyleDeclaration
	{
		#region Static Members

		private static Regex styleRegex = new Regex(
            @"^(?<name>[A-Za-z\-0-9]+)\s*:(?<value>[^;\}!]+)(!\s?(?<priority>important))?;?");
		
        #endregion

        #region Private Fields

        private bool _readOnly;
        private CssStyleSheetType _origin;
        private Dictionary<string, CssStyleBlock> styles = new Dictionary<string, CssStyleBlock>();

        #endregion

		#region Constructors
		/// <summary>
		/// The constructor used internally when collecting styles for a specified element
		/// </summary>
		internal CssStyleDeclaration()
		{
			_origin = CssStyleSheetType.Collector;
			_readOnly = true;
			_parentRule = null;
		}

		/// <summary>
		/// The constructor for CssStyleDeclaration
		/// </summary>
		/// <param name="css">The string to parse for CSS</param>
		/// <param name="parentRule">The parent rule or parent stylesheet</param>
		/// <param name="readOnly">True if this instance is readonly</param>
		/// <param name="origin">The type of CssStyleSheet</param>
		public CssStyleDeclaration(ref string css, CssRule parentRule, bool readOnly, CssStyleSheetType origin)
		{
			_origin = origin;
			_readOnly = readOnly;
			_parentRule = parentRule;

			css = parseString(css);
		}

		public CssStyleDeclaration(string css, CssRule parentRule, bool readOnly, CssStyleSheetType origin)
		{
			_origin = origin;
			_readOnly = readOnly;
			_parentRule = parentRule;

			parseString(css);
		}
		#endregion

        #region Public Properties

        public bool ReadOnly
		{
			get
			{
				return _readOnly;
			}
		}

		public CssStyleSheetType Origin
		{
			get
			{
				return _origin;
			}
        }

        #endregion

        #region Public Methods

        /// <summary>
		/// Used to find matching style rules in the cascading order
		/// </summary>
		internal void GetStylesForElement(CssCollectedStyleDeclaration csd, int specificity)
		{
            foreach (KeyValuePair<string, CssStyleBlock> de in styles)
			{
				CssStyleBlock scs = de.Value;
				csd.CollectProperty(scs.Name, specificity,
                    (CssValue)GetPropertyCssValue(scs.Name), scs.Origin, scs.Priority);
			}
		}

		#endregion

        #region Private Methods

        private string parseString(string cssText)
        {
            bool startedWithABracket = false;

            cssText = cssText.Trim();
            if (cssText.StartsWith("{"))
            {
                cssText = cssText.Substring(1).Trim();
                startedWithABracket = true;
            }

            Match match = styleRegex.Match(cssText);
            while (match.Success)
            {
                string name = match.Groups["name"].Value;
                string value = match.Groups["value"].Value;
                if (_parentRule != null)
                {
                    value = ((CssRule)_parentRule).DeReplaceStrings(value);
                }
                string prio = match.Groups["priority"].Value;

                CssStyleBlock style = new CssStyleBlock(name, value, prio, _origin);

                bool addStyle = false;
                if (styles.ContainsKey(name))
                {
                    string existingPrio = ((CssStyleBlock)styles[name]).Priority;

                    if (existingPrio != "important" || prio == "important")
                    {
                        styles.Remove(name);
                        addStyle = true;
                    }
                }
                else
                {
                    addStyle = true;

                }

                if (addStyle)
                {
                    styles.Add(name, style);
                }

                cssText = cssText.Substring(match.Length).Trim();
                match = styleRegex.Match(cssText);
            }

            cssText = cssText.Trim();
            if (cssText.StartsWith("}"))
            {
                cssText = cssText.Substring(1);
            }
            else if (startedWithABracket)
            {
                throw new DomException(DomExceptionType.SyntaxErr, "Style declaration ending bracket missing");
            }
            return cssText;
        }

        #endregion

		#region ICssStyleDeclaration Members

		/// <summary>
		/// Used to set a property value and priority within this declaration block
		/// </summary>
		/// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
		/// <param name="value">The new value of the property.</param>
		/// <param name="priority">The new priority of the property (e.g. "important").</param>
		/// <exception cref="DomException">SYNTAX_ERR: Raised if the specified value has a syntax error and is unparsable.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this declaration is readonly or the property is readonly.</exception>
		public void SetProperty(string propertyName, string value, string priority)
		{
            if (_readOnly) 
                throw new DomException(DomExceptionType.NoModificationAllowedErr);

			styles[propertyName] = new CssStyleBlock(propertyName, value, priority, _origin);
		}

		/// <summary>
		/// Used to retrieve the priority of a CSS property (e.g. the "important" qualifier) if the property has been explicitly set in this declaration block.
		/// </summary>
		/// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
		/// <returns>A string representing the priority (e.g. "important") if one exists. The empty string if none exists.</returns>
		public virtual string GetPropertyPriority(string propertyName)
		{
			return (styles.ContainsKey(propertyName)) ? styles[propertyName].Priority : String.Empty;
		}

		/// <summary>
		/// Used to remove a CSS property if it has been explicitly set within this declaration block.
		/// </summary>
		/// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
		/// <returns>Returns the value of the property if it has been explicitly set for this declaration block. Returns the empty string if the property has not been set or the property name does not correspond to a known CSS property.</returns>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this declaration is readonly or the property is readonly.</exception>
		public string RemoveProperty(string propertyName)
		{
			if (_readOnly) 
                throw new DomException(DomExceptionType.NoModificationAllowedErr);

			if (styles.ContainsKey(propertyName))
			{
				CssStyleBlock s = styles[propertyName];
				styles.Remove(propertyName);
				return s.Value;
			}
			
            return String.Empty;
		}

		
		
		/// <summary>
		/// Used to retrieve the object representation of the value of a CSS property if it has been explicitly set within this declaration block. This method returns null if the property is a shorthand property. Shorthand property values can only be accessed and modified as strings, using the getPropertyValue and setProperty methods.
		/// </summary>
		/// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
		/// <returns>Returns the value of the property if it has been explicitly set for this declaration block. Returns null if the property has not been set.</returns>
		public virtual ICssValue GetPropertyCssValue(string propertyName)
		{
			if (styles.ContainsKey(propertyName))
			{
				CssStyleBlock scs = styles[propertyName];
				if (scs.CssValue == null)
				{
					scs.CssValue = CssValue.GetCssValue(scs.Value, ReadOnly);
				}
				return scs.CssValue;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Used to retrieve the value of a CSS property if it has been explicitly set within this declaration block.
		/// </summary>
		/// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
		/// <returns>Returns the value of the property if it has been explicitly set for this declaration block. Returns the empty string if the property has not been set.</returns>
		public virtual string GetPropertyValue(string propertyName)
		{
			return (styles.ContainsKey(propertyName)) ? styles[propertyName].Value : String.Empty;
		}

		
		private ICssRule _parentRule;
		/// <summary>
		/// The CSS rule that contains this declaration block or null if this CSSStyleDeclaration is not attached to a CSSRule.
		/// </summary>
		public ICssRule ParentRule
		{
			get
			{
				return _parentRule;
			}
		}

		/// <summary>
		/// The number of properties that have been explicitly set in this declaration block. The range of valid indices is 0 to length-1 inclusive.
		/// </summary>
		public virtual ulong Length
		{
			get
			{
				return (ulong)styles.Count;
			}
		}

		/// <summary>
		/// The parsable textual representation of the declaration block (excluding the surrounding curly braces). Setting this attribute will result in the parsing of the new value and resetting of all the properties in the declaration block including the removal or addition of properties.
		/// </summary>
		/// <exception cref="DomException">SYNTAX_ERR: Raised if the specified CSS string value has a syntax error and is unparsable.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this declaration is readonly or a property is readonly.</exception>
		public virtual string CssText
		{
			get
			{
                StringBuilder builder = new StringBuilder();

				//string ret = String.Empty;
				
				IEnumerator<KeyValuePair<string, CssStyleBlock>> enu = styles.GetEnumerator();
				while (enu.MoveNext())
				{
					CssStyleBlock style = enu.Current.Value;
                    builder.Append(style.CssText);
                    builder.Append(";");
					//ret += style.CssText + ";";
				}

                return builder.ToString();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Used to retrieve the properties that have been explicitly set in this declaration block. The order of the properties retrieved using this method does not have to be the order in which they were set. This method can be used to iterate over all properties in this declaration block.
		/// The name of the property at this ordinal position. The empty string if no property exists at this position.
		/// </summary>
		public virtual string this[ulong index]
		{
			get
			{
				if(index>=Length) return String.Empty;
				else
				{
					int ind = (int)index;
                    IEnumerator<KeyValuePair<string, CssStyleBlock>> iterator = styles.GetEnumerator();
                    iterator.MoveNext();
                    KeyValuePair<string, CssStyleBlock> enu = iterator.Current;
                    for (int i = 0; i < ind; i++)
                    {
                        iterator.MoveNext();
                        enu = iterator.Current;
                    }

					return (string)enu.Key;
				}
			}
		}
		#endregion
	}
}
