using System;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The CSSImportRule interface represents a @import rule within 
	/// a CSS style sheet. The @import rule is used to import style 
	/// rules from other style sheets. 
	/// </summary>
	/// <developer>niklas@protocol7.com</developer>
	/// <completed>80</completed>	 
	public interface ICssImportRule : ICssRule
	{
		/// <summary>
		/// The style sheet referred to by this rule, if it has been loaded. The value of this attribute is null if the style sheet has not yet been loaded or if it will not be loaded (e.g. if the style sheet is for a media type not supported by the user agent).
		/// </summary>
		SharpVectors.Dom.Css.ICssStyleSheet StyleSheet
		{
			get;
		}
	
		/// <summary>
		/// A list of media types for which this style sheet may be used.
		/// </summary>
		SharpVectors.Dom.Stylesheets.IMediaList Media
		{
			get;
		}
	
		/// <summary>
		/// The location of the style sheet to be imported. The attribute will not contain the "url(...)" specifier around the URI.
		/// </summary>
		string Href
		{
			get;
		}
	}
}
