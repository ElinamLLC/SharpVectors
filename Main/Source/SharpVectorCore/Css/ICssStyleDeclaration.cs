// <developer>niklas@protocol7.com</developer>
// <completed>60</completed>	

using System;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSStyleDeclaration interface represents a single CSS 
	/// declaration block. This interface may be used to determine 
	/// the style properties currently set in a block or to set 
	/// style properties explicitly within the block. 
	/// </summary>
	public interface ICssStyleDeclaration
	{
		/// <summary>
		/// Used to set a property value and priority within this declaration block.
		/// </summary>
		/// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
		/// <param name="value">The new value of the property.</param>
		/// <param name="priority">The new priority of the property (e.g. "important").</param>
		/// <exception cref="DomException">SYNTAX_ERR: Raised if the specified value has a syntax error and is unparsable.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this declaration is readonly or the property is readonly.</exception>
		void SetProperty(string propertyName, string value, string priority);
	
		/// <summary>
		/// Used to retrieve the priority of a CSS property (e.g. the "important" qualifier) if the property has been explicitly set in this declaration block.
		/// </summary>
		/// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
		/// <returns>A string representing the priority (e.g. "important") if one exists. The empty string if none exists.</returns>
		string GetPropertyPriority(string propertyName);
	
		/// <summary>
		/// Used to remove a CSS property if it has been explicitly set within this declaration block.
		/// </summary>
		/// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
		/// <returns>Returns the value of the property if it has been explicitly set for this declaration block. Returns the empty string if the property has not been set or the property name does not correspond to a known CSS property.</returns>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this declaration is readonly or the property is readonly.</exception>
		string RemoveProperty(string propertyName);
	
		/// <summary>
		/// Used to retrieve the object representation of the value of a CSS property if it has been explicitly set within this declaration block. This method returns null if the property is a shorthand property. Shorthand property values can only be accessed and modified as strings, using the getPropertyValue and setProperty methods.
		/// </summary>
		/// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
		/// <returns>Returns the value of the property if it has been explicitly set for this declaration block. Returns null if the property has not been set.</returns>
		ICssValue GetPropertyCssValue(string propertyName);
	
		/// <summary>
		/// Used to retrieve the value of a CSS property if it has been explicitly set within this declaration block.
		/// </summary>
		/// <param name="propertyName">The name of the CSS property. See the CSS property index.</param>
		/// <returns>Returns the value of the property if it has been explicitly set for this declaration block. Returns the empty string if the property has not been set.</returns>
		string GetPropertyValue(string propertyName);
	
		/// <summary>
		/// The CSS rule that contains this declaration block or null if this CSSStyleDeclaration is not attached to a CSSRule.
		/// </summary>
		ICssRule ParentRule
		{
			get;
		}
	
		/// <summary>
		/// The number of properties that have been explicitly set in this declaration block. The range of valid indices is 0 to length-1 inclusive.
		/// </summary>
		ulong Length
		{
			get;
		}
	
		/// <summary>
		/// The parsable textual representation of the declaration block (excluding the surrounding curly braces). Setting this attribute will result in the parsing of the new value and resetting of all the properties in the declaration block including the removal or addition of properties.
		/// </summary>
		/// <exception cref="DomException">SYNTAX_ERR: Raised if the specified CSS string value has a syntax error and is unparsable.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this declaration is readonly or a property is readonly.</exception>
		string CssText
		{
			get;
			set;
		}

		/// <summary>
		/// Used to retrieve the properties that have been explicitly set in this declaration block. The order of the properties retrieved using this method does not have to be the order in which they were set. This method can be used to iterate over all properties in this declaration block.
		/// </summary>
		string this[ulong index]
		{
			get;
		}
	}
}
