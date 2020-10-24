namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The <c>ICssPrimitiveValue</c> interface represents a single CSS value. 
	/// </summary>
	/// <remarks>
	/// This interface may be used to determine the value of a specific style property currently set in a block 
	/// or to set a specific style property explicitly within the block. An instance of this interface might be 
	/// obtained from the <see cref="ICssStyleDeclaration.GetPropertyCssValue"/> method of the <see cref="ICssStyleDeclaration"/> interface. 
	/// A <see cref="ICssPrimitiveValue"/> object only occurs in a context of a CSS property. 
	/// </remarks>
	public interface ICssPrimitiveValue : ICssValue
    {
		/// <summary>
		/// The type of the value as defined by the constants specified above.
		/// </summary>
		CssPrimitiveType PrimitiveType { get; }

		/// <summary>
		/// A method to set the float value with a specified unit. If the property attached with this value cannot 
		/// accept the specified unit or the float value, the value will be unchanged and a <see cref="DomException"/> 
		/// will be raised.
		/// </summary>
		/// <param name="unitType">A unit code as defined above. The unit code can only be a float unit type 
		/// (i.e. <c>NUMBER</c>, <c>PERCENTAGE</c>, <c>EMS</c>, <c>EXS</c>, <c>PX</c>, <c>CM</c>, <c>MM</c>, <c>IN</c>, 
		/// <c>PT</c>, <c>PC</c>, <c>DEG</c>, <c>RAD</c>, <c>GRAD</c>, <c>MS</c>, <c>S</c>, <c>HZ</c>, <c>KHZ</c>, <c>DIMENSION</c>).
		/// </param>
		/// <param name="floatValue">The new float value.</param>
		/// <exception cref="DomException">
		/// <c>INVALID_ACCESS_ERR:</c> Raised if the attached property doesn't support the float value or the unit type.
		/// </exception>
		/// <exception cref="DomException"><c>NO_MODIFICATION_ALLOWED_ERR:</c> Raised if this property is readonly.</exception>
		void SetFloatValue(CssPrimitiveType unitType, double floatValue);

		/// <summary>
		/// This method is used to get a float value in a specified unit. If this CSS value doesn't contain a float value 
		/// or can't be converted into the specified unit, a <see cref="DomException"/> is raised.
		/// </summary>
		/// <param name="unitType">A unit code to get the float value. The unit code can only be a float unit type 
		/// (i.e. <c>NUMBER</c>, <c>PERCENTAGE</c>, <c>EMS</c>, <c>EXS</c>, <c>PX</c>, <c>CM</c>, <c>MM</c>, <c>IN</c>, 
		/// <c>PT</c>, <c>PC</c>, <c>DEG</c>, <c>RAD</c>, <c>GRAD</c>, <c>MS</c>, <c>S</c>, <c>HZ</c>, <c>KHZ</c>, <c>DIMENSION</c>).</param>
		/// <returns>The float value in the specified unit.</returns>
		/// <exception cref="DomException">
		/// <c>INVALID_ACCESS_ERR:</c> Raised if the CSS value doesn't contain a float value or if the float value 
		/// can't be converted into the specified unit.
		/// </exception>
		double GetFloatValue(CssPrimitiveType unitType);

		/// <summary>
		/// A method to set the string value with the specified unit. If the property attached to this value can't accept the 
		/// specified unit or the string value, the value will be unchanged and a <see cref="DomException"/> will be raised.
		/// </summary>
		/// <param name="stringType">A string code as defined above. The string code can only be a string unit type 
		/// (i.e. <c>STRING</c>, <c>URI</c>, <c>IDENT</c>, and <c>ATTR</c>).</param>
		/// <param name="stringValue">The new string value.</param>
		/// <exception cref="DomException">
		/// <c>INVALID_ACCESS_ERR:</c> Raised if the CSS value doesn't contain a string value or if the string 
		/// value can't be converted into the specified unit.
		/// </exception>
		/// <exception cref="DomException">
		/// <c>NO_MODIFICATION_ALLOWED_ERR:</c> Raised if this property is readonly.</exception>
		void SetStringValue(CssPrimitiveType stringType,  string stringValue);

		/// <summary>
		/// This method is used to get the string value. If the CSS value doesn't contain a string value, 
		/// a <see cref="DomException"/> is raised. 
		/// <para>
		/// Note: Some properties (like 'font-family' or 'voice-family') convert a whitespace separated list of idents to a string.
		/// </para>
		/// </summary>
		/// <returns>The string value in the current unit. The current primitiveType can only be a string 
		/// unit type (i.e. <c>STRING</c>, <c>URI</c>, <c>IDENT</c>, and <c>ATTR</c>).</returns>
		/// <exception cref="DomException"><c>INVALID_ACCESS_ERR:</c> Raised if the CSS value doesn't contain a string value.</exception>
		string GetStringValue();

		/// <summary>
		/// This method is used to get the <see cref="ICssCounter"/> value. If this CSS value doesn't contain a counter value, 
		/// a <see cref="DomException"/> is raised. Modification to the corresponding style property can 
		/// be achieved using the <see cref="ICssCounter"/> interface
		/// </summary>
		/// <returns>The Counter value</returns>
		/// <exception cref="DomException">
		/// <c>INVALID_ACCESS_ERR:</c> Raised if the CSS value doesn't contain a Counter value (e.g. this is not <c>COUNTER</c>).
		/// </exception>
		ICssCounter GetCounterValue();

		/// <summary>
		/// This method is used to get the <see cref="ICssRect"/> value. If this CSS value doesn't contain a rect value, 
		/// a <see cref="DomException"/> is raised. Modification to the corresponding style property 
		/// can be achieved using the <see cref="ICssRect"/> interface.
		/// </summary>
		/// <returns>The Rect value</returns>
		/// <exception cref="DomException">
		/// <c>INVALID_ACCESS_ERR:</c> Raised if the CSS value doesn't contain a Rect value. (e.g. this is not <c>RECT</c>).
		/// </exception>
		ICssRect GetRectValue();

		/// <summary>
		/// This method is used to get the RGB color. If this CSS value doesn't contain a RGB color value, 
		/// a <see cref="DomException"/> is raised. Modification to the corresponding style property can be achieved 
		/// using the <see cref="ICssColor"/> interface.
		/// </summary>
		/// <returns>the RGB color value.</returns>
		/// <exception cref="DomException">
		/// <c>INVALID_ACCESS_ERR:</c> Raised if the attached property can't return a RGB color value (e.g. this is not <c>RGBCOLOR</c>).
		/// </exception>
		ICssColor GetRgbColorValue();
	}
}
