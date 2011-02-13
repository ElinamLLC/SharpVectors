// <developer>niklas@protocol7.com</developer>
// <completed>100</completed> 

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// The CssValueType Enum Class contains the possible Css Value
    /// Types.  This is an extension to the CSS spec.  The spec has
    /// a list of constants defined within the ICssValue Interface 
    /// </summary>
	public enum CssValueType
	{
		/// <summary>
		/// The value is inherited and the cssText contains "inherit".
		/// </summary>
		Inherit,
		/// <summary>
		/// The value is a primitive value and an instance of the 
        /// CSSPrimitiveValue interface can be obtained by using 
        /// binding-specific casting methods on this instance of 
        /// the CSSValue interface.
		/// </summary>
		PrimitiveValue,
		/// <summary>
		/// The value is a CSSValue list and an instance of the 
        /// CSSValueList interface can be obtained by using 
        /// binding-specific casting methods on this instance 
        /// of the CSSValue interface.
		/// </summary>
		ValueList,
		/// <summary>
		/// The value is a custom value.
		/// </summary>
		Custom
	}
}
