namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The <c>ICssFontFaceRule</c> interface represents a <c>@font-face </c>rule in a CSS style sheet. 
	/// The <c>@font-face</c> rule is used to hold a set of font descriptions. 
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
