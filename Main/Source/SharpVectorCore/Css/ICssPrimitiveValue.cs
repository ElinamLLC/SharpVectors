// <developer>niklas@protocol7.com</developer>
// <completed>50</completed>	

using System;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSPrimitiveValue interface represents a single CSS value. 
	/// This interface may be used to determine the value of a specific
	/// style property currently set in a block or to set a specific 
	/// style property explicitly within the block. An instance of this 
	/// interface might be obtained from the getPropertyCSSValue method
	/// of the CSSStyleDeclaration interface. A CSSPrimitiveValue object
	/// only occurs in a context of a CSS property. 
	/// </summary>
	public interface ICssPrimitiveValue
	{
		/// <summary>
		/// The type of the value as defined by the constants specified above.
		/// </summary>
		CssPrimitiveType PrimitiveType{get;}

		/// <summary>
		/// A method to set the float value with a specified unit. If the property attached with this value can not accept the specified unit or the float value, the value will be unchanged and a DOMException will be raised
		/// </summary>
		/// <param name="unitType">A unit code as defined above. The unit code can only be a float unit type (i.e. CSS_NUMBER, CSS_PERCENTAGE, CSS_EMS, CSS_EXS, CSS_PX, CSS_CM, CSS_MM, CSS_IN, CSS_PT, CSS_PC, CSS_DEG, CSS_RAD, CSS_GRAD, CSS_MS, CSS_S, CSS_HZ, CSS_KHZ, CSS_DIMENSION).</param>
		/// <param name="floatValue">The new float value.</param>
		/// <exception cref="DomException">INVALID_ACCESS_ERR: Raised if the attached property doesn't support the float value or the unit type.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this property is readonly.</exception>
		void SetFloatValue(CssPrimitiveType unitType, double floatValue);

		/// <summary>
		/// This method is used to get a float value in a specified unit. If this CSS value doesn't contain a float value or can't be converted into the specified unit, a DOMException is raised.
		/// </summary>
		/// <param name="unitType">A unit code to get the float value. The unit code can only be a float unit type (i.e. CSS_NUMBER, CSS_PERCENTAGE, CSS_EMS, CSS_EXS, CSS_PX, CSS_CM, CSS_MM, CSS_IN, CSS_PT, CSS_PC, CSS_DEG, CSS_RAD, CSS_GRAD, CSS_MS, CSS_S, CSS_HZ, CSS_KHZ, CSS_DIMENSION).</param>
		/// <returns>The float value in the specified unit.</returns>
		/// <exception cref="DomException">INVALID_ACCESS_ERR: Raised if the CSS value doesn't contain a float value or if the float value can't be converted into the specified unit.</exception>
		double GetFloatValue(CssPrimitiveType unitType);

		/// <summary>
		/// A method to set the string value with the specified unit. If the property attached to this value can't accept the specified unit or the string value, the value will be unchanged and a DOMException will be raised.
		/// </summary>
		/// <param name="stringType">A string code as defined above. The string code can only be a string unit type (i.e. CSS_STRING, CSS_URI, CSS_IDENT, and CSS_ATTR).</param>
		/// <param name="stringValue">The new string value.</param>
		/// <exception cref="DomException">INVALID_ACCESS_ERR: Raised if the CSS value doesn't contain a string value or if the string value can't be converted into the specified unit.</exception>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised if this property is readonly.</exception>
		void SetStringValue(CssPrimitiveType stringType,  string stringValue);
	
		/// <summary>
		/// This method is used to get the string value. If the CSS value doesn't contain a string value, a DOMException is raised.
		/// Note: Some properties (like 'font-family' or 'voice-family') convert a whitespace separated list of idents to a string.
		/// </summary>
		/// <returns>The string value in the current unit. The current primitiveType can only be a string unit type (i.e. CSS_STRING, CSS_URI, CSS_IDENT and CSS_ATTR).</returns>
		/// <exception cref="DomException">INVALID_ACCESS_ERR: Raised if the CSS value doesn't contain a string value.</exception>
		string GetStringValue();

		/// <summary>
		/// This method is used to get the Counter value. If this CSS value doesn't contain a counter value, a DOMException is raised. Modification to the corresponding style property can be achieved using the Counter interface
		/// </summary>
		/// <returns>The Counter value</returns>
		/// <exception cref="DomException">INVALID_ACCESS_ERR: Raised if the CSS value doesn't contain a Counter value (e.g. this is not CSS_COUNTER).</exception>
		ICssCounter GetCounterValue();

		/// <summary>
		/// This method is used to get the Rect value. If this CSS value doesn't contain a rect value, a DOMException is raised. Modification to the corresponding style property can be achieved using the Rect interface.
		/// </summary>
		/// <returns>The Rect value</returns>
		/// <exception cref="DomException">INVALID_ACCESS_ERR: Raised if the CSS value doesn't contain a Rect value. (e.g. this is not CSS_RECT).</exception>
		ICssRect GetRectValue();

		/// <summary>
		/// This method is used to get the RGB color. If this CSS value doesn't contain a RGB color value, a DOMException is raised. Modification to the corresponding style property can be achieved using the RGBColor interface.
		/// </summary>
		/// <returns>the RGB color value.</returns>
		/// <exception cref="DomException">INVALID_ACCESS_ERR: Raised if the attached property can't return a RGB color value (e.g. this is not CSS_RGBCOLOR).</exception>
		ICssColor GetRgbColorValue();

	}
}
