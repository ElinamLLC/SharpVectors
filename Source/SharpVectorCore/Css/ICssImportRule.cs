using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The <c>ICssImportRule</c> interface represents a <c>@import</c> rule within a CSS style sheet. 
	/// The <c>@import</c> rule is used to import style rules from other style sheets. 
	/// </summary>
	public interface ICssImportRule : ICssRule
	{
		/// <summary>
		/// The style sheet referred to by this rule, if it has been loaded. The value of this attribute 
        /// is null if the style sheet has not yet been loaded or if it will not be loaded (e.g. if the 
        /// style sheet is for a media type not supported by the user agent).
		/// </summary>
		ICssStyleSheet StyleSheet
		{
			get;
		}
	
		/// <summary>
		/// A list of media types for which this style sheet may be used.
		/// </summary>
		IMediaList Media
		{
			get;
		}
	
		/// <summary>
		/// The location of the style sheet to be imported. The attribute will not contain the 
        /// <c>"url(...)"</c> specifier around the URI.
		/// </summary>
		string Href
		{
			get;
		}
	}
}
