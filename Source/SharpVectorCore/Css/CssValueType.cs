namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The <c>CssValueType</c> Enum Class contains the possible CSS Value Types. This is an extension to the 
	/// CSS specification. The specification has a list of constants defined within the ICssValue Interface 
	/// </summary>
	public enum CssValueType
	{
		/// <summary>
		/// The value is inherited and the cssText contains "inherit".
		/// </summary>
		Inherit,
		/// <summary>
		/// The value is a primitive value and an instance of the <see cref="ICssPrimitiveValue"/> interface can be 
		/// obtained by using binding-specific casting methods on this instance of the <see cref="ICssValue"/> interface.
		/// </summary>
		PrimitiveValue,
		/// <summary>
		/// The value is a <see cref="ICssValue"/> list and an instance of the <see cref="ICssValueList"/> interface 
		/// can be obtained by using binding-specific casting methods on this instance of the <see cref="ICssValue"/> interface.
		/// </summary>
		ValueList,
		/// <summary>
		/// The value is a custom value.
		/// </summary>
		Custom
	}
}
