namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The <c>CssRuleType</c> Enum contains the possible Rule Type Values. This is an extension of the CSS specification.
	/// The spec contains only a list of contstant in the <see cref="ICssRule"/> interface. 
	/// </summary>
	public enum CssRuleType
	{
		/// <summary>
		/// The rule is a CSSUnknownRule.
		/// </summary>
		UnknownRule,
		/// <summary>
		/// The rule is a CSSStyleRule.
		/// </summary>
		StyleRule,
		/// <summary>
		/// The rule is a CSSCharsetRule.
		/// </summary>
		CharsetRule,
		/// <summary>
		/// The rule is a CSSImportRule.
		/// </summary>
		ImportRule,
		/// <summary>
		/// The rule is a CSSMediaRule.
		/// </summary>
		MediaRule,
		/// <summary>
		/// The rule is a CSSFontFaceRule.
		/// </summary>
		FontFaceRule,
		/// <summary>
		/// The rule is a CSSPageRule.
		/// </summary>
		PageRule
	}
}
