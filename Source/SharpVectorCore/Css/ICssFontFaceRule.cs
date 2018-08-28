namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSFontFaceRule interface represents a @font-face rule in a CSS style sheet. 
    /// The @font-face rule is used to hold a set of font descriptions. 
	/// </summary>
	public interface ICssFontFaceRule : ICssRule 
	{
		/// <summary>
		/// The declaration-block of this rule.
		/// </summary>
		ICssStyleDeclaration Style
		{
			get;
		}
	}
}
