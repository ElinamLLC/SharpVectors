namespace SharpVectors.Dom.Css
{
	/// <summary>
	/// The different types of CssStyleSheets
	/// </summary>
	public enum CssStyleSheetType
    {
		/// <summary>
		/// The stylesheet is a user agent stylesheet
		/// </summary>
		UserAgent, 
		/// <summary>
		/// The stylesheet is a author stylesheet
		/// </summary>
		Author, 
		/// <summary>
		/// The stylesheet is a user stylesheet
		/// </summary>
		User, 
		/// <summary>
		/// The styles comes from a inline style attribute
		/// </summary>
		Inline, 
		NonCssPresentationalHints,
		/// <summary>
		/// Used internally for collection of styles for an element
		/// </summary>
		Collector,
		/// <summary>
		/// Used internally for unknown properties
		/// </summary>
		Unknown
	}
}
